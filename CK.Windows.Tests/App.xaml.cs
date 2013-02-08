using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;

namespace CK.Windows.Tests
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        PresentationSource _menuSite;
        Popup w2;
        Window  w3;

        private static CustomPopupPlacement[] GetPopupPlacement( Size popupSize, Size targetSize, Point offset )
        {
            var point = SystemParameters.WorkArea.BottomRight;
            point.Y = point.Y - popupSize.Height;
            return new[] { new CustomPopupPlacement( point, PopupPrimaryAxis.Horizontal ) };
        }

        protected override void OnStartup( StartupEventArgs e )
        {
            base.OnStartup( e );
            DockPanel p = new DockPanel();
            HwndSource.DefaultAcquireHwndFocusInMenuMode = false;
            Keyboard.DefaultRestoreFocusMode = RestoreFocusMode.None;
            
            ToolBar tb = new ToolBar();
            Button bt = new Button() { Content = "Clic 2" };
            tb.Items.Add( new Button() { Content = "Clic" } );

            p.Children.Add( tb );
            p.Children.Add( bt );
            p.Children.Add( new TextBox() { Text = "abc azeaze aze aze" } );

            //w2 = new Popup();
            //w2.Child = p;
            //w2.Width = 200;
            //w2.Height = 100;
            ////w2.Topmost = true;
            //w2.StaysOpen = true;
            //w2.Focusable = false;
            //w2.CustomPopupPlacementCallback = GetPopupPlacement;
            //w2.Placement = PlacementMode.Absolute;
            //HeaderedItemsControl
            ////w2.Activated += w2_Activated;
            ////bt.PreviewGotKeyboardFocus += w2_PreviewGotKeyboardFocus;
            ////tb.PreviewGotKeyboardFocus += w2_PreviewGotKeyboardFocus;
            ////bt.PreviewLostKeyboardFocus += w2_PreviewLostKeyboardFocus;
            ////tb.PreviewLostKeyboardFocus += w2_PreviewLostKeyboardFocus;
            //w2.IsOpen = true;
            
            w3 = new Window();
            w3.Title = "FOCUS WINDOW";
            w3.Content = new TextBox() { Text = "abc" };
            w3.Width = 200;
            w3.Height = 100;
            w3.Activated += w2_Activated;
            w3.Show();
        }

        void w2_PreviewLostKeyboardFocus( object sender, KeyboardFocusChangedEventArgs e )
        {
            if( _menuSite != null )
            {
                InputManager.Current.PopMenuMode( _menuSite );
                _menuSite = null;
            }
        }


        void w2_PreviewGotKeyboardFocus( object sender, KeyboardFocusChangedEventArgs e )
        {
            _menuSite = HwndSource.FromVisual( w2.Child );
            InputManager.Current.PushMenuMode( _menuSite );
        }

        void w2_Activated( object sender, EventArgs e )
        {
            Debug.Write( "activated" );
        }

    }
}
