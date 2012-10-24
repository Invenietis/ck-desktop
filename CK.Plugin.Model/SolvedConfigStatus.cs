#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Plugin.Model\SolvedConfigStatus.cs) is part of CiviKey. 
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
* Copyright © 2007-2012, 
*     Invenietis <http://www.invenietis.com>,
*     In’Tech INFO <http://www.intechinfo.fr>,
* All rights reserved. 
*-----------------------------------------------------------------------------*/
#endregion

using System;

namespace CK.Plugin
{
    /// <summary>
    /// Represents a final configuration status that applies to a plugin or a service.
    /// Adds the Disabled notion to the <see cref="RunningRequirements"/> enumeration.
    /// </summary>
    [Flags]
    public enum SolvedConfigStatus
    {
        /// <summary>
        /// Plugin or service is optional.
        /// </summary>
        Optional = RunningRequirement.Optional,

        /// <summary>
        /// Plugin or service is optional, if it exists it should be started if possible.
        /// </summary>
        OptionalTryStart = RunningRequirement.OptionalTryStart,

        /// <summary>
        /// Plugin or service must exist and be runnable (but not necessarily started).
        /// It is guaranteed to be runnable.
        /// </summary>
        MustExist = RunningRequirement.MustExist,

        /// <summary>
        /// Plugin or service must exist and be runnable.
        /// It is always guaranteed to be runnable (and will be initially started if possible) but it can be stopped later.
        /// </summary>
        MustExistTryStart = RunningRequirement.MustExistTryStart,

        /// <summary>
        /// Plugin or service must exist and must be started.
        /// </summary>
        MustExistAndRun = RunningRequirement.MustExistAndRun,

        /// <summary>
        /// Plugin or service is disabled.
        /// </summary>
        Disabled = 8
    }
}
