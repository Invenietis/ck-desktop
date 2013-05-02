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

namespace CK.Windows
{
    public class NoFocusWindow : Window
    {
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;

        WindowInteropHelper _interopHelper;
        HwndSourceHook _wndHook;
        IntPtr _lastFocused;
        bool _ncbuttondown;
        bool _hitTestable;

        public NoFocusWindow()
        {
            _interopHelper = new WindowInteropHelper( this );
        }

        public void SetHitTestable( bool hitTestable )
        {
            _hitTestable = hitTestable;
        }

        private void DoSetHitTestable()
        {
            int windowLong = (int)Win.Functions.GetWindowLong( _interopHelper.Handle, Win.WindowLongIndex.GWL_EXSTYLE );
            if( ( ( windowLong & 0x20 ) == 0 ) != _hitTestable )
            {
                int num;
                if( _hitTestable )
                {
                    num = windowLong & -33;
                }
                else
                {
                    num = windowLong | 0x20;
                }
                Win.Functions.SetWindowLong( _interopHelper.Handle, Win.WindowLongIndex.GWL_EXSTYLE, (uint)num );
            }
        }

        protected override void OnSourceInitialized( EventArgs e )
        {
            Win.Functions.SetWindowLong(
                _interopHelper.Handle,
                Win.WindowLongIndex.GWL_EXSTYLE,
                (uint)Win.Functions.GetWindowLong( _interopHelper.Handle, Win.WindowLongIndex.GWL_EXSTYLE ) |
                (uint)Win.WS_EX_NOACTIVATE );

            HwndSourceParameters parameters = new HwndSourceParameters();
            //HwndSource mainWindowSrc = HwndSource.FromHwnd( _interopHelper.Handle );
            parameters.ExtendedWindowStyle = (int)( Win.WS_EX_TOPMOST | Win.WS_EX_TOOLWINDOW | Win.WS_EX_NOACTIVATE );
            parameters.RestoreFocusMode = System.Windows.Input.RestoreFocusMode.None;
            //parameters.AcquireHwndFocusInMenuMode = true;

            DoSetHitTestable();
            var mainWindowSrc = HwndSource.FromHwnd( _interopHelper.Handle );
            _wndHook = new HwndSourceHook( WndProc );
            mainWindowSrc.AddHook( _wndHook );

            mainWindowSrc.CompositionTarget.BackgroundColor = Color.FromArgb( 0, 0, 0, 0 );
            mainWindowSrc.CompositionTarget.RenderMode = RenderMode.Default;

            if( OSVersionInfo.IsWindowsVistaOrGreater && CK.Windows.Interop.Dwm.Functions.IsCompositionEnabled() )
            {
                CK.Windows.Interop.Win.Margins m = new CK.Windows.Interop.Win.Margins() { LeftWidth = -1, RightWidth = -1, TopHeight = -1, BottomHeight = -1 };
                CK.Windows.Interop.Dwm.Functions.ExtendFrameIntoClientArea( _interopHelper.Handle, ref m );
            }
            else
            {
                this.Background = new SolidColorBrush( Colors.WhiteSmoke );
            }


            base.OnSourceInitialized( e );
        }

        protected override void OnMouseLeftButtonDown( MouseButtonEventArgs e )
        {
            //GetFocus();
            DragMove();
            base.OnMouseLeftButtonDown( e );
        }

        protected override void OnMouseLeftButtonUp( MouseButtonEventArgs e )
        {
            //ReleaseFocus();
            base.OnMouseLeftButtonUp( e );
        }

        void GetFocus()
        {
            _lastFocused = CK.Windows.Interop.Win.Functions.GetForegroundWindow();
            CK.Windows.Interop.Win.Functions.SetForegroundWindow( _interopHelper.Handle );
        }

        void ReleaseFocus()
        {
            CK.Windows.Interop.Win.Functions.SetForegroundWindow( _lastFocused );
        }

        private object HandleDeactivateApp( object arg )
        {
            //if( !this.StaysOpen )
            //{
            //    base.SetCurrentValueInternal( IsOpenProperty, BooleanBoxes.FalseBox );
            //}
            //this.FirePopupCouldClose();
            return null;
        }

        IntPtr WndProc( IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled )
        {
            if( msg != Win.WM_ACTIVATEAPP )
            {
                if( msg == Win.WM_MOUSEACTIVATE )
                {
                    handled = true;
                    return new IntPtr( 3 );
                }
                if( msg != Win.WM_WINDOWPOSCHANGING )
                {
                    return IntPtr.Zero;
                }
            }
            else
            {
                if( wParam == IntPtr.Zero )
                {
                    base.Dispatcher.BeginInvoke( DispatcherPriority.Normal, new DispatcherOperationCallback( this.HandleDeactivateApp ), null );
                }
                return IntPtr.Zero;
            }

            switch( msg )
            {
                case Win.WM_MOUSEACTIVATE:
                    return (IntPtr)0x0003;
                    handled = true;
                    return new IntPtr( 3 );

                case CK.Windows.Interop.Win.WM_SETFOCUS:
                    _lastFocused = hWnd;
                    break;
                case CK.Windows.Interop.Win.WM_NCLBUTTONDOWN:
                    _ncbuttondown = true;
                    GetFocus();
                    break;
                case CK.Windows.Interop.Win.WM_NCMOUSEMOVE:
                    if( _ncbuttondown )
                    {
                        ReleaseFocus();
                        _ncbuttondown = false;
                    }
                    break;
            }
            return hWnd;
        }

        protected override void OnStateChanged( EventArgs e )
        {
            if( WindowState == System.Windows.WindowState.Maximized )
                WindowState = System.Windows.WindowState.Normal;
        }

        #region WindowPlacement methods

        public void SetPlacement( WINDOWPLACEMENT placement )
        {
            placement.length = Marshal.SizeOf( typeof( WINDOWPLACEMENT ) );
            placement.flags = 0;
            placement.showCmd = ( placement.showCmd == SW_SHOWMINIMIZED ? SW_SHOWNORMAL : placement.showCmd );
            SetWindowPlacement( _interopHelper.Handle, ref placement );
        }

        public WINDOWPLACEMENT GetPlacement()
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            GetWindowPlacement( _interopHelper.Handle, out placement );
            return placement;
        }

        [System.Runtime.InteropServices.DllImport( "user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall )]
        private static extern bool SetWindowPlacement( IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl );

        [System.Runtime.InteropServices.DllImport( "user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall )]
        private static extern bool GetWindowPlacement( IntPtr hWnd, out WINDOWPLACEMENT lpwndpl );
    }
    // RECT structure required by WINDOWPLACEMENT structure
    [Serializable]
    [StructLayout( LayoutKind.Sequential )]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        public RECT( int left, int top, int right, int bottom )
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }
    }

    // POINT structure required by WINDOWPLACEMENT structure
    [Serializable]
    [StructLayout( LayoutKind.Sequential )]
    public struct POINT
    {
        public int X;
        public int Y;

        public POINT( int x, int y )
        {
            this.X = x;
            this.Y = y;
        }
    }

    // WINDOWPLACEMENT stores the position, size, and state of a window
    [Serializable]
    [StructLayout( LayoutKind.Sequential )]
    public struct WINDOWPLACEMENT
    {
        public int length;
        public int flags;
        public int showCmd;
        public POINT minPosition;
        public POINT maxPosition;
        public RECT normalPosition;
    }

        #endregion

}





