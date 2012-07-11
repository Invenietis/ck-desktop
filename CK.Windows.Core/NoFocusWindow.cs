using System;
using System.Windows;
using System.Windows.Interop;
using CK.Core;
using System.Windows.Media;
using CK.Interop;
using System.Windows.Input;
using System.Runtime.InteropServices;
using CK.WindowsInterop;

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

        protected override void OnSourceInitialized( EventArgs e )
        {
            CK.WindowsInterop.Win.Functions.SetWindowLong( _interopHelper.Handle, CK.WindowsInterop.Win.WindowLongIndex.GWL_EXSTYLE, (uint)CK.WindowsInterop.Win.WS_EX.NOACTIVATE );

            HwndSource mainWindowSrc = HwndSource.FromHwnd( _interopHelper.Handle );

            mainWindowSrc.CompositionTarget.BackgroundColor = Color.FromArgb( 0, 0, 0, 0 );
            mainWindowSrc.CompositionTarget.RenderMode = RenderMode.Default;

            if( OSVersionInfo.IsWindowsVistaOrGreater && CK.WindowsInterop.Dwm.Functions.IsCompositionEnabled() )
            {
                CK.WindowsInterop.Win.Margins m = new CK.WindowsInterop.Win.Margins() { LeftWidth = -1, RightWidth = -1, TopHeight = -1, BottomHeight = -1 };
                CK.WindowsInterop.Dwm.Functions.ExtendFrameIntoClientArea( _interopHelper.Handle, ref m );
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
            _lastFocused = CK.WindowsInterop.Win.Functions.GetForegroundWindow();
            CK.WindowsInterop.Win.Functions.SetForegroundWindow( _interopHelper.Handle );
        }

        void ReleaseFocus()
        {
            CK.WindowsInterop.Win.Functions.SetForegroundWindow( _lastFocused );
        }

        IntPtr WndProc( IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled )
        {
            switch( (CK.WindowsInterop.Win.WM)msg )
            {
                case CK.WindowsInterop.Win.WM.NCLBUTTONDOWN:
                    _ncbuttondown = true;
                    GetFocus();
                    break;
                case CK.WindowsInterop.Win.WM.NCMOUSEMOVE:
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
