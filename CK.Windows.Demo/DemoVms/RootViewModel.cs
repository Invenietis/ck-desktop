#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Demo\DemoVms\RootViewModel.cs) is part of CiviKey. 
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
using Caliburn.Micro;
using CK.Windows.Config;
using System.Windows;
using CK.Windows.App;

namespace CK.Windows.Demo
{
    internal class RootViewModel : ConfigPage
    {
        SubViewModel _subvm;

        public RootViewModel( ConfigManager configManager )
            : base( configManager )
        {
            DisplayName = "Root view";

            this.AddAction( "Show a popup", () => MessageBox.Show( "Pow!" ) );
            this.AddAction( "Show another popup", () => MessageBox.Show( "Another Pow!" ) );
            this.AddAction( "Show a custom modal popup", ShowCustomMessageBox );
            this.AddAction( "Show another custom modal popup", ShowOtherCustomMessageBox );
            this.AddLink( _subvm ?? ( _subvm = new SubViewModel( configManager ) ) );
        }

        /// <summary>
        /// Shows how to use the WPF CustomMsgBox
        /// </summary>
        internal void ShowCustomMessageBox()
        {
            ModalViewModel modalDataContext = new ModalViewModel( "Mise à jour disponible", String.Format( "Testing TextWrapping to make sure that the window {0}won't end up extremely wide.", Environment.NewLine ), true, "Remember my choice test", CustomMsgBoxIcon.Question, 1 );

            modalDataContext.Buttons.Add( new ModalButton( modalDataContext, "OK", null, ModalResult.Ok ) );
            modalDataContext.Buttons.Add( new ModalButton( modalDataContext, "Cancel", () => Console.Out.WriteLine( "Testing Cancel" ), ModalResult.Cancel ) );

            CustomMsgBox b = new CustomMsgBox( ref modalDataContext );
            b.ShowDialog();

            Console.Out.WriteLine( String.Format( "result : {0}", modalDataContext.ModalResult + " \r\n checkbox checked : " + modalDataContext.IsCheckboxChecked ) );
        }

        /// <summary>
        /// Shows how to use the WPF CustomMsgBox
        /// </summary>
        internal void ShowOtherCustomMessageBox()
        {
            ModalViewModel modalDataContext = new ModalViewModel( "Mise à jour disponible", "No buttons have been added to this MsgBox", CustomMsgBoxIcon.Question, 1 );

            CustomMsgBox b = new CustomMsgBox( ref modalDataContext );
            b.ShowDialog();

            Console.Out.WriteLine( String.Format( "result : {0}", modalDataContext.ModalResult + " \r\n checkbox checked : " + modalDataContext.IsCheckboxChecked ) );
        }
    }
}
