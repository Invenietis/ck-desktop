using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using CK.Windows.Config;
using System.Windows;

namespace CK.Windows.Demo
{
    internal class RootViewModel : ConfigPage
    {
        public RootViewModel( ConfigManager configManager )
            : base( configManager )
        {
            DisplayName = "Root view";

            this.AddAction( "Show a popup", () => MessageBox.Show( "Pow!" ) );
            this.AddAction( "Show another popup", () => MessageBox.Show( "Another Pow!" ) );
        }
    }
}
