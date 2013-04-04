using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace CK.Windows
{
    public class NullImageConverter : IValueConverter
    {
        public object Convert( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
        {
            if( value == null )
                return Binding.DoNothing;
            return value;
        }

        public object ConvertBack( object value, Type targetType, object parameter, System.Globalization.CultureInfo culture )
        {
            throw new NotImplementedException();
        }
    }
}
