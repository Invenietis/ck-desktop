using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Caliburn.Micro
{
    public class WizardNavigableEventArgs : EventArgs
    {
        public IWizardNavigable Wizard { get; private set; }

        public WizardNavigableEventArgs( IWizardNavigable wizard )
        {
            Wizard = wizard;
        }
    }
}
