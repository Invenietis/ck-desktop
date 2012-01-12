using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using CK.Windows.Config;
using System.Windows;

namespace CK.Windows.Demo
{
    internal class SubViewModel : ConfigPage
    {
        SpecificViewModel _specificVM;
        public SpecificViewModel SpecificVM { get { return _specificVM ?? (_specificVM = new SpecificViewModel( ConfigManager )); } }

        ProfilesViewModel _profilesVM;
        public ProfilesViewModel ProfilesVM { get { return _profilesVM ?? ( _profilesVM = new ProfilesViewModel( ConfigManager ) ); } }

        public bool BoolProperty { get; set; }

        public SubViewModel( ConfigManager configManager )
            : base( configManager )
        {
            DisplayName = "Sub view";

            this.AddProperty( "Bool property", this, ( o ) => o.BoolProperty );

            this.AddLink( SpecificVM );

            this.AddLink( ProfilesVM );
        }
    }
}
