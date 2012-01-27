using System;
using System.Windows;
using System.Windows.Interop;
using CK.Core;
using System.Windows.Media;
using CK.Interop;
using System.Windows.Input;

namespace CK.Windows
{
    public class NoFocusWindow : Window
    {
        WindowInteropHelper _interopHelper;
        HwndSourceHook _wndHook;
        IntPtr _lastFocused;
        bool _ncbuttondown;

        public NoFocusWindow()
        {
        }

        protected override void OnSourceInitialized( EventArgs e )
        {
            _interopHelper = new WindowInteropHelper( this );

            Win.Functions.SetWindowLong( _interopHelper.Handle, Win.WindowLongIndex.GWL_EXSTYLE, (uint)Win.WS_EX.NOACTIVATE );

            HwndSource mainWindowSrc = HwndSource.FromHwnd( _interopHelper.Handle );

            mainWindowSrc.CompositionTarget.BackgroundColor = Color.FromArgb( 0, 0, 0, 0 );
            mainWindowSrc.CompositionTarget.RenderMode = RenderMode.Default;

            if( OSVersionInfo.IsWindowsVistaOrGreater && Dwm.Functions.IsCompositionEnabled() )
            {
                Win.Margins m = new Win.Margins() { LeftWidth = -1, RightWidth = -1, TopHeight = -1, BottomHeight = -1 };
                Dwm.Functions.ExtendFrameIntoClientArea( _interopHelper.Handle, ref m );
            }
            else
            {

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
            _lastFocused = Win.Functions.GetForegroundWindow();
            Win.Functions.SetForegroundWindow( _interopHelper.Handle );
        }

        void ReleaseFocus()
        {
            Win.Functions.SetForegroundWindow( _lastFocused );
        }


        IntPtr WndProc( IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled )
        {
            switch( (Win.WM)msg )
            {
                case Win.WM.NCLBUTTONDOWN:
                    _ncbuttondown = true;
                    GetFocus();
                    break;
                case Win.WM.NCMOUSEMOVE:
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
    }
}
