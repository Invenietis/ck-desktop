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
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using CK.Core;
using CK.Windows.Interop;

namespace CK.Windows
{
    public partial class CKWindow
    {
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;


        IntPtr WndProcWinNoFocusDefault( IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled )
        {
            WinDefaultDriver driver = (WinDefaultDriver)_driver;
            switch( msg )
            {
                case Win.WM_NCHITTEST:
                    {
                        int hit = Win.Functions.DefWindowProc( _hwnd, msg, wParam, lParam ).ToInt32();
                        if( hit == Win.HTCLIENT )
                        {
                            CKNCHitTest( PointFromLParam( lParam ), ref hit );
                        }
                        handled = true;
                        return new IntPtr( hit );
                    }
                case Win.WM_DWMCOMPOSITIONCHANGED:
                    {
                        driver.TryExtendFrame();
                        return IntPtr.Zero;
                    }
                case CK.Windows.Interop.Win.WM_NCLBUTTONDOWN:
                    {
                        _ncbuttondown = true;
                        GetFocus();
                        return hWnd;
                    }
                case CK.Windows.Interop.Win.WM_NCMOUSEMOVE:
                    {
                        if( _ncbuttondown )
                        {
                            ReleaseFocus();
                            _ncbuttondown = false;
                        }
                        return hWnd;
                    }
            }
            return IntPtr.Zero;
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

        protected override void OnStateChanged( EventArgs e )
        {
            if( WindowState == System.Windows.WindowState.Maximized )
            {
                WindowState = System.Windows.WindowState.Normal;
                ReleaseFocus();
            }
        }

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


        class WinDefaultDriver : OSDriver
        {
            bool _isExtendedFrame;

            internal WinDefaultDriver( CKWindow w, HwndSource wSource )
                : base( w )
            {
                wSource.AddHook( w.WndProcWinNoFocusDefault );
                TryExtendFrame();
            }

            public void TryExtendFrame()
            {
                try
                {
                    _isExtendedFrame = Dwm.Functions.IsCompositionEnabled();
                    if( _isExtendedFrame )
                    {
                        // Negative margins have special meaning to DwmExtendFrameIntoClientArea.
                        // Negative margins create the "sheet of glass" effect, where the client 
                        // area is rendered as a solid surface without a window border.
                        Win.Margins m = new CK.Windows.Interop.Win.Margins() { LeftWidth = -1, RightWidth = -1, TopHeight = -1, BottomHeight = -1 };
                        Dwm.Functions.ExtendFrameIntoClientArea( W.ThisWindowHandle, ref m );

                        W.Background = Brushes.Transparent;
                        HwndSource.FromHwnd( W.ThisWindowHandle ).CompositionTarget.BackgroundColor = Colors.Transparent;
                    }
                }
                catch
                {
                    _isExtendedFrame = false;
                    W.Background = new SolidColorBrush( Colors.WhiteSmoke );
                }
                WinTrace( W, _isExtendedFrame ? "Frame extended" : "Frame NOT extended" );
            }

        }
    }
}
