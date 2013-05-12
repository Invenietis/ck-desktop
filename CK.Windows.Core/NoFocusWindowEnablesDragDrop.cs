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
    public class NoFocusWindowEnablesDragDrop : Window
    {
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;

        WindowInteropHelper _interopHelper;
        HwndSourceHook _wndHook;
        IntPtr _lastFocused;
        bool _ncbuttondown;

        public NoFocusWindowEnablesDragDrop()
        {
            _interopHelper = new WindowInteropHelper( this );
            this.Focusable = false;
        }

        protected override void OnSourceInitialized( EventArgs e )
        {
            CK.Windows.Interop.Win.Functions.SetWindowLong( _interopHelper.Handle, CK.Windows.Interop.Win.WindowLongIndex.GWL_EXSTYLE, (uint)CK.Windows.Interop.Win.WS_EX_NOACTIVATE );

            HwndSource mainWindowSrc = HwndSource.FromHwnd( _interopHelper.Handle );

            mainWindowSrc.CompositionTarget.BackgroundColor = Color.FromArgb( 0, 0, 0, 0 );
            mainWindowSrc.CompositionTarget.RenderMode = RenderMode.Default;

            if( OSVersionInfoTEMP.OSLevel >= OSVersionInfoTEMP.SimpleOSLevel.WindowsVista && CK.Windows.Interop.Dwm.Functions.IsCompositionEnabled() )
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
            Console.Out.WriteLine( "Mouse button down" );
            GetFocus();
            DragMove();
            base.OnMouseLeftButtonDown( e );
        }

        protected override void OnMouseLeftButtonUp( MouseButtonEventArgs e )
        {
            Console.Out.WriteLine( "Mouse button up" );
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
            switch( msg )
            {
                //case CK.Windows.Interop.Win.WM.ACTIVATE:
                //case CK.Windows.Interop.Win.WM.NCACTIVATE:
                //    if( !_ncbuttondown )
                //    {
                //        handled = true;
                //        //ReleaseFocus();
                //        Console.Out.WriteLine( "ACTIVATE triggered, but focus released" );
                //    }
                //    else
                //    {
                //        _lastFocused = wParam;
                //        Console.Out.WriteLine( "ACTIVATE triggered, got focus" );
                //    }

                //    break;
                case CK.Windows.Interop.Win.WM_SETFOCUS:
                    //if( !_ncbuttondown )
                    //{
                    //    handled = true;
                    //    //ReleaseFocus();
                    //    Console.Out.WriteLine( "Set focus triggered, but focus released" );
                    //}
                    //else
                    //{
                        _lastFocused = wParam;
                        Console.Out.WriteLine( "Set focus triggered, got focus" );
                    //}
                    
                    break;
                case CK.Windows.Interop.Win.WM_NCLBUTTONDOWN:
                    _ncbuttondown = true;
                    GetFocus();
                    Console.Out.WriteLine( "ButtonDown, Getting the focus" );
                    break;
                case CK.Windows.Interop.Win.WM_NCMOUSEMOVE:
                    if( _ncbuttondown )
                    {
                        ReleaseFocus();
                        _ncbuttondown = false;
                        Console.Out.WriteLine( "MouseMove, releasing the focus" );
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
}
