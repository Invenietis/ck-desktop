using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CK.Plugin.Hosting
{
    enum PluginRunningRequirementReason
    {
        /// <summary>
        /// Initialized by PluginData constructor.
        /// </summary>
        Config,
        
        /// <summary>
        /// Sets by ServiceData.RetrieveTheOnlyPlugin.
        /// </summary>
        FromServiceConfigToSinglePlugin,

        /// <summary>
        /// Sets by ServiceData.RetrieveTheOnlyPlugin and ServiceData.SetRunningRequirement.
        /// </summary>
        FromServiceToSinglePlugin,

    }

}
