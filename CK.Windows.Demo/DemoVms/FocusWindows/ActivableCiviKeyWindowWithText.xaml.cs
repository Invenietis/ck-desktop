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
    /// Interaction logic for TextBuffering.xaml
    /// </summary>
    public partial class ActivableCiviKeyWindowWithText : CKWindow
    {
        public ActivableCiviKeyWindowWithText()
        {
            InitializeComponent();
        }

        protected override bool IsDraggableVisual( DependencyObject visualElement )
        {
            return base.IsDraggableVisual( visualElement ) || visualElement == Explanation;
        }
    }
}
