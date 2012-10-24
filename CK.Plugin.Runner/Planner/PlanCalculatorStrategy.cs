using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CK.Plugin.Hosting
{
    /// <summary>
    /// Drives the way the <see cref="PluginRunner"/> 
    /// </summary>
    public enum PlanCalculatorStrategy
    {
        /// <summary>
        /// Any plugin that can be stopped are not started.
        /// </summary>
        Minimal = 0,
        
        /// <summary>
        /// The <see cref="SolvedConfigStatus.OptionalTryStart"/> and <see cref="SolvedConfigStatus.MustExistTryStart"/> are
        /// taken into account, the fact that the plugin is currently running is ignored.
        /// </summary>
        HonorConfigTryStartIgnoreIsRunning = 1,

        /// <summary>
        /// The <see cref="SolvedConfigStatus.OptionalTryStart"/> and <see cref="SolvedConfigStatus.MustExistTryStart"/> are
        /// taken into account, and plugins that are currently running are kept alive if possible.
        /// </summary>
        HonorConfigTryStart = 2,

        /// <summary>
        /// Same as <see cref="HonorConfigTryStartIgnoreIsRunning"/> with the addition of 
        /// references <see cref="RunningRequirement.OptionalTryStart"/> and <see cref="RunningRequirement.MustExistTryStart"/> from plugins to services.
        /// </summary>
        HonorConfigAndReferenceTryStartIgnoreIsRunning = 10,

        /// <summary>
        /// Same as <see cref="HonorConfigTryStart"/> with the addition of 
        /// references <see cref="RunningRequirement.OptionalTryStart"/> and <see cref="RunningRequirement.MustExistTryStart"/> from plugins to services.
        /// </summary>
        HonorConfigAndReferenceTryStart = 11,

        /// <summary>
        /// Each and every plugin that can be started will be started if possible.
        /// </summary>
        Maximal = 20,
    }

}
