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
    public partial class CKWindow
    {
        IntPtr WndProcXP( IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled )
        {
            XPDriver driver = (XPDriver)_driver;
            return IntPtr.Zero;
        }

        class XPDriver : OSDriver
        {
            internal XPDriver( CKWindow w, HwndSource wSource )
                : base( w )
            {
                wSource.AddHook( w.WndProcXP );
            }
        }
    }
}
