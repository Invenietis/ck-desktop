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
    public partial class CiviKeyTextBuffering : CiviKeyWindow
    {
        IntPtr _targetHWnd;
        bool _isExternalWindow;

        public CiviKeyTextBuffering()
        {
            InitializeComponent();
            Send.IsEnabled = false;
        }

        protected override bool IsDraggableVisual( DependencyObject visualElement )
        {
            return base.IsDraggableVisual( visualElement ) || visualElement == Explanation;
        }

        protected override void SetActiveTarget( IntPtr hWnd, bool isExternalWindow )
        {
            _targetHWnd = hWnd;
            _isExternalWindow = isExternalWindow;
            if( _targetHWnd == IntPtr.Zero )
            {
                Send.IsEnabled = false;
                SendText.Text = "(no active target)";
            }
            else
            {
                if( isExternalWindow )
                {
                    SendText.Text = String.Format( "Send to external HWnd = 0x{0:X}", hWnd.ToInt32() );
                    SendText.IsEnabled = true;
                }
                else 
                {
                    HwndSource s = HwndSource.FromHwnd( hWnd );
                    if( s == null )
                    {
                        SendText.Text = String.Format( "Internal Target but unable to get the WPF window for it. HWnd = 0x{0:X}", hWnd.ToInt32() );
                        Send.IsEnabled = false;
                    }
                    else
                    {
                        Window w = s.RootVisual as Window;
                        if( w == null )
                        {
                            SendText.Text = String.Format( "This should never happen: the internal target is not a WPF window. HWnd = 0x{0:X}", hWnd.ToInt32() );
                            Send.IsEnabled = false;
                        }
                        else
                        {
                            SendText.Text = String.Format( "Send to internal window '{0}' (HWnd = 0x{1:X})", w.ToString(), hWnd.ToInt32() );
                            Send.IsEnabled = true;
                        }
                    }
                }
            }
        }

        private void SendClick( object sender, RoutedEventArgs e )
        {
            Win.Functions.SetForegroundWindow( _targetHWnd );
            PseudoSendStringService.SendString( WorkingBuffer.Text );
        }
    }
}
