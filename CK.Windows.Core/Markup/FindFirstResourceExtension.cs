using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Markup;
using System.ComponentModel;
using System.Windows;
using System.Xml;

namespace CK.Windows
{
    /// <summary>
    /// This markup extension works contrary to the default StaticRessourceExtension : the first occurrence of a ressource key will hide all others.
    /// </summary>
    public class FindFirstResourceExtension : MarkupExtension
    {
        object _resourceKey;

        public FindFirstResourceExtension( string resourceKey )
        {
            _resourceKey = resourceKey;
        }

        public override object ProvideValue( IServiceProvider serviceProvider )
        {
            var value = FindResource( Application.Current.Resources, _resourceKey );
            return value;
        }

        /// <summary>
        /// Recursively look into dictionaries to find the first occurrence of the resource key
        /// </summary>
        /// <param name="dictionary"></param>
        /// <param name="resourceKey"></param>
        /// <returns></returns>
        object FindResource( ResourceDictionary dictionary, object resourceKey )
        {
            var value = dictionary[resourceKey];
            if( value == null )
            {
                foreach( var merged in dictionary.MergedDictionaries )
                {
                    var val = FindResource( merged, resourceKey );
                    if( val != null ) return val;
                }
            }
            return value;
        }

        public object ResourceKey
        {
            get { return _resourceKey; }
            set { _resourceKey = value; }
        }
    }
}
