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
