using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CK.Core;

namespace CK.Plugin.Hosting
{
    public interface IConfigurationSolverResult
    {
        bool ConfigurationSuccess { get; }

        IReadOnlyCollection<IPluginInfo> BlockingPlugins { get; }

        IReadOnlyCollection<IServiceInfo> BlockingServices { get; }

        IReadOnlyCollection<IPluginInfo> DisabledPlugins { get; }

        IReadOnlyCollection<IPluginInfo> StoppedPlugins { get; }

        IReadOnlyCollection<IPluginInfo> RunningPlugins { get; }
    }

}
