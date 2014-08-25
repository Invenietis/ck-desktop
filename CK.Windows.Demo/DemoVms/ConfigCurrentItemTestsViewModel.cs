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

namespace CK.Windows.Demo
{
    internal class ConfigCurrentItemTestsViewModel : ConfigPage
    {
        ProfilesNoNotifyViewModel _profilesNoNotifyVM;
        public ProfilesNoNotifyViewModel ProfilesNoNotifyVM { get { return _profilesNoNotifyVM ?? (_profilesNoNotifyVM = new ProfilesNoNotifyViewModel( ConfigManager )); } }

        ProfilesAutoSetViewModel _profilesAutoSetVM;
        public ProfilesAutoSetViewModel ProfilesAutoSetVM { get { return _profilesAutoSetVM ?? (_profilesAutoSetVM = new ProfilesAutoSetViewModel( ConfigManager )); } }

        ProfilesViewModel _profilesVM;
        public ProfilesViewModel ProfilesVM { get { return _profilesVM ?? ( _profilesVM = new ProfilesViewModel( ConfigManager ) ); } }

        public bool BoolProperty { get; set; }

        public ConfigCurrentItemTestsViewModel( ConfigManager configManager )
            : base( configManager )
        {
            DisplayName = "CurrentItemConfig usages";

            this.AddLink( ProfilesNoNotifyVM );

            this.AddLink( ProfilesAutoSetVM );

            this.AddLink( ProfilesVM );
        }
    }
}
