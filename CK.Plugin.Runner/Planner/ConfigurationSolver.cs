using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CK.Core;
using System.Diagnostics;

namespace CK.Plugin.Hosting
{
    internal class ConfigurationSolver
    {
        readonly Predicate<IPluginInfo> _isPluginRunning;

        Dictionary<IServiceInfo,ServiceData> _services;
        List<ServiceRootData> _serviceRoots;
        Dictionary<IPluginInfo,PluginData> _plugins;

        public ConfigurationSolver( Predicate<IPluginInfo> isPluginRunning )
        {
            _isPluginRunning = isPluginRunning;
            _services = new Dictionary<IServiceInfo, ServiceData>();
            _serviceRoots = new List<ServiceRootData>();

            _plugins = new Dictionary<IPluginInfo, PluginData>();
        }

        public bool Initialize( Dictionary<object, SolvedConfigStatus> finalConfig, PlanCalculatorStrategy strategy, IEnumerable<IServiceInfo> services, IEnumerable<IPluginInfo> plugins )
        {
            // Registering all Services.
            _services.Clear();
            _serviceRoots.Clear();
            foreach( IServiceInfo sI in services )
            {
                // This creates services and applies solved configuration to them: directly disabled services
                // and specializations disabled by their generalizations' configuration are handled.
                RegisterService( finalConfig, sI );
            }
            // Service trees have been built and we have the roots.
            // We can now handle MustRun services: there must be at most one such service by service 
            // root otherwise it is a configuration error.
            foreach( var root in _serviceRoots )
            {
                if( !root.Disabled ) root.InitializeMustExistService();
            }
            // We can now instantiate plugin data. 
            _plugins.Clear();
            foreach( IPluginInfo p in plugins )
            {
                RegisterPlugin( finalConfig, p );
            }
            // Initialize services disabled state based on their available plugins:
            // roots without any available plugins are de facto disabled.
            foreach( var service in _services.Values )
            {
                if( !service.Disabled ) service.OnAllPluginsAdded();
            }
            // Now, we apply ServiceReference MustExist constraints from every plugins to their referenced services.
            foreach( PluginData p in _plugins.Values )
            {
                // When a plugin is disabled because of a disabled required service reference and it implements a service, the service
                // become disabled (if it has no more available implementations) and that triggers disabling of plugins that require
                // the service. This works because disable flag on each participant is carefully set before propagating the
                // information to others to avoid loops and because such plugins reference themselves at the required service (AddMustExistReferencer).
                if( !p.Disabled && p.MinimalRunningRequirement >= RunningRequirement.MustExist )
                {
                    p.CheckReferencesWhenMustExist();
                }
            }
            // Time to build the alternatives.
            AlternativeManager m = new AlternativeManager( strategy );
            foreach( PluginData p in _plugins.Values )
            {
                if( p.Disabled )
                {
                    if( p.PluginSolvedStatus != SolvedConfigStatus.Disabled && p.MinimalRunningRequirement >= RunningRequirement.MustExist )
                    {
                        // Conflict.
                        return false;
                    }
                    m.AddDisabledPlugin( p );
                }
                else if( p.Service == null )
                {
                    m.AddPurePlugin( p );
                }
            }

            // Any Service that has a SolvedConfigStatus greater or equal to MustExist and is disabled leads to an impossible plan.
            foreach( ServiceData s in _services.Values )
            {
                if( s.Disabled )
                {
                    if( s.ServiceSolvedStatus != SolvedConfigStatus.Disabled && s.MinimalRunningRequirement >= RunningRequirement.MustExist )
                    {
                        // Conflict.
                        return false;
                    }
                    // When a whole Service is disabled, the AlternativeManager has nothing to do:
                    // any unknown service to him is considered as disabled.
                }
                else if( s.Generalization == null )
                {
                    m.AddServiceRoot( s.GeneralizationRoot );
                }
            }
            return true;
        }

        ServiceData RegisterService( Dictionary<object, SolvedConfigStatus> finalConfig, IServiceInfo s )
        {
            ServiceData data;
            if( _services.TryGetValue( s, out data ) ) return data;

            SolvedConfigStatus serviceStatus = finalConfig.GetValueWithDefault( s, SolvedConfigStatus.Optional );
            // Handle generalization.
            ServiceData dataGen = null;
            if( s.Generalization != null )
            {
                dataGen = RegisterService( finalConfig, s.Generalization );
            }
            Debug.Assert( (s.Generalization == null) == (dataGen == null) );
            if( dataGen == null )
            {
                var dataRoot = new ServiceRootData( s, serviceStatus );
                _serviceRoots.Add( dataRoot );
                data = dataRoot;
            }
            else
            {
                data = new ServiceData( s, dataGen, serviceStatus );
            }
            _services.Add( s, data );
            return data;
        }

        PluginData RegisterPlugin( Dictionary<object, SolvedConfigStatus> finalConfig, IPluginInfo p )
        {
            PluginData data;
            if( _plugins.TryGetValue( p, out data ) ) return data;
            
            SolvedConfigStatus pluginStatus = finalConfig.GetValueWithDefault( p, SolvedConfigStatus.Optional );
            ServiceData service = p.Service != null ? _services[ p.Service ] : null;
            data = new PluginData( _services, p, service, _isPluginRunning( p ), pluginStatus );
            _plugins.Add( p, data );
            return data;
        }


    }
}
