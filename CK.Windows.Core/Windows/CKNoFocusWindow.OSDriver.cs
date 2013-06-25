#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Core\CKWindow.Central.cs) is part of CiviKey. 
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

using System.Windows.Interop;
using CK.Core;

namespace CK.Windows
{
    public partial class CKNoFocusWindow
    {
        abstract class OSDriver
        {
            protected readonly CKNoFocusWindow Window;

            protected OSDriver( CKNoFocusWindow w )
            {
                Window = w;
            }

            public static OSDriver Create( CKNoFocusWindow w, HwndSource wSource, OSVersionInfoTEMP.SimpleOSLevel osLevel = OSVersionInfoTEMP.SimpleOSLevel.Unknown )
            {
                if( osLevel == OSVersionInfoTEMP.SimpleOSLevel.Unknown ) osLevel = OSVersionInfoTEMP.OSLevel;

                //if( osLevel >= OSVersionInfoTEMP.SimpleOSLevel.Windows8 )
                //{
                //    return new Win8Driver( w, wSource );
                //}
                
                return new WinDefaultDriver( w, wSource );
            }
        }
    }
}
