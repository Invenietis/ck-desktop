using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using CK.Windows.Config;
using System.Windows;

namespace CK.Windows.Demo
{
    internal class SpecificViewModel : ConfigPage
    {
        public SpecificViewModel( ConfigManager configManager )
            : base( configManager )
        {
            DisplayName = "Specific view";
        }

        public string Property { get { return "Some bound text !"; } }
    }
}
