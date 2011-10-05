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

        public bool BoolProperty { get; set; }

        public SubViewModel( ConfigManager configManager )
            : base( configManager )
        {
            DisplayName = "Sub view";

            this.AddProperty( "Bool property", this, ( o ) => o.BoolProperty );

            this.AddLink( _specificVM ?? (_specificVM = new SpecificViewModel( configManager )) );
        }
    }
}
