#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Demo\DemoVms\WindowTestsViewModel.cs) is part of CiviKey. 
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
using Caliburn.Micro;
using CK.Windows.Config;
using System.Windows;
using CK.Windows.App;
using System.Threading;
using System.Windows.Threading;
using CK.Windows.Core;

namespace CK.Windows.Demo
{
    internal class WindowTestsViewModel : ConfigPage
    {
        WPFThread _secondThread;
        NoFocusManager _noFocusManager;

        public WindowTestsViewModel( AppViewModel app, ConfigManager configManager )
            : base( configManager )
        {
            DisplayName = "Not activable windows";

            _noFocusManager = new NoFocusManager();

            // Using extension methods.
            this.AddAction( "CiviKey window, not Activable.", () => new DemoVms.NonActivableCiviKeyWindow( _noFocusManager ).Show() );
            this.AddAction( "CiviKey window, truly not Activable (secondary thread).", SecondThread );
            this.AddAction( "CiviKey window, Activable.", () => new DemoVms.ActivableCiviKeyWindowWithText( _noFocusManager ).Show() );
            this.AddAction( "Standard WPF window, Activable.", () => new DemoVms.StandardWindow().Show() );
        }

        void SecondThread()
        {
            if( _secondThread == null ) _secondThread = new WPFThread( "CK-Certified second thread" );
            _secondThread.Dispatcher.BeginInvoke( (System.Action)(() => new DemoVms.NonActivableCiviKeyWindow( _noFocusManager ).Show()), null );
        }

        /// <summary>
        /// Shows how to use the WPF CustomMsgBox
        /// <summary>
        internal void ShowCustomMessageBox()
        {
            ModalViewModel modalDataContext = new ModalViewModel( "Mise à jour disponible", String.Format( "Testing TextWrapping to make sure that the window {0}won't end up extremely wide.", Environment.NewLine ), true, "Remember my choice test", CustomMsgBoxIcon.Question, 1 );
            modalDataContext.Buttons.Add( new ModalButton( modalDataContext, "OK", null, ModalResult.Ok ) );
            modalDataContext.Buttons.Add( new ModalButton( modalDataContext, "Cancel", () => Console.Out.WriteLine( "Testing Cancel" ), ModalResult.Cancel ) );
            CustomMsgBox b = new CustomMsgBox( ref modalDataContext );
            b.ShowDialog();
            Console.Out.WriteLine( String.Format( "result : {0}", modalDataContext.ModalResult + " \r\n checkbox checked : " + modalDataContext.IsCheckboxChecked ) );
        }
    }
}
