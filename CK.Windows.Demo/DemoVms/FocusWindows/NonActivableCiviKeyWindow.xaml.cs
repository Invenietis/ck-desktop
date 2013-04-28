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
    public partial class NonActivableCiviKeyWindow : CiviKeyWindow
    {
        public NonActivableCiviKeyWindow()
        {
            InitializeComponent();
        }

        protected override bool IsDraggableVisual( DependencyObject visualElement )
        {
            return base.IsDraggableVisual( visualElement ) || visualElement == DraggableImage || visualElement == Explanation;
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
