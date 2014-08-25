#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Demo\DemoVms\FocusWindows\ActivableCiviKeyWindowWithText.xaml.cs) is part of CiviKey. 
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
    /// Interaction logic for TextBuffering.xaml
    /// </summary>
    public partial class ActivableCiviKeyWindowWithText : CKNoFocusWindow
    {
        public ActivableCiviKeyWindowWithText( NoFocusManager noFocusManager )
            : base( noFocusManager )
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
