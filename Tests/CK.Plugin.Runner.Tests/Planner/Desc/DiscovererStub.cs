using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CK.Plugin.Runner.Tests.Planner
{
    public class DiscovererStub
    {
        public readonly Dictionary<string,PluginInfoStub> Plugins;
        public readonly Dictionary<string,ServiceInfoStub> Services;
        readonly Dictionary<string,SolvedConfigStatus> _configStatus;

        public DiscovererStub()
        {
            Plugins = new Dictionary<string, PluginInfoStub>();
            Services = new Dictionary<string, ServiceInfoStub>();
            _configStatus = new Dictionary<string, SolvedConfigStatus>();
        }

        public PluginInfoStub Plugin( string name, string serviceName = null )
        {
            PluginInfoStub p = new PluginInfoStub( this, name );
            if( serviceName != null )
            {
                p.Service = Services[serviceName];
            }
            Plugins.Add( p.PluginFullName, p );
            return p;
        }

        public ServiceInfoStub Service( string serviceName, string generalizationName = null )
        {
            ServiceInfoStub s = new ServiceInfoStub( this, serviceName );
            if( generalizationName != null )
            {
                s.Generalization = Services[generalizationName];
            }
            Services.Add( s.ServiceFullName, s );
            return s;
        }

        public void SetFinalConfig( string serviceOrPluginName, SolvedConfigStatus solvedConfigStatus )
        {
            if( solvedConfigStatus == SolvedConfigStatus.Optional )
                _configStatus.Remove( serviceOrPluginName );
            else _configStatus[serviceOrPluginName] = solvedConfigStatus;
        }

        public IEnumerable<IPluginInfo> AllPluginInfo
        {
            [DebuggerStepThrough]
            get { return Plugins.Values; }
        }

        public IEnumerable<IServiceInfo> AllServiceInfo
        {
            [DebuggerStepThrough]
            get { return Services.Values; }
        }

        public Dictionary<object,SolvedConfigStatus> FinalConfig
        {
            [DebuggerStepThrough]
            get 
            {
                var r = new Dictionary<object, SolvedConfigStatus>();
                foreach( var k in _configStatus )
                {
                    PluginInfoStub p;
                    if( Plugins.TryGetValue( k.Key, out p ) ) r.Add( p, k.Value );
                    else r.Add( Services[k.Key], k.Value );
                }
                return r;
            }
        }



    }
}
