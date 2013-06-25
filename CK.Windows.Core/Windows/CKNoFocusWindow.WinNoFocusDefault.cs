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
using CK.Windows.Core;
using CK.Windows.Interop;

namespace CK.Windows
{
    public partial class CKNoFocusWindow
    {
        IntPtr WndProcWinNoFocusDefault( IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled )
        {
            //WinDefaultDriver driver = (WinDefaultDriver)_driver;
            switch( msg )
            {
                case Win.WM_NCHITTEST:
                    {
                        int hit = Win.Functions.DefWindowProc( Hwnd, msg, wParam, lParam ).ToInt32();
                        if( hit == Win.HTCLIENT )
                        {
                            CKNCHitTest( PointFromLParam( lParam ), ref hit );
                        }
                        handled = true;
                        return new IntPtr( hit );
                    }
                case Win.WM_DWMCOMPOSITIONCHANGED:
                    {
                        this.IsFrameExtended = CKWindowTools.TryExtendFrame( this );
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

            if(e.LeftButton == MouseButtonState.Pressed)
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
            CK.Windows.Interop.Win.Functions.SetForegroundWindow( Hwnd );
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

        class WinDefaultDriver : OSDriver
        {
            internal WinDefaultDriver( CKNoFocusWindow w, HwndSource wSource )
                : base( w )
            {
                wSource.AddHook( w.WndProcWinNoFocusDefault );
                w.IsFrameExtended = CKWindowTools.TryExtendFrame( w );
            }
        }
    }
}
