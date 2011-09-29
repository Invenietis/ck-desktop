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
                    CompositeDataTemplateSelector.Log.Value.Debug( log => log( "Exact type {0} match: {1} for type {2}.", Type.Name, success, itemType ) );
                }
                else
                {
                    success = CK.Reflection.ReflectionHelper.CovariantMatch( Type, itemType );
                    CompositeDataTemplateSelector.Log.Value.Debug( log => log( "Covariant type {0} match: {1} for type {2}.", Type.Name, success, itemType ) );
                }
            }
            else CompositeDataTemplateSelector.Log.Value.Debug( log => log( "Unitialized rule (no Type nor TypeDescriptor set). Rule failed." ) );
            return success;
        }
    }

}
