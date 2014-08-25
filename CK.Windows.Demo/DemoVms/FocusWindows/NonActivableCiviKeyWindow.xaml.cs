#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Demo\DemoVms\FocusWindows\NonActivableCiviKeyWindow.xaml.cs) is part of CiviKey. 
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
* Copyright © 2007-2014, 
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

namespace CK.Windows.Demo.DemoVms
{
    /// <summary>
    /// Interaction logic for NonActivableCiviKeyWindow.xaml
    /// </summary>
    public partial class NonActivableCiviKeyWindow : CKNoFocusWindow
    {
        public NonActivableCiviKeyWindow( NoFocusManager noFocusManager )
            : base( noFocusManager )
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
