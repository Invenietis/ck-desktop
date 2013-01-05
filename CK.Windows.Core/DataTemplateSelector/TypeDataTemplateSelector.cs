#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Core\DataTemplateSelector\TypeDataTemplateSelector.cs) is part of CiviKey. 
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
using System.ComponentModel;
using System.Windows;

namespace CK.Windows
{
    /// <summary>
    /// This is a <see cref="TemplatedDataTemplateSelector"/> that matches the <see cref="P:Type"/> of the object
    /// for which a data template must be selected.
    /// </summary>
    public class TypeDataTemplateSelector : TemplatedDataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the type of the object that must match.
        /// </summary>
        public Type Type { get; set; }

        /// <summary>
        /// Gets or sets whether the match must be strict (i.e. <see cref="Type.IsAssignableFrom"/>), or
        /// must use more relaxed <see cref="CK.Reflection.ReflectionHelper.CovariantMatch">covariance rules</see>.
        /// Defaults to false.
        /// </summary>
        [DefaultValue( false )]
        public bool ExactTypeMatch { get; set; }

        protected override bool Match( object item, DependencyObject container )
        {
            bool success = false;
            if( Type != null )
            {
                Type itemType = item.GetType();
                if( ExactTypeMatch )
                {
                    success = Type.IsAssignableFrom( itemType );
                    //CompositeDataTemplateSelector.Log.Debug( log => log( "Exact type {0} match: {1} for type {2}.", Type.Name, success, itemType ) );
                }
                else
                {
                    success = CK.Reflection.ReflectionHelper.CovariantMatch( Type, itemType );
                    //CompositeDataTemplateSelector.Log.Debug( log => log( "Covariant type {0} match: {1} for type {2}.", Type.Name, success, itemType ) );
                }
            }
            else CompositeDataTemplateSelector.Log.Debug( log => log( "Unitialized rule (no Type nor TypeDescriptor set). Rule failed." ) );
            return success;
        }
    }

}
