#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Plugin.Model\Discoverer\DiscoverDoneEventArgs.cs) is part of CiviKey. 
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CK.Core;

namespace CK.Plugin
{
    public class DiscoverDoneEventArgs : EventArgs
    {
        ICKReadOnlyList<IAssemblyInfo> _newAssemblies;
        ICKReadOnlyList<IAssemblyInfo> _changedAssemblies;
        ICKReadOnlyList<IAssemblyInfo> _deletedAssemblies;

        ICKReadOnlyList<IPluginInfo> _newPlugins;
        ICKReadOnlyList<IPluginInfo> _changedPlugins;
        ICKReadOnlyList<IPluginInfo> _deletedPlugins;

        ICKReadOnlyList<IPluginConfigAccessorInfo> _newEditors;
        ICKReadOnlyList<IPluginConfigAccessorInfo> _changedEditors;
        ICKReadOnlyList<IPluginConfigAccessorInfo> _deletedEditors;

        ICKReadOnlyList<IServiceInfo> _newServices;
        ICKReadOnlyList<IServiceInfo> _changedServices;
        ICKReadOnlyList<IServiceInfo> _deletedServices;

        ICKReadOnlyList<IPluginInfo> _newOldPlugins;
        ICKReadOnlyList<IPluginInfo> _deletedOldPlugins;

        ICKReadOnlyList<string> _newMissingAssemblies;
        ICKReadOnlyList<string> _deletedMissingAssemblies;

        public ICKReadOnlyList<IAssemblyInfo> NewAssemblies { get { return _newAssemblies; } }
        public ICKReadOnlyList<IAssemblyInfo> ChangedAssemblies { get { return _changedAssemblies; } }
        public ICKReadOnlyList<IAssemblyInfo> DeletedAssemblies { get { return _deletedAssemblies; } }

        /// <summary>
        /// Gets the list of new discovered plugins (contains also plugins on error).
        /// </summary>
        public ICKReadOnlyList<IPluginInfo> NewPlugins { get { return _newPlugins; } }
        public ICKReadOnlyList<IPluginInfo> ChangedPlugins { get { return _changedPlugins; } }
        public ICKReadOnlyList<IPluginInfo> DeletedPlugins { get { return _deletedPlugins; } }

        public ICKReadOnlyList<IPluginConfigAccessorInfo> NewEditors { get { return _newEditors; } }
        public ICKReadOnlyList<IPluginConfigAccessorInfo> ChangedEditors { get { return _changedEditors; } }
        public ICKReadOnlyList<IPluginConfigAccessorInfo> DeletedEditors { get { return _deletedEditors; } }
        
        /// <summary>
        /// Gets the list of new discovered services (contains also services on error).
        /// </summary>
        public ICKReadOnlyList<IServiceInfo> NewServices { get { return _newServices; } }
        public ICKReadOnlyList<IServiceInfo> ChangedServices { get { return _changedServices; } }
        public ICKReadOnlyList<IServiceInfo> DeletedServices { get { return _deletedServices; } }

        /// <summary>
        /// Gets the list of appearing old plugins. They may be previously active plugins replaced by a newer version
        /// or a "new" old plugin (when both plugins plugin versions are discovered at once).
        /// </summary>
        public ICKReadOnlyList<IPluginInfo> NewOldPlugins { get { return _newOldPlugins; } }
        public ICKReadOnlyList<IPluginInfo> DeletedOldPlugins { get { return _deletedOldPlugins; } }

        /// <summary>
        /// Gets the list of missing assemblies.
        /// </summary>
        public ICKReadOnlyList<string> NewDisappearedAssemblies { get { return _newMissingAssemblies; } }
        public ICKReadOnlyList<string> DeletedDisappearedAssemblies { get { return _deletedMissingAssemblies; } }

        public int ChangeCount { get; private set; }

        public DiscoverDoneEventArgs(
            ICKReadOnlyList<IAssemblyInfo> newAssemblies, ICKReadOnlyList<IAssemblyInfo> changedAssemblies, ICKReadOnlyList<IAssemblyInfo> deletedAssemblies,
            ICKReadOnlyList<IPluginInfo> newPlugins, ICKReadOnlyList<IPluginInfo> changedPlugins, ICKReadOnlyList<IPluginInfo> deletedPlugins,
            ICKReadOnlyList<IPluginConfigAccessorInfo> newEditors, ICKReadOnlyList<IPluginConfigAccessorInfo> changedEditors, ICKReadOnlyList<IPluginConfigAccessorInfo> deletedEditors,
            ICKReadOnlyList<IServiceInfo> newServices, ICKReadOnlyList<IServiceInfo> changedServices, ICKReadOnlyList<IServiceInfo> deletedServices,
            ICKReadOnlyList<IPluginInfo> newOldPlugins, ICKReadOnlyList<IPluginInfo> deletedOldPlugins,
            ICKReadOnlyList<string> newMissingAssemblies, ICKReadOnlyList<string> deletedMissingAssemblies )
        {
            _newAssemblies = newAssemblies;
            _changedAssemblies = changedAssemblies;
            _deletedAssemblies = deletedAssemblies;
            _newPlugins = newPlugins;
            _changedPlugins = changedPlugins;
            _deletedPlugins = deletedPlugins;
            ChangeCount = newAssemblies.Count + changedAssemblies.Count + deletedAssemblies.Count + newPlugins.Count + changedPlugins.Count + deletedPlugins.Count;
            
            _newEditors = newEditors;
            _changedEditors = changedEditors;
            _deletedEditors = deletedEditors;
            _newServices = newServices;
            _changedServices = changedServices;
            _deletedServices = deletedServices;
            ChangeCount += newEditors.Count + changedEditors.Count + deletedEditors.Count + newServices.Count + changedServices.Count + deletedServices.Count;
            
            _newOldPlugins = newOldPlugins;
            _deletedOldPlugins = deletedOldPlugins;
            _newMissingAssemblies = newMissingAssemblies;
            _deletedMissingAssemblies = deletedMissingAssemblies;
            ChangeCount += newOldPlugins.Count + deletedOldPlugins.Count + newMissingAssemblies.Count + deletedMissingAssemblies.Count;
        }
    }
}
