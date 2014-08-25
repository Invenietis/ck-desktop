#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Core\Windows\IHitTestElementController.cs) is part of CiviKey. 
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
* Copyright © 2007-2014, 
*     Invenietis <http://www.invenietis.com>,
*     In’Tech INFO <http://www.intechinfo.fr>,
* All rights reserved. 
*-----------------------------------------------------------------------------*/
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace CK.Windows.Core
{
    public interface IHitTestElementController
    {
        /// <summary>
        /// Allows changing the current HTCode.
        /// </summary>
        /// <param name="p">The point used for HitTest</param>
        /// <param name="HTCode">The current HTCode</param>
        /// <param name="hitObject">The element currently hit</param>
        /// <returns></returns>
        int GetHitTestResult( Point p, int HTCode, DependencyObject hitObject );
    }
}
