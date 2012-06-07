using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using CK.Windows.Config;

namespace CK.Windows.Demo
{
    internal class AppViewModel : Conductor<IScreen>
    {
        public ConfigManager ConfigManager { get; private set; }

        public AppViewModel()
        {
            DisplayName = "CK-Windows demo App";

            ConfigManager = new ConfigManager();
            ConfigManager.ActivateItem( new RootViewModel( ConfigManager ) );
            ActivateItem( ConfigManager );
        }
    }
}
