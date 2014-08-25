#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Plugin.Runner\Planner\ConfigurationSolver\PluginServiceRelation.cs) is part of CiviKey. 
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
    class PluginServiceRelation
    {
        public readonly PluginData Plugin;
        public readonly RunningRequirement Requirement;
        public readonly ServiceData Service;
        
        public PluginServiceRelation( PluginData p, RunningRequirement r, ServiceData s )
        {
            Requirement = r;
            Service = s;
            Plugin = p;
            NextServiceRef = s.AddServiceRef( this );
        }

        /// <summary>
        /// Support for the linked list of plugin references for a service.
        /// </summary>
        public readonly PluginServiceRelation NextServiceRef;

        /// <summary>
        /// Gets whether this relation is necessarily satisfied regardless of dynamic state of the system.
        /// </summary>
        public bool IsStructurallySatisfied
        {
            get 
            {
                // When Requirement is MustExistAndRun, the service must be running.
                if( Requirement == RunningRequirement.MustExistAndRun ) return Service.MinimalRunningRequirement == RunningRequirement.MustExistAndRun;
                // When Requirement is MustExist or MustExistTryStart, the service must exist. 
                if( Requirement >= RunningRequirement.MustExist ) return Service.MinimalRunningRequirement >= RunningRequirement.MustExist;
                // When Requirement is optional, the service can be in any state.
                return true;
            }
        }
    }
}
