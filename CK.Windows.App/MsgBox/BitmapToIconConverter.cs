#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.App\MsgBox\BitmapToIconConverter.cs) is part of CiviKey. 
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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace CK.Windows.App
{
    public class IconToBitmapConverter : IValueConverter
    {
        public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
        {
            Icon icon = null;

            if( value.GetType() == typeof( CustomMsgBoxIcon ) )
            {
                CustomMsgBoxIcon msgBoxIcon;
                if( Enum.TryParse<CustomMsgBoxIcon>( value.ToString(), out msgBoxIcon ) )
                {
                    icon = GetMessageBoxIcon( msgBoxIcon );
                }
            }

            if( value.GetType() == typeof( Icon ) )
            {
                icon = value as Icon;
            }

            if( icon != null )
            {
                using( MemoryStream iconStream = new MemoryStream() )
                {
                    ImageSource imageSource = Imaging.CreateBitmapSourceFromHIcon(
                    icon.Handle,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions() );

                    return imageSource;
                }
            }

            return null;
        }

        public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Get system icon for MessageBoxIcon.
        /// </summary>
        /// <param name="icon">The MessageBoxIcon value.</param>
        /// <returns>SystemIcon type Icon.</returns>
        public static Icon GetMessageBoxIcon( CustomMsgBoxIcon icon )
        {
            switch( icon )
            {
                case CustomMsgBoxIcon.Information:
                    return SystemIcons.Information;
                case CustomMsgBoxIcon.Warning:
                    return SystemIcons.Warning;
                case CustomMsgBoxIcon.Error:
                    return SystemIcons.Error;
                case CustomMsgBoxIcon.Question:
                    return SystemIcons.Question;
                default:
                    return null;
            }
        }
    }

    // Summary:
    //     Specifies constants defining which information to display.
    public enum CustomMsgBoxIcon
    {
        // Summary:
        //     The message box contain no symbols.
        None = 0,
        //
        // Summary:
        //     The message box contains a symbol consisting of white X in a circle with
        //     a red background.
        Error = 16,
        //
        // Summary:
        //     The message box contains a symbol consisting of a question mark in a circle.
        //     The question-mark message icon is no longer recommended because it does not
        //     clearly represent a specific type of message and because the phrasing of
        //     a message as a question could apply to any message type. In addition, users
        //     can confuse the message symbol question mark with Help information. Therefore,
        //     do not use this question mark message symbol in your message boxes. The system
        //     continues to support its inclusion only for backward compatibility.
        Question = 32,
        //
        // Summary:
        //     The message box contains a symbol consisting of an exclamation point in a
        //     triangle with a yellow background.
        Warning = 48,
        //
        // Summary:
        //     The message box contains a symbol consisting of a lowercase letter i in a
        //     circle.
        Information = 64
    }
}
