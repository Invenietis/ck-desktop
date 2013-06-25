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
    public partial class ActivableCiviKeyWindowWithText : CKNoFocusWindow
    {
        public ActivableCiviKeyWindowWithText()
        {
            InitializeComponent();
        }

        protected override bool IsDraggableVisual( DependencyObject visualElement )
        {
            return visualElement == Explanation || ( visualElement is Border && VisualTreeHelper.GetParent( visualElement ) == this );
        }

        protected override void OnActivated( EventArgs e )
        {
            Console.WriteLine( "ActivableCiviKeyWindowWithText.OnActivated (hWnd=0x{0:X})", Hwnd );
            base.OnActivated( e );
        }

        protected override void OnDeactivated( EventArgs e )
        {
            Console.WriteLine( "ActivableCiviKeyWindowWithText.OnDeactivated (hWnd=0x{0:X})", Hwnd );
            base.OnDeactivated( e );
        }
    }
}
