#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Core\DataTemplateSelector\CompositeDataTemplateSelector.cs) is part of CiviKey. 
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

using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System;

namespace CK.Windows
{

    /// <summary>
    /// This specialized <see cref="DataTemplateSelector"/> implements a simple first-match rule mechanism
    /// to select the <see cref="DataTemplate"/> that must be applied to an object.
    /// </summary>
    [ContentProperty( "Selectors" )]
    public class CompositeDataTemplateSelector : DataTemplateSelector
    {
        static Common.Logging.ILog _log;
        static internal Common.Logging.ILog Log
        {
            get
            {
                return _log ?? (_log = Common.Logging.LogManager.GetLogger<CompositeDataTemplateSelector>());
            }
        }

        public CompositeDataTemplateSelector()
        {
            Selectors = new List<DataTemplateSelector>();
        }

        /// <summary>
        /// Gets the CompositeDataTemplateSelector on which we have to look for selectors if we don't find a good one in our <see pref="Selectors"/>.
        /// </summary>
        public CompositeDataTemplateSelector Fallback { get; set; }

        /// <summary>
        /// Gets the list of <see cref="DataTemplateSelector"/> that this composite contains.
        /// </summary>
        public List<DataTemplateSelector> Selectors { get; private set; }

        /// <summary>
        /// Implements the template selection by finding the first selector among <see cref="Selectors"/> that returns a non null <see cref="DataTemplate"/>.
        /// </summary>
        /// <param name="item">The object.</param>
        /// <param name="container"></param>
        /// <returns>A <see cref="DataTemplate"/> for the <paramref name="item"/>.</returns>
        public override DataTemplate SelectTemplate( object item, DependencyObject container )
        {
            DataTemplate result = null;
            foreach( DataTemplateSelector s in Selectors )
            {
                if( (result = s.SelectTemplate( item, container )) != null ) break;
            }
            if( result == null && Fallback != null )
                result = Fallback.SelectTemplate( item, container );
            return result;
        }
    }
}
