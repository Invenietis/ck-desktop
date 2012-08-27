#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Demo\DemoVms\ProfilesNoNotifyViewModel.cs) is part of CiviKey. 
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

    //This ViewModel uses the ConfigCurrentItem with an underlying collection that doesn't implement INotifyPropertyChanged
    //Instead, it uses the RefreshView method on the ConfigItemCurrent object, to have the view refresh accordingly
    internal class ProfilesNoNotifyViewModel : ConfigPage
    {
        public List<string> Profiles { get; set; }
        private ConfigItemCurrent<string> _profiles;

        string _selectedProfile;
        public string SelectedProfile
        {
            get { return _selectedProfile; }
            set
            {
                _selectedProfile = value;
                _profiles.RefreshCurrent( this, new EventArgs() );
            }
        }

        public ProfilesNoNotifyViewModel( ConfigManager configManager )
            : base( configManager )
        {
            DisplayName = "Profiles management - Model is not INotifyPropertyChanged";

            Profiles = new List<string>();

            _profiles = this.AddCurrentItem<string, ProfilesNoNotifyViewModel>( "Profiles", "", this, ( o ) => o.SelectedProfile, ( o ) => o.Profiles, false, "Choose a profile" );

            this.AddAction( "Add a Profile", () =>
            {
                Profiles.Add( "Profile - " + DateTime.Now );

                _profiles.RefreshValues( this, new EventArgs() );
            } );

            this.AddAction( "Remove a profile", () =>
            {
                if ( Profiles.Count > 0 )
                {
                    Profiles.Remove( Profiles.First() );
                    _profiles.RefreshValues( this, new EventArgs() );
                }
            } );

            this.AddAction( "Select Other Profile", () =>
            {
                if ( Profiles.Count > 0 )
                {
                    Random r = new Random();
                    int result = (r.Next( 0, Profiles.Count ));

                    SelectedProfile = Profiles[result];
                }
            } );
        }
    }
}
