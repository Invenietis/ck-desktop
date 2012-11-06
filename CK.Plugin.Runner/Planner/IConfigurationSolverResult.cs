using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CK.Plugin.Hosting
{
    public interface IConfigurationSolverResult
    {
        bool ConfigurationSuccess { get; }

        int ConfigurationErrorCount { get; }
    }

}
