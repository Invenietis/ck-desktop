#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Plugin.Runner\Planner\ConfigurationSolver\ConfigurationSolverResult.cs) is part of CiviKey. 
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
using System.Diagnostics;
using CK.Core;

namespace CK.Plugin.Hosting
{
    public class ConfigurationSolverResult : IConfigurationSolverResult
    {
        ICKReadOnlyCollection<IPluginInfo> _blockingPlugins;
        ICKReadOnlyCollection<IServiceInfo> _blockingServices;
        
        ICKReadOnlyCollection<IPluginInfo> _disabledPlugins;
        ICKReadOnlyCollection<IPluginInfo> _stoppedPlugins;
        ICKReadOnlyCollection<IPluginInfo> _runningPlugins;

        public ConfigurationSolverResult( List<IPluginInfo> blockingPlugins, List<IServiceInfo> blockingServices )
        {
            Debug.Assert( blockingPlugins != null || blockingServices != null );
            if( blockingPlugins != null )
            {
                _blockingPlugins = blockingPlugins.ToReadOnlyCollection();
            }
            if( blockingServices != null )
            {
                _blockingServices = blockingServices.ToReadOnlyCollection();
            }
        }

        public ConfigurationSolverResult( List<IPluginInfo> disabledPlugins, List<IPluginInfo> stoppedPlugins, List<IPluginInfo> runningPlugins )
        {
            ConfigurationSuccess = true;
            _disabledPlugins = disabledPlugins.ToReadOnlyCollection();
            _stoppedPlugins = stoppedPlugins.ToReadOnlyCollection();
            _runningPlugins = runningPlugins.ToReadOnlyCollection();
        }

        public bool ConfigurationSuccess { get; private set; }

        public ICKReadOnlyCollection<IPluginInfo> BlockingPlugins 
        { 
            get { return _blockingPlugins; } 
        }

        public ICKReadOnlyCollection<IServiceInfo> BlockingServices 
        {
            get { return _blockingServices; } 
        }

        public ICKReadOnlyCollection<IPluginInfo> DisabledPlugins
        {
            get { return _disabledPlugins; }
        }

        public ICKReadOnlyCollection<IPluginInfo> StoppedPlugins
        {
            get { return _stoppedPlugins; }
        }

        public ICKReadOnlyCollection<IPluginInfo> RunningPlugins
        {
            get { return _runningPlugins; }
        }

        internal void ApplyToLiveConfiguration( LiveConfiguration config )
        {
            Debug.Assert( ConfigurationSuccess );
            
        }
    }
}
