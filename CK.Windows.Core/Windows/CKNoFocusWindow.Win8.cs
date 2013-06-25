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
    public partial class CKNoFocusWindow
    {

        class Win8Driver : OSDriver
        {
            internal Win8Driver( CKNoFocusWindow w, HwndSource wSource )
                : base( w )
            {
                wSource.AddHook( WndProc );
            }

            IntPtr WndProc( IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled )
            {
                switch( msg )
                {
                    case Win.WM_NCHITTEST:
                        {
                            int hit = Win.Functions.DefWindowProc( Window.Hwnd, msg, wParam, lParam ).ToInt32();
                            if( hit == Win.HTCLIENT )
                            {
                                Window.CKNCHitTest( Window.PointFromLParam( lParam ), ref hit );
                            }
                            handled = true;
                            return new IntPtr( hit );
                        }
                }
                return IntPtr.Zero;
            }
        }

    }
}
