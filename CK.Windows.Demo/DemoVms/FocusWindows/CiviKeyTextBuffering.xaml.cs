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
        IntPtr _targetHWnd;
        bool _isExternalWindow;

        public CiviKeyTextBuffering()
        {
            InitializeComponent();
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
                SendText.Text = "(no active target: it will be the foreground window)";
            }
            else
            {
                if( isExternalWindow )
                {
                    SendText.Text = String.Format( "Send to external HWnd = 0x{0:X}", hWnd.ToInt32() );
                }
                else 
                {
                    HwndSource s = HwndSource.FromHwnd( hWnd );
                    if( s == null )
                    {
                        SendText.Text = String.Format( "Internal Target but unable to get the WPF window for it. HWnd = 0x{0:X}", hWnd.ToInt32() );
                    }
                    else
                    {
                        Window w = s.RootVisual as Window;
                        if( w == null )
                        {
                            SendText.Text = String.Format( "This should never happen: the internal target is not a WPF window. HWnd = 0x{0:X}", hWnd.ToInt32() );
                        }
                        else
                        {
                            SendText.Text = String.Format( "Send to internal window '{0}' (HWnd = 0x{1:X})", w.ToString(), hWnd.ToInt32() );
                        }
                    }
                }
            }
        }

        private void SendClick( object sender, RoutedEventArgs e )
        {
            if( _targetHWnd == IntPtr.Zero )
            {
                IntPtr w = Win.Functions.GetForegroundWindow();
                uint processId;
                Win.Functions.GetWindowThreadProcessId( w, out processId );
                if( processId != System.Diagnostics.Process.GetCurrentProcess().Id )
                {
                    _targetHWnd = w;
                }
            }
            if( _targetHWnd != IntPtr.Zero )
            {
                Win.Functions.SetForegroundWindow( _targetHWnd );
                PseudoSendStringService.SendString( WorkingBuffer.Text );
            }
        }
    }
}
