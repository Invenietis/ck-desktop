using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace CK.Windows.Demo.DemoVms
{
    /// <summary>
    /// Interaction logic for NonActivableCiviKeyWindow.xaml
    /// </summary>
    public partial class NonActivableCiviKeyWindow : CKNoFocusWindow
    {
        public NonActivableCiviKeyWindow()
        {
            InitializeComponent();
        }

        protected override bool IsDraggableVisual( DependencyObject visualElement )
        {
            if( visualElement == DraggableImage || visualElement == Explanation ) return true;
            if( visualElement is Border )
            {
                return VisualTreeHelper.GetParent( visualElement ) == this;
            }
            return false;
        }

        private void ClickedKey( object sender, RoutedEventArgs e )
        {
            PseudoSendStringService.SendString( ((Button)e.Source).Name );
        }

        private void ThrowException( object sender, RoutedEventArgs e )
        {
            throw new Exception( "Test Exception" );
        }
    }
}
