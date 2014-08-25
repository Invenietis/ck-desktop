#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Plugin.Runner\Planner\Impl\LiveConfiguration.cs) is part of CiviKey. 
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
using CK.Core;
using System.ComponentModel;
using System.Collections.Specialized;

namespace CK.Plugin.Hosting
{
    class LiveConfiguration : ILiveConfiguration
    {
        CKObservableSortedArrayList<LivePlugin> _plugins;
        CKObservableSortedArrayList<LiveService> _services;

        internal LiveConfiguration()
        {
            _plugins = new CKObservableSortedArrayList<LivePlugin>();
            _services = new CKObservableSortedArrayList<LiveService>();
        }

        ILivePluginInfo ILiveConfiguration.FindPlugin( IPluginInfo p )
        {
            return FindPlugin( p );
        }

        ICKObservableReadOnlyCollection<ILivePluginInfo> ILiveConfiguration.Plugins
        {
            get { return _plugins; }
        }

        ILiveServiceInfo ILiveConfiguration.FindService( IServiceInfo s )
        {
            return FindService( s );
        }

        ICKObservableReadOnlyCollection<ILiveServiceInfo> ILiveConfiguration.Services
        {
            get { return _services; }
        }

        internal LivePlugin FindPlugin( IPluginInfo p )
        {
            int idx = _plugins.IndexOf( p.PluginId, ( live, pluginId ) => live.PluginInfo.PluginId.CompareTo( pluginId ) );
            return idx >= 0 ? _plugins[idx] : null;
        }

        internal LiveService FindService( IServiceInfo s )
        {
            int idx = _services.IndexOf( s.AssemblyQualifiedName, ( live, aqn ) => live.ServiceInfo.AssemblyQualifiedName.CompareTo( aqn ) );
            return idx >= 0 ? _services[idx] : null;
        }

        internal LivePlugin EnsurePlugin( PluginData d )
        {
            LivePlugin live = FindPlugin( d.PluginInfo );

            var service = d.Service != null ? FindService( d.Service.ServiceInfo ) : null;
            if( live == null )
            {
                live = new LivePlugin( d.PluginInfo, d.MinimalRunningRequirement, service, d.Status );
                _plugins.Add( live );
            }
            else
            {
                live.ConfigRequirement = d.MinimalRunningRequirement;
                live.Service = service;
                live.Status = d.Status;
            }
            return live;
        }

        internal LiveService EnsureService( ServiceData s )
        {
            LiveService live = FindService( s.ServiceInfo );
            var generalization = s.Generalization != null ? FindService( s.Generalization.ServiceInfo ) : null;
            var runningPlugin = s.RunningPlugin != null ? FindPlugin( s.RunningPlugin.PluginInfo ) : null;
            if( live == null )
            {
                live = new LiveService( s.ServiceInfo, s.MinimalRunningRequirement, generalization, runningPlugin, s.Status );
                _services.Add( live );
            }
            else
            {
                live.ConfigRequirement = s.MinimalRunningRequirement;
                live.Status = s.Status;
                live.Generalization = generalization;
                live.RunningPlugin = runningPlugin;
            }
            return live;
        }

    }
}
