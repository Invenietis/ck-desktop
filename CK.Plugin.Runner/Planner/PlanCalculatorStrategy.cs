#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Plugin.Runner\Planner\PlanCalculatorStrategy.cs) is part of CiviKey. 
*  
* CiviKey is free software: you can redistribute it and/or modify 
* it under the terms of the GNU Lesser General Public License as published 
* by the Free Software Foundation, either version 3 of the License, or 
* (at your option) any later version. 
*  
* CiviKey is distributed in the hope that it will be useful, 
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
* GNU Lesser General Public License for more details. 
* You should have received a copy of the GNU Lesser General Public License 
* along with CiviKey.  If not, see <http://www.gnu.org/licenses/>. 
*  
* Copyright © 2007-2014, 
*     Invenietis <http://www.invenietis.com>,
*     In’Tech INFO <http://www.intechinfo.fr>,
* All rights reserved. 
*-----------------------------------------------------------------------------*/
#endregion

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
