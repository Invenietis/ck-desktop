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
        ObservableSortedArrayList<LivePlugin> _plugins;
        ObservableSortedArrayList<LiveService> _services;

        internal LiveConfiguration()
        {
            _plugins = new ObservableSortedArrayList<LivePlugin>();
            _services = new ObservableSortedArrayList<LiveService>();
        }

        ILivePluginInfo ILiveConfiguration.FindPlugin( IPluginInfo p )
        {
            return FindPlugin( p );
        }

        IObservableReadOnlyCollection<ILivePluginInfo> ILiveConfiguration.Plugins
        {
            get { return _plugins; }
        }

        ILiveServiceInfo ILiveConfiguration.FindService( IServiceInfo s )
        {
            return FindService( s );
        }

        IObservableReadOnlyCollection<ILiveServiceInfo> ILiveConfiguration.Services
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
