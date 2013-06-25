using CK.Windows.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;

namespace CK.Windows
{
    /// <summary>
    /// WPF Window that has the aero "glossy-effect".
    /// </summary>
    public class CKWindow : Window
    {
        /// <summary>
        /// Gets the Win32 window handle of this <see cref="Window"/> object.
        /// Available once <see cref="Window.SourceInitialized"/> has been raised.
        /// </summary>
        public IntPtr Hwnd { get; private set; }

        /// <summary>
        /// Gets whether this window has the Aero "glossy effect".
        /// Can be false if the desktop compoisiton has been disabled.
        /// </summary>
        protected bool IsFrameExtended { get; set; }

        private WindowInteropHelper InteropHelper { get; set; }

        /// <summary>
        /// Defautl constructor
        /// </summary>
        public CKWindow()
            : base()
        {
            InteropHelper = new WindowInteropHelper( this );
        }

        /// <summary>
        /// When the window is initialized, gets the current window's Handle.
        /// Then calls WPF Window OnSourceInitialized.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSourceInitialized( EventArgs e )
        {
            Hwnd = new WindowInteropHelper( this ).Handle;
            HwndSource hSource = HwndSource.FromHwnd( Hwnd );
            hSource.AddHook( WndProcWinDefault );

            IsFrameExtended = CKWindowTools.TryExtendFrame( this );

            base.OnSourceInitialized( e );
        }

        IntPtr WndProcWinDefault( IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled )
        {
            switch( msg )
            {
                case Win.WM_DWMCOMPOSITIONCHANGED:
                    {
                        IsFrameExtended = CKWindowTools.TryExtendFrame( this );
                        return IntPtr.Zero;
                    }
            }
            return IntPtr.Zero;
        }

        protected override void OnMouseLeftButtonDown( MouseButtonEventArgs e )
        {
            if( e.LeftButton == MouseButtonState.Pressed )
                DragMove();

            base.OnMouseLeftButtonDown( e );
        }
    }
}
