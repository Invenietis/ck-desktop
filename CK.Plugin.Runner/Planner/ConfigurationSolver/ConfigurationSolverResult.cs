using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CK.Core;

namespace CK.Plugin.Hosting
{
    class ConfigurationSolverResult : IConfigurationSolverResult
    {
        IReadOnlyCollection<IPluginInfo> _blockingPlugins;
        IReadOnlyCollection<IServiceInfo> _blockingServices;
        
        IReadOnlyCollection<IPluginInfo> _disabledPlugins;
        IReadOnlyCollection<IPluginInfo> _stoppedPlugins;
        IReadOnlyCollection<IPluginInfo> _runningPlugins;

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

        public IReadOnlyCollection<IPluginInfo> BlockingPlugins 
        { 
            get { return _blockingPlugins; } 
        }

        public IReadOnlyCollection<IServiceInfo> BlockingServices 
        {
            get { return _blockingServices; } 
        }

        public IReadOnlyCollection<IPluginInfo> DisabledPlugins
        {
            get { return _disabledPlugins; }
        }

        public IReadOnlyCollection<IPluginInfo> StoppedPlugins
        {
            get { return _stoppedPlugins; }
        }

        public IReadOnlyCollection<IPluginInfo> RunningPlugins
        {
            get { return _runningPlugins; }
        }

        internal void ApplyToLiveConfiguration( LiveConfiguration config )
        {
            Debug.Assert( ConfigurationSuccess );
            
        }



    }

}
