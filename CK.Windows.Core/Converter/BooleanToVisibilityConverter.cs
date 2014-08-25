#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Core\Converter\BooleanToVisibilityConverter.cs) is part of CiviKey. 
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
using System.Windows.Data;
using System.Windows;

namespace CK.Windows
{
    [ValueConversion( typeof( bool ), typeof( Visibility ) )]
    public class BooleanToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Internal arsed data structure.
        /// </summary>
        struct Parameter
        {
            public bool Invert;
            public Visibility NotVisible;
        }

        /// <summary>
        /// Converts a boolean value to a Visibility value.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter">
        /// ConverterParameter is of type Visibility
        /// </param><param name="culture"></param>
        /// <returns></returns>
        public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
        {
            Parameter p = ParseParameter( parameter );
            bool isVisible = (bool)value;
            if( p.Invert ) isVisible = !isVisible;
            return isVisible ? Visibility.Visible : p.NotVisible;
        }


        /// <summary>
        /// Supports 2-way databinding of the BooleanToVisibilityConverter, converting Visibility to a boolean.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="targetType"></param>
        /// <param name="parameter"></param>
        /// <param name="culture"></param>
        /// <returns></returns>
        public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
        {
            Parameter p = ParseParameter( parameter );
            bool isVisible = ((Visibility)value == Visibility.Visible);
            if( p.Invert ) isVisible = !isVisible;
            return isVisible;
        }

        /// <summary>   
        /// Parses 'Invert', 'Hidden' or 'Invert,Hidden' into <see cref="Parameter"/>.
        /// </summary>   
        /// <param name="parameter">Parameter string.</param>   
        /// <returns>Parsed structure.</returns>   
        static Parameter ParseParameter( object parameter )
        {
            if( parameter is Parameter ) return (Parameter)parameter;
            switch( (string)parameter )
            {
                case null:
                case "": return new Parameter() { Invert = false, NotVisible = Visibility.Collapsed };
                case "Invert": return new Parameter() { Invert = true, NotVisible = Visibility.Collapsed };
                case "Hidden": return new Parameter() { Invert = false, NotVisible = Visibility.Hidden };
                case "Invert,Hidden": return new Parameter() { Invert = true, NotVisible = Visibility.Hidden };
            }
            throw new FormatException( "Invalid mode specified as the ConverterParameter. It can be empty (defaults to Collapsed), 'Invert', 'Hidden' or 'Invert,Hidden'." );
        }
    }
}
