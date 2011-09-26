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

namespace CK.Windows.App
{
    /// <summary>
    /// Interaction logic for CrashLogWindow.xaml
    /// </summary>
    internal partial class CrashLogWindow : Window
    {
        public CrashLogWindow( CrashLogWindowViewModel dataContext )
        {
            DataContext = dataContext;
            InitializeComponent();
        }

        public CrashLogWindowViewModel Uploader
        {
            get { return (CrashLogWindowViewModel)DataContext; }
        }

        private void SendClick( object sender, RoutedEventArgs e )
        {
            Uploader.StartUpload();
        }

        private void DeleteClick( object sender, RoutedEventArgs e )
        {
            FrameworkElement d = ((DependencyObject)e.Source).FindParent<FrameworkElement>();
            if( d != null ) Uploader.DeleteFile( (System.IO.FileInfo)d.DataContext );
        }

        private void ViewClick( object sender, RoutedEventArgs e )
        {
            FrameworkElement d = ((DependencyObject)e.Source).FindParent<FrameworkElement>();
            if( d != null ) Uploader.ViewFile( (System.IO.FileInfo)d.DataContext );
        }

    }
}
