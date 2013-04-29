#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Core\CKWindow.Win8.cs) is part of CiviKey. 
*  
* CiviKey is free software: you can redistribute it and/or modify 
* it under the terms of the GNU Lesser General Public License as published 
* by the Free Software Foundation, either version 3 of the License, or 
* (at your option) any later version. 
*  
* CiviKey is distributed in the hope that it will be useful, 
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
* GNU Lesser General Public License for more details. 
* You should have received a copy of the GNU Lesser General Public License 
* along with CiviKey.  If not, see <http://www.gnu.org/licenses/>. 
*  
* Copyright © 2007-2013, 
*     Invenietis <http://www.invenietis.com>,
*     In’Tech INFO <http://www.intechinfo.fr>,
* All rights reserved. 
*-----------------------------------------------------------------------------*/
#endregion

#if DEBUG
#define WINTRACE
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Interop;
using System.Windows.Threading;
using CK.Core;
using CK.Windows.Interop;

namespace CK.Windows
{
    public partial class CKWindow
    {
        IntPtr WndProcWin8( IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled )
        {
            RestoreHopeWin8 hope = (RestoreHopeWin8)_hopeRestorer;
            switch( msg )
            {
                // Only clicks if desktop composition isn't enabled
                case Win.WM_NCHITTEST:
                    {
                        int hit = Win.Functions.DefWindowProc( _hwnd, msg, wParam, lParam ).ToInt32();
                        if( hit == Win.HTClient
                            && _isExtendedFrame
                            && IsOnExtendedFrame( PointFromLParam( lParam ) ) )
                        {
                            hit = Win.HTCaption;
                        }
                        handled = true;
                        return new IntPtr( hit );
                    }
                case Win.WM_DWMCOMPOSITIONCHANGED:
                    {
                        TryExtendFrame();
                        return IntPtr.Zero;
                    }
                // From MSDN:
                //  The WS_EX_NOACTIVATE value for dwExStyle prevents foreground activation by the system. 
                //  To prevent queue activation when the user clicks on the window, you must process the WM_MOUSEACTIVATE message appropriately. 
                //  To bring the window to the foreground or to activate it programmatically, use SetForegroundWindow or SetActiveWindow. 
                //  Returning FALSE to WM_NCACTIVATE prevents the window from losing queue activation. However, the return value is ignored at activation time.
                //case Win.WM.MOUSEACTIVATE:
                //    {
                //        handled = true;
                //        Console.WriteLine( "MOUSEACTIVATE" );

                //        // Returning MA_NOACTIVATEANDEAT leads to "dead", non draggable window.
                //        // return new IntPtr( Win.MA_NOACTIVATEANDEAT );

                //        // Returning MA_NOACTIVATE does not change the behavior: a NCActivate is received.
                //        // Seems that intercepting this message is absolutely useless.
                //        return new IntPtr( Win.MA_NOACTIVATE );
                //    }
                case Win.WM_NCACTIVATE:
                    {
                        // Console.WriteLine( "{0} - NCACTIVATE - wParam={1}", ToString(), wParam.ToInt32() );

                        // From MSDN:
                        //  When the wParam parameter is FALSE, an application should return TRUE to indicate that the system should proceed with 
                        //  the default processing, or it should return FALSE to prevent the change. 
                        //  When wParam is TRUE, the return value is ignored.
                        //
                        //      handled = true;
                        //      return IntPtr.Zero;
                        //
                        // This is simply false: when wParam is true, returning false from NCACTIVATE prevents not only
                        // this window to be activated, but also durably disable any activable window of the process!
                        //
                        // We can not blindly return false here. We must take into account the wParam that tells us something like "Hey, you'd better 
                        // accept to activate otherwise strange things will occur!".
                        //
                        // Note: when wParam is false, this does not change anything if the previously activated window is an external one,
                        //       but for internal ones, this removes an horrible blink of this window.
                        // 
                        // From MSDN: If the window is minimized when this message is received, the application should pass the 
                        //            message to the DefWindowProc function.
                        if( WindowState == System.Windows.WindowState.Minimized ) break;

                        if( !ShowActivated )
                        {
                            // This avoids the NC area's blinking.
                            if( wParam.ToInt32() != 0 )
                            {
                                WinTrace( _hwnd, "NC Activation ignored for non activable window." );
                                handled = true;
                                return IntPtr.Zero;
                            }
                        }
                        else
                        {
                            // if( wParam.ToInt32() == 0 )
                            //
                            // We are an Activable window and we are asked to deactivate.
                            // If the future window is an activable one, we must do nothing (accept the deactivation).
                            // If the future window is not activable, the "restore hope" will reactivate it and everything
                            // works fine... 
                            // Except that this Activable window blink awfully.
                            //
                            // Idea: We can handle the message so that we refuse the deactivation. But this is 
                            // dangerous: refusing the deactivation forbids any action from the user in another window.
                            //
                            // (How can we detect that the future window is Activable? I did not find any totally safe way
                            // to do this. Avoiding the blink should be done somewhere else.)
                            //
                            // YES! after numerous tries, it is as easy as returning true (to accept the deactivation), but handling the 
                            // message so that redrawing of the deactivated non-client area is not processed (we claim to have done it).
                            // We use "restore hope" to push the fact that deactivation has been refused for this window.
                            // "Restore hope" calls us back with a WM_NCACTIVATE after having set the _civikeyWindowDeactivation to be this
                            // window handle: when we receive this "real" deativation message, we process it normally.
                            //

                            if( wParam.ToInt32() == 0 )
                            {
                                if( hope.CurrentHWndMessageTarget != _hwnd )
                                {
                                    WinTrace( _hwnd, "Refusing deactivation of Activable Window." );
                                    hope.PushDeactivate( _hwnd );
                                    handled = true;
                                    return Win.PtrTrue;
                                }
                                else
                                {
                                    // We are beeing deactivated by "restore hope" callback.
                                    // Do not do anything to let the deactivation occurs normally.
                                }
                            }
                        }
                        break;
                    }
                case Win.WM_ACTIVATE:
                    {
                        // This WM_ACTIVATE message is sent to the 2 windows: the one that is activated 
                        // and the one that is deactivated.
                        // We use it to capture the previously activated window, if it is another window of this application.
                        // From MSDN:
                        //  This handle can be NULL, and is always NULL when the window being activated and 
                        //  the window being deactivated are in separate processes.

                        // If we are deactivated no information about the deactivated one, we have nothing to do.
                        if( lParam != IntPtr.Zero && (wParam.ToInt32()&0xFFFF) != Win.WA_INACTIVE )
                        {
                            // We are activating and we have the handle of the previously active one.
                            // If the previous is an activable one, memorizes it in order to restore its activation later.
                            if( (Win.Functions.GetWindowLong( lParam, Win.WindowLongIndex.GWL_EXSTYLE ).ToInt32() & (int)Win.WS_EX.NOACTIVATE) == 0 )
                            {
                                // If we are an activable window, calls the overridable SetActiveTarget.
                                if( ShowActivated )
                                {
                                    SetActiveTarget( lParam, isExternalWindow: false );
                                }
                                else
                                {
                                    // When we are a non-activable window, we reject the activation by scheduling
                                    // the reactivation of the previously activated one.
                                    WinTrace( _hwnd, "Added to restore hwnd {0:X}.", lParam.ToInt32() );
                                    hope.PushSetForeground( lParam );
                                }
                            }
                        }
                        break;
                    }
                case Win.WM_ACTIVATEAPP:
                    {
                        // Previous code does not work at all for windows in other processes...
                        // We must handle the case explicitely.
                        // This message seems to be the right starting point: we know the thread identifier of the application that 
                        // lost the focus, but better: the GetForegroundWindow function works and gives us the previous window handle.
                        if( wParam == Win.PtrFalse )
                        {
                            // All CiviKeyWindow intances will do the same job here (and I don't care to avoid this).
                            IntPtr prev = Win.Functions.GetForegroundWindow();
                            // When prev is null, it is the desktop. We can NOT ignore it otherwise we will try to send message to 
                            // another application (and the security of Windows will make the SetForegroundWindow call successful but 
                            // useless).
                            SetActiveTarget( prev, true ); 
                        }
                        break;
                    }
            }
            return IntPtr.Zero;
        }

        class RestoreHopeWin8 : RestoreHope
        {
            /// <summary>
            /// This buffer secures the fact that there may be more than one activation/deactivation
            /// cycle between asynchronously restoring the activation.
            /// Current implementation uses DispatcherPriority.ApplicationIdle. It could be made more "aggressive" by 
            /// using DispatcherPriority.Background but I prefer staying the most "outside of the system", the less intrusive,
            /// as possible: ApplicationIdle seems to work well and has a quite low priority.
            /// </summary>
            readonly FIFOBuffer<Tuple<IntPtr,bool>> _restoreHopeHwnd = new FIFOBuffer<Tuple<IntPtr, bool>>( 32 );

            public void PushSetForeground( IntPtr hWnd )
            {
                _restoreHopeHwnd.Push( new Tuple<IntPtr, bool>( hWnd, true ) );
                TriggerRestoreHope();
            }

            public void PushDeactivate( IntPtr hWnd )
            {
                _restoreHopeHwnd.Push( new Tuple<IntPtr, bool>( hWnd, false ) );
                TriggerRestoreHope();
            }

            public override HwndSourceHook GetWndProc( CKWindow w )
            {
                return w.WndProcWin8;
            }

            protected override void DoRestoreHope()
            {
                if( _restoreHopeHwnd.Count > 0 )
                {
                    WinTrace( "==> Restore Hwnd Count = {0}.", _restoreHopeHwnd.Count );
                    var last = _restoreHopeHwnd.ToArray();
                    _restoreHopeHwnd.Clear();
                    IntPtr hasBeenActivated = IntPtr.Zero;
                    for( int i = last.Length - 1; i >= 0; --i )
                    {
                        var w = last[i];
                        if( w.Item2 )
                        {
                            if( hasBeenActivated == IntPtr.Zero )
                            {
                                WinTrace( "==> Restoring Hwnd {0:X}.", w.Item1.ToInt32() );
                                if( Win.Functions.SetForegroundWindow( w.Item1 ) != 0 ) hasBeenActivated = w.Item1;
                            }
                        }
                        else
                        {
                            if( w.Item1 != hasBeenActivated )
                            {
                                WinTrace( "==> Actual deactivation of {0:X}.", w.Item1.ToInt32() );
                                SendHopeMessage( w.Item1, Win.WM_NCACTIVATE, Win.PtrFalse, IntPtr.Zero );
                            }
                        }
                    }
                    WinTrace( "==> End of restore Hwnd" );
                }
            }
        }

    }
}
