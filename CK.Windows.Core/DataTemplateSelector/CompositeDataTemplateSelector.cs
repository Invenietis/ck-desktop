using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;

namespace CK.Windows
{

    /// <summary>
    /// This specialized <see cref="DataTemplateSelector"/> implements a simple first-match rule mechanism
    /// to select the <see cref="DataTemplate"/> that must be applied to an object.
    /// </summary>
    [ContentProperty( "Selectors" )]
    public class CompositeDataTemplateSelector : DataTemplateSelector
    {
        static readonly internal Common.Logging.ILog Log = Common.Logging.LogManager.GetLogger<CompositeDataTemplateSelector>();

        public CompositeDataTemplateSelector()
        {
            Selectors = new List<DataTemplateSelector>();
        }

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
            return result;
        }
    }
}
