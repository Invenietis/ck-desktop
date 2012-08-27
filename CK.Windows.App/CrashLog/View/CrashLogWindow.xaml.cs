#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.App\CrashLog\View\CrashLogWindow.xaml.cs) is part of CiviKey. 
*  
* CiviKey is free software: you can redistribute it and/or modify 
* it under the terms of the GNU Lesser General Public License as published 
* by the Free Software Foundation, either version 3 of the License, or 
* (at your option) any later version. 
*  
* CiviKey is distributed in the hope that it will be useful, 
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
* GNU Lesser General Public License for more details. 
* You should have received a copy of the GNU Lesser General Public License 
* along with CiviKey.  If not, see <http://www.gnu.org/licenses/>. 
*  
* Copyright © 2007-2012, 
*     Invenietis <http://www.invenietis.com>,
*     In’Tech INFO <http://www.intechinfo.fr>,
* All rights reserved. 
*-----------------------------------------------------------------------------*/
#endregion

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
