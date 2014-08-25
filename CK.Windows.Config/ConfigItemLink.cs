#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Config\ConfigItemLink.cs) is part of CiviKey. 
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
using System.ComponentModel;
using System.Windows.Input;

namespace CK.Windows.Config
{

    public class ConfigItemLink : ConfigItem, ICommand
    {
        public ConfigPage _target;

        public ConfigItemLink( ConfigManager configManager, ConfigPage target, INotifyPropertyChanged monitor )
            : base( configManager )
        {
            if( target == null ) throw new ArgumentNullException( "target" );
            DisplayName = target.DisplayName;
            Description = target.Description;
            _target = target;
            if( monitor != null )
            {
                monitor.PropertyChanged += ( o, e ) =>
                {
                    if( e.PropertyName == "DisplayName" ) DisplayName = _target.DisplayName;
                    if( e.PropertyName == "Description" ) Description = _target.Description;
                };
            }

        }

        public ICommand GotoCommand { get { return this; } }

        public void Goto()
        {
            ConfigManager.ActivateItem( _target );
        }

        bool ICommand.CanExecute( object parameter )
        {
            return true;
        }

        event EventHandler ICommand.CanExecuteChanged
        {
            add { }
            remove { }
        }

        void ICommand.Execute( object parameter )
        {
            Goto();
        }

    }
}
