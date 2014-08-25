#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Config\ConfigPage.cs) is part of CiviKey. 
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

using Caliburn.Micro;

namespace CK.Windows.Config
{

    public class ConfigPage : Screen, IConfigItemContainer
    {
        ConfigManager _configManager;
        string _description;
        BindableCollection<object> _items;

        public ConfigPage( ConfigManager configManager )
        {
            _configManager = configManager;
            _items = new BindableCollection<object>();
        }

        public ConfigManager ConfigManager { get { return _configManager; } }

        public string Description
        {
            get { return _description; }
            set
            {
                if( _description != value )
                {
                    _description = value;
                    NotifyOfPropertyChange( "Description" );
                }
            }
        }

        public IObservableCollection<object> Items { get { return _items; } }

    }


}
