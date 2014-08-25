#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Core\DataTemplateSelector\TemplatedDataTemplateSelector.cs) is part of CiviKey. 
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
using System.Windows.Controls;
using System.Windows;
using System.Windows.Markup;
using System.ComponentModel;
using System.Collections.ObjectModel;
using CK.Core;
using System.ComponentModel.Design.Serialization;
using System.Security;
using System.Globalization;

namespace CK.Windows
{
    /// <summary>
    /// A <see cref="DataTemplateSelector"/> with an actual <see cref="DataTemplate"/> instance expressed as a XAML content property.
    /// By default, this child <see cref="Template"/> is always selected by <see cref="SelectTemplate"/>, but the goal of this class
    /// is to be used as a base class: the virtual <see cref="Match"/> method must be overriden.
    /// </summary>
    [ContentProperty( "Template" )]
    public class TemplatedDataTemplateSelector : DataTemplateSelector
    {
        /// <summary>
        /// Gets or sets the actual <see cref="DataTemplate"/> that must be selected.
        /// </summary>
        public DataTemplate Template { get; set; }

        /// <summary>
        /// Overriden to return <see cref="Template"/> if <see cref="Match"/> returns true.
        /// </summary>
        /// <param name="item">The data object for which to select the template.</param>
        /// <param name="container">The data-bound object.</param>
        /// <returns>Returns <see cref="Template"/> or null. The default value is <see cref="Template"/> since this <see cref="Match"/> returns true.</returns>
        public override sealed  DataTemplate SelectTemplate( object item, DependencyObject container )
        {
            return Match( item, container ) ? Template : null; 
        }

        /// <summary>
        /// Template method to be overriden.
        /// </summary>
        /// <param name="item">The data object for which to select the template.</param>
        /// <param name="container">The data-bound object.</param>
        /// <returns>True if the data object matches any constraint that this selector may implement.</returns>
        protected virtual bool Match( object item, DependencyObject container )
        {
            CompositeDataTemplateSelector.Log.Debug( log => log( "TemplatedDataTemplateSelector default match for {0}.", item ) );
            return true;
        }
    }
}
