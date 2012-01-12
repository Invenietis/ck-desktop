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
    internal class ProfilesAutoSetViewModel : ConfigPage
    {
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

        public ProfilesAutoSetViewModel( ConfigManager configManager )
            : base( configManager )
        {
            DisplayName = "Profiles management";
     
            Profiles = new ObservableCollection<string>();

            this.AddCurrentItem<string, ProfilesAutoSetViewModel>( "Profiles", "", this, ( o ) => o.SelectedProfile, ( o ) => o.Profiles, true, "Choose a profile" );

            this.AddProperty( "Selected Profile", "The selected profile", this, p => p.SelectedProfile );

            this.AddAction( "Add a Profile", () => { Profiles.Add( "Profile - " + DateTime.Now ); } );

            this.AddAction( "Remove a profile", () => { if ( Profiles.Count > 0 ) Profiles.Remove( Profiles.First() ); } );
        }
    }
}
