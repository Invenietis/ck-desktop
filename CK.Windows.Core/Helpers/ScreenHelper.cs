#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Core\Helpers\ScreenHelper.cs) is part of CiviKey. 
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

using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Interop;
using System;
using System.Windows;

namespace CK.Windows.Helpers
{
    /// <summary>
    /// Class holding helpers. Checking if an object is in ou out of the computer screens, getting the primary screen etc..
    /// </summary>
    public class ScreenHelper
    {
        /// <summary>
        /// Gets whether a rectangle is in one of the screens of the computer
        /// </summary>
        /// <param name="rect">The rectangle</param>
        /// <returns>true if the rectangle is in one of the screens of the computer</returns>
        public static bool IsInScreen( Rectangle rect )
        {
            return Screen.AllScreens.Any( ( s ) => s.WorkingArea.Contains( rect ) );
        }

        /// <summary>
        /// Gets whether a point is in one of the screens of the computer
        /// </summary>
        /// <param name="point">The point</param>
        /// <returns>true if the point is in one of the screens of the computer</returns>
        public static bool IsInScreen( System.Drawing.Point point )
        {
            return Screen.AllScreens.Any( ( s ) => s.WorkingArea.Contains( point ) );
        }

        /// <summary>
        /// Gets whether a window is in one of the screens of the computer
        /// </summary>
        /// <param name="window">The window</param>
        /// <returns>true if the window is in one of the screens of the computer</returns>
        public static bool IsInScreen( Window window )
        {
            return Screen.AllScreens.Any( ( s ) => s.WorkingArea.Contains( new Rectangle( (int)window.Top, (int)window.Left, (int)window.Width, (int)window.Left ) ) );
        }

        /// <summary>
        /// Gets the point at the center of the screen in which is the Rectangle set as parameter
        /// </summary>
        /// <param name="rect">The rectangle</param>
        /// <returns></returns>
        public static System.Drawing.Point GetCenterOfParentScreen( Rectangle rect )
        {
            Screen parent = Screen.FromRectangle( rect );
            return new System.Drawing.Point( parent.WorkingArea.Width / 2, parent.WorkingArea.Height / 2 );
        }

        /// <summary>
        /// Gets the Rectangle of the primary screen
        /// </summary>
        /// <returns>The rectangle of the primary screen</returns>
        public static Rectangle GetPrimaryScreenSize()
        {
            return Screen.PrimaryScreen.Bounds;
        }

    }
}
