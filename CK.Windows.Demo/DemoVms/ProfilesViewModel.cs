#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Demo\DemoVms\ProfilesViewModel.cs) is part of CiviKey. 
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
using System.Collections.ObjectModel;

namespace CK.Windows.Demo
{
    //This ViewModel uses the ConfigItemCurrent with "don't ensure that current is not null". It will not set the model unless the user changes it via the combobox.
    internal class ProfilesViewModel : ConfigPage
    {
        ProfilesAutoSetViewModel _profilesAutoSetVM;
        public ProfilesAutoSetViewModel ProfilesAutoSetVM { get { return _profilesAutoSetVM ?? (_profilesAutoSetVM = new ProfilesAutoSetViewModel( ConfigManager )); } }

        public ObservableCollection<string> Profiles { get; set; }        

        string _selectedProfile;
        public string SelectedProfile
        {
            get { return _selectedProfile; }
            set
            {
                _selectedProfile = value;
                NotifyOfPropertyChange( () => SelectedProfile );
            }
        }

        public ProfilesViewModel( ConfigManager configManager )
            : base( configManager )
        {
            DisplayName = "Profiles management";
     
            Profiles = new ObservableCollection<string>();                        

            this.AddCurrentItem<string, ProfilesViewModel>( "Profiles", "", this, ( o ) => o.SelectedProfile, ( o ) => o.Profiles, false, "Choose a profile" );

            this.AddProperty( "Selected Profile", "The selected profile", this, p => p.SelectedProfile );

            this.AddAction( "Add a Profile", () => { Profiles.Add( "Profile - " + DateTime.Now ); } );

            this.AddAction( "Remove a profile", () => { if ( Profiles.Count > 0 ) Profiles.Remove( Profiles.First() ); } );

            //this.AddLink( ProfilesAutoSetVM );
        }
    }
}
