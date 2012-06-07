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
