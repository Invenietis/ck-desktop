#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Core\NoFocusWindow.cs) is part of CiviKey. 
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
* Copyright © 2007-2012, 
*     Invenietis <http://www.invenietis.com>,
*     In’Tech INFO <http://www.intechinfo.fr>,
* All rights reserved. 
*-----------------------------------------------------------------------------*/
#endregion

using System;
using System.Windows;
using System.Windows.Interop;
using CK.Core;
using System.Windows.Media;
using CK.Interop;
using System.Windows.Input;
using System.Runtime.InteropServices;
using CK.Windows.Interop;
using System.Windows.Threading;
using System.Collections.Generic;

namespace CK.Windows
{
    public class CiviKeyWindow : Window
    {
        /// <summary>
        /// This buffer secures the fact that there may be more than one activation/deactivation
        /// cycle between asynchronously restoring the activation.
        /// Current implementation uses DispatcherPriority.ApplicationIdle. It could be made more "aggressive" by 
        /// using DispatcherPriority.Background but I prefer staying the most "outside of the system", the less intrusive,
        /// as possible: ApplicationIdle seems to work well and has a quite low priority.
        /// </summary>
        [ThreadStatic]
        static readonly FIFOBuffer<Tuple<IntPtr,bool>> _restoreHopeHwnd = new FIFOBuffer<Tuple<IntPtr, bool>>( 32 );
        [ThreadStatic]
        static readonly DispatcherOperationCallback _restoreHope = new DispatcherOperationCallback( RestoreHope );
        [ThreadStatic]
        static IntPtr _civikeyWindowDeactivation;

        IntPtr _hwnd;
        bool _isExtendedFrame;

        protected override void OnSourceInitialized( EventArgs e )
        {
            _hwnd = new WindowInteropHelper( this ).Handle;
            HwndSource hSource = HwndSource.FromHwnd( _hwnd );

            if( !ShowActivated )
            {
                Win.Functions.SetWindowLong(
                            _hwnd,
                            Win.WindowLongIndex.GWL_EXSTYLE,
                            (uint)Win.Functions.GetWindowLong( _hwnd, Win.WindowLongIndex.GWL_EXSTYLE ) |
                            (uint)Win.WS_EX.NOACTIVATE );
            }
            hSource.AddHook( WndProc );
            hSource.CompositionTarget.BackgroundColor = Color.FromArgb( 0, 0, 0, 0 );
            hSource.CompositionTarget.RenderMode = RenderMode.Default;
            TryExtendFrame();
            base.OnSourceInitialized( e );
        }

        void TryExtendFrame()
        {
            try
            {
                _isExtendedFrame = OSVersionInfo.IsWindowsVistaOrGreater && Dwm.Functions.IsCompositionEnabled();
                if( _isExtendedFrame )
                {
                    // Negative margins have special meaning to DwmExtendFrameIntoClientArea.
                    // Negative margins create the "sheet of glass" effect, where the client 
                    // area is rendered as a solid surface without a window border.
                    Win.Margins m = new CK.Windows.Interop.Win.Margins() { LeftWidth = -1, RightWidth = -1, TopHeight = -1, BottomHeight = -1 };
                    Dwm.Functions.ExtendFrameIntoClientArea( _hwnd, ref m );
                }
            }
            catch
            {
                _isExtendedFrame = false;
                Background = new SolidColorBrush( Colors.WhiteSmoke );
            }
        }

        IntPtr WndProc( IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled )
        {
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
                                if( _civikeyWindowDeactivation != _hwnd )
                                {
                                    Console.WriteLine( "Refusing deactivation of Activable Window {0:X}.", _hwnd.ToInt32() );
                                    _restoreHopeHwnd.Push( new Tuple<IntPtr, bool>( _hwnd, false ) );
                                    Dispatcher.BeginInvoke( DispatcherPriority.ApplicationIdle, _restoreHope, null );
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
                                    Console.WriteLine( "{0}: added hwnd {1:X}.", ToString(), lParam.ToInt32() );
                                    _restoreHopeHwnd.Push( new Tuple<IntPtr,bool>( lParam, true ) );
                                    Dispatcher.BeginInvoke( DispatcherPriority.ApplicationIdle, _restoreHope, null );
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
                            // When null, it the desktop: we ignore it.
                            if( prev != IntPtr.Zero ) SetActiveTarget( prev, true ); 
                        }
                        break;
                    }
            }
            return IntPtr.Zero;
        }

        /// <summary>
        /// Called for activable Windows (when <see cref="Window.ShowActivated"/> is true).
        /// This can be used to manually restore the focus to previously active window.
        /// </summary>
        /// <param name="hWnd">The handle window of the window that just lost the focus.</param>
        /// <param name="isExternalWindow">True if the window is in another process. False if it is an internal (activable) window.</param>
        protected virtual void SetActiveTarget( IntPtr hWnd, bool isExternalWindow )
        {
        }

        /// <summary>
        /// Gets the Win32 window handle of this <see cref="Window"/> object.
        /// Available once <see cref="Window.SourceInitialized"/> has been raised.
        /// </summary>
        protected IntPtr ThisWindowHandle 
        { 
            get { return _hwnd; } 
        }

        /// <summary>
        /// Heart of the "Restore Hope" mechanism.
        /// </summary>
        static object RestoreHope( object p )
        {
            if( _restoreHopeHwnd.Count > 0 )
            {
                Console.WriteLine( "==> Restore Hwnd Count = {0}.", _restoreHopeHwnd.Count );
                IntPtr hasBeenActivated = IntPtr.Zero;
                var last = _restoreHopeHwnd.ToArray();
                for( int i = last.Length - 1; i >= 0; --i )
                {
                    var w = last[i];
                    if( w.Item2 )
                    {
                        if( hasBeenActivated == IntPtr.Zero )
                        {
                            Console.WriteLine( "==> Restoring Hwnd {0:X}.", w.Item1.ToInt32() );
                            if( Win.Functions.SetForegroundWindow( w.Item1 ) != 0 ) hasBeenActivated = w.Item1;
                        }
                    }
                    else
                    {
                        if( w.Item1 != hasBeenActivated )
                        {
                            Console.WriteLine( "==> Actual deactivation of {0:X}.", w.Item1.ToInt32() );
                            _civikeyWindowDeactivation = w.Item1;
                            Win.Functions.SendMessage( w.Item1, Win.WM_NCACTIVATE, Win.PtrFalse, IntPtr.Zero );
                            _civikeyWindowDeactivation = IntPtr.Zero;
                        }
                    }
                }
                Console.WriteLine( "==> End of restore Hwnd" );
                _restoreHopeHwnd.Clear();
            }
            return null;
        }

        Point PointFromLParam( IntPtr lParam )
        {
            return new Point( lParam.ToInt32() & 0xFFFF, lParam.ToInt32() >> 16 );
        }

        bool IsOnExtendedFrame( Point p )
        {
            var point = PointFromScreen( p );
            Visual client = Content as Visual;
            if( client != null )
            {
                HitTestResult result = VisualTreeHelper.HitTest( client, point );
                if( result != null ) return IsDraggableVisual( result.VisualHit );
            }
            // Nothing was hit - assume that this area is covered by frame extensions anyway
            return true;
        }

        /// <summary>
        /// By default, the <see cref="ContentControl.Content"/> is draggable. Any other element are not.
        /// By overriding this method, other visual elements can be considered as a handle to drag the window.
        /// </summary>
        /// <param name="visualElement">Visual element that could be considered as a handle to drag the window.</param>
        /// <returns>True if the element must drag the window.</returns>
        protected virtual bool IsDraggableVisual( DependencyObject visualElement )
        {
            return visualElement == Content;
        }

    }

}