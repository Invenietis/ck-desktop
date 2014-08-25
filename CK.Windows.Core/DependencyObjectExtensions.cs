#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Core\DependencyObjectExtensions.cs) is part of CiviKey. 
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
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace CK.Windows
{
    /// <summary>
    /// Exposes useful extension methods for <see cref="DependencyObject"/>.
    /// </summary>
    public static class DependencyObjectExtensions
    {
        /// <summary>
        /// Gets the closest parent that matches a predicate. 
        /// This method correctly handles ContentElement that are not Visual nor Visual3D. 
        /// Extracted from http://www.codeproject.com/KB/WPF/WpfElementTrees.aspx.
        /// See also http://blogs.msdn.com/b/mikehillberg/archive/2008/05/23/of-logical-and-visual-trees-in-wpf.aspx about
        /// lagical and visual elements.
        /// </summary>
        /// <param name="this">This dependency object.</param>
        /// <param name="finder">Predicate to select the parent.</param>
        /// <returns>The first <see cref="DependencyObject"/> that satisfies the predicate. Null otherwise.</returns>
        public static DependencyObject FindParent( this DependencyObject @this, Predicate<DependencyObject> finder )
        {
            for( ; ; )
            {
                if( @this is Visual || @this is Visual3D )
                {
                    @this = VisualTreeHelper.GetParent( @this );
                }
                else
                {
                    // If we're in Logical Land then we must walk 
                    // up the logical tree until we find a 
                    // Visual/Visual3D to get us back to Visual Land.
                    @this = LogicalTreeHelper.GetParent( @this );
                }
                if( @this == null || finder( @this ) ) return @this;
            }
        }

        /// <summary>
        /// Gets the closest parent object of type <typeparamref name="T"/>.
        /// </summary>
        /// <typeparam name="T">Type of parent to locate.</typeparam>
        /// <param name="this">This dependency object.</param>
        /// <returns>The first <see cref="DependencyObject"/> that is a <typeparamref name="T"/>. Null if no such parent exists.</returns>
        public static T FindParent<T>( this DependencyObject @this ) where T : DependencyObject
        {
            return (T)FindParent( @this, p => p is T );
        }

    }
}
