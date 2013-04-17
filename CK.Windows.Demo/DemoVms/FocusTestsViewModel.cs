#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Demo\DemoVms\ConfigCurrentItemTestsViewModel.cs) is part of CiviKey. 
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
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Media;

namespace CK.Windows.Demo
{
    internal class FocusTestsViewModel : ConfigPage
    {
        public FocusTestsViewModel( AppViewModel app, ConfigManager configManager )
            : base( configManager )
        {
            DisplayName = "Test the NoFocusWindow";

            var action = new ConfigItemAction( this.ConfigManager, new SimpleCommand( () =>
            {
                NoFocusWindow sendingWindow = new NoFocusWindow();
                Button sendingButton = new Button();
                sendingButton.Content = "Send a Space";
                sendingButton.Focusable = false;
                sendingButton.Command = new SimpleCommand( () =>
                {
                    KeyEventArgs args = new KeyEventArgs( InputManager.Current.PrimaryKeyboardDevice, PresentationSource.FromVisual( sendingButton ), 0, Key.Space );
                    args.RoutedEvent = Keyboard.KeyDownEvent;
                    InputManager.Current.ProcessInput( args );
                } );

                sendingWindow.Content = sendingButton;
                sendingWindow.Width = 200;
                sendingWindow.Height = 100;
                sendingWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                sendingWindow.Left = 0;
                sendingWindow.Top = 0;

                NoFocusWindow receivingWindow = new NoFocusWindow();
                TextBox receivingTextBox = new TextBox();
                receivingTextBox.Text = "Blop";
                receivingWindow.Content = receivingTextBox;

                sendingWindow.Show();
                receivingWindow.Show();
                receivingWindow.Width = 200;
                receivingWindow.Height = 100;
                receivingWindow.WindowStartupLocation = WindowStartupLocation.Manual;
                receivingWindow.Left = 205;
                receivingWindow.Top = 0;
            } ) );

            action.ImagePath = "Forward.png";
            action.DisplayName = "Show the windows";
            this.Items.Add( action );
        }
    }
}
