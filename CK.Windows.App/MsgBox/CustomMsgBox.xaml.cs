#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.App\MsgBox\CustomMsgBox.xaml.cs) is part of CiviKey. 
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
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CK.Windows.App
{
    /// <summary>
    /// This Messagebox enables you to display a messagebox without being restricted by the YesNo or OK buttons of the built-in message box.
    /// You can choose to display a checkbox (see the different constructors of the <see cref="ModalViewModel"/> )
    /// Retrieve the information after the call to <see cref="Window.ShowDialog()"/> in the <see cref="ModalViewModel"/> object.
    /// </summary>
    public partial class CustomMsgBox : Window
    {
        ModalViewModel _ctx;
        /// <summary>
        /// Constructor of a CustomMsgBox
        /// This Messagebox enables you to display a messagebox without being restricted by the YesNo or OK buttons of the built-in message box.
        /// You can choose to display a checkbox (see the different constructors of the <see cref="ModalViewModel"/>)
        /// Retrieve the information after the call to <see cref="Window.ShowDialog()"/> in the <see cref="ModalViewModel"/> object.
        /// </summary>
        /// <param name="dataContext"></param>
        public CustomMsgBox( ref ModalViewModel dataContext )
        {
            _ctx = dataContext;
            EnsureButtons();
            
            dataContext.Holder = this;
            DataContext = _ctx;
            InitializeComponent();
        }

        private void EnsureButtons()
        {
            if( _ctx.Buttons.Count == 0 ) _ctx.Buttons.Add( new ModalButton( _ctx, "OK", null, ModalResult.Ok ) );
        }

        protected override void OnContentRendered( EventArgs e )
        {
            IEnumerable<Button> visualButtons = TreeHelper.FindChildren<Button>( this.buttongrid );
            if(_ctx.FocusedButtonIndex >= visualButtons.ToList().Count) _ctx.FocusedButtonIndex = 0;
            Button b = visualButtons.ElementAtOrDefault( _ctx.FocusedButtonIndex );
            if( b != null ) b.Focus();

            base.OnContentRendered( e );
        }
    }
}
