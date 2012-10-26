#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Demo\DemoVms\ProfilesAutoSetViewModel.cs) is part of CiviKey. 
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
    public class CustomProfile
    {
        public string Name { get; set; }
        public CustomProfile( string name )
        {
            Name = name;
        }
    }

    //This ViewModel uses the ConfigCurrentItem in Auto set mode. 
    //If the collectionView has items and that the current is null, the collection will set the model's current as the first value of the collection
    //In this case, The Model is INotifyPropertyChanged
    internal class ProfilesAutoSetViewModel : ConfigPage
    {

        ProfilesNoNotifyViewModel _profilesAutoSetVM;
        public ProfilesNoNotifyViewModel ProfilesNoNotifyVM { get { return _profilesAutoSetVM ?? ( _profilesAutoSetVM = new ProfilesNoNotifyViewModel( ConfigManager ) ); } }

        public ObservableCollection<CustomProfile> Profiles { get; set; }

        CustomProfile _selectedProfile;
        public CustomProfile SelectedProfile
        {
            get { return _selectedProfile; }
            set
            {
                _selectedProfile = value;
                NotifyOfPropertyChange( () => SelectedProfile );
            }
        }

        public ProfilesAutoSetViewModel( ConfigManager configManager )
            : base( configManager )
        {
            DisplayName = "Profiles management - Auto setting the current element";

            Profiles = new ObservableCollection<CustomProfile>();

            this.AddCurrentItem<CustomProfile, ProfilesAutoSetViewModel>( "Profiles", "", this, ( o ) => o.SelectedProfile, ( o ) => o.Profiles, true, "Choose a profile" );

            this.AddProperty( "Selected Profile", "The selected profile", this, p => p.SelectedProfile );

            this.AddAction( "Add a Profile", () => { Profiles.Add( new CustomProfile( "Profile - " + DateTime.Now ) ); } );

            this.AddAction( "Remove a profile", () => { if( Profiles.Count > 0 ) Profiles.Remove( Profiles.First() ); } );

            //this.AddLink( ProfilesNoNotifyVM );
        }
    }
}
