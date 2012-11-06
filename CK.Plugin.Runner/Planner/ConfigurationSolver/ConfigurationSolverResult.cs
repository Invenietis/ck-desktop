using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CK.Plugin.Hosting
{
    class ConfigurationSolverResult : IConfigurationSolverResult
    {
        ConfigurationSolver _solver;
        
        List<PluginData> _blockingPlugins;
        List<ServiceData> _blockingServices;

        internal ConfigurationSolverResult( ConfigurationSolver solver )
        {
            _solver = solver;
            ConfigurationSuccess = true;
        }

        public ConfigurationSolverResult( ConfigurationSolver solver, List<PluginData> blockingPlugins, List<ServiceData> blockingServices )
        {
            Debug.Assert( blockingPlugins != null || blockingServices != null );
            _solver = solver;
            if( blockingPlugins != null )
            {
                _blockingPlugins = blockingPlugins;
                ConfigurationErrorCount = _blockingPlugins.Count;
            }
            if( blockingServices != null )
            {
                _blockingServices = blockingServices;
                ConfigurationErrorCount += _blockingServices.Count;
            }
        }

        public bool ConfigurationSuccess { get; private set; }

        public int ConfigurationErrorCount { get; private set; }

        internal void ApplyToLiveConfiguration( LiveConfiguration config )
        {
            Debug.Assert( ConfigurationSuccess );
            
        }

    }

}
