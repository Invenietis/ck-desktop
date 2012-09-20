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

        public NoFocusWindow()
        {
            _interopHelper = new WindowInteropHelper( this );
        }

        /// <summary>
        /// Gets the pointer of the window which had focus before the skin was clicked on.
        /// </summary>
        public IntPtr LastFocusedWindowHandle { get { return _lastFocused; } }

        protected override void OnSourceInitialized( EventArgs e )
        {
            CK.Windows.Interop.Win.Functions.SetWindowLong( _interopHelper.Handle, CK.Windows.Interop.Win.WindowLongIndex.GWL_EXSTYLE, (uint)CK.Windows.Interop.Win.WS_EX.NOACTIVATE );

            HwndSource mainWindowSrc = HwndSource.FromHwnd( _interopHelper.Handle );

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

            _wndHook = new HwndSourceHook( WndProc );
            mainWindowSrc.AddHook( _wndHook );

            base.OnSourceInitialized( e );
        }

        protected override void OnMouseLeftButtonDown( MouseButtonEventArgs e )
        {
            GetFocus();
            DragMove();
            base.OnMouseLeftButtonDown( e );
        }

        protected override void OnMouseLeftButtonUp( MouseButtonEventArgs e )
        {
            ReleaseFocus();
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

        IntPtr WndProc( IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled )
        {
            switch( (CK.Windows.Interop.Win.WM)msg )
            {
                case CK.Windows.Interop.Win.WM.SETFOCUS:
                    _lastFocused = wParam;
                    break;
                case CK.Windows.Interop.Win.WM.NCLBUTTONDOWN:
                    _ncbuttondown = true;
                    GetFocus();
                    break;
                case CK.Windows.Interop.Win.WM.NCMOUSEMOVE:
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
