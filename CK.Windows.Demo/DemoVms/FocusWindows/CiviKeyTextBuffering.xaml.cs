using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using CK.Windows.Interop;

namespace CK.Windows.Demo.DemoVms
{
    /// <summary>
    /// Interaction logic for CiviKeyTextBuffering.xaml
    /// </summary>
    public partial class CiviKeyTextBuffering : CKWindow
    {

        public CiviKeyTextBuffering()
        {
            InitializeComponent();
        }

        protected override bool IsDraggableVisual( DependencyObject visualElement )
        {
            return base.IsDraggableVisual( visualElement ) || visualElement == Explanation;
        }

        private void SendClick( object sender, RoutedEventArgs e )
        {
            // Obtains the targetHWnd (the last that was activated).
            // 
            // Win.Functions.SetForegroundWindow( targetHWnd );
            // PseudoSendStringService.SendString( WorkingBuffer.Text );
        }
    }
}
