using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CK.Plugin.Hosting
{
    partial class AlternativeManager
    {
        readonly PlanCalculatorStrategy _strategy;

        Dictionary<PluginData,RPlugin> _plugins;
        Dictionary<IServiceInfo,IServiceDriver> _knownServices;
        List<IDriver> _drivers;
        double _cardinality;

        public AlternativeManager( PlanCalculatorStrategy strategy )
        {
            _strategy = strategy;
            _plugins = new Dictionary<PluginData, RPlugin>();
            _knownServices = new Dictionary<IServiceInfo, IServiceDriver>();
            _drivers = new List<IDriver>();
        }

        internal void AddDisabledPlugin( PluginData p )
        {
            Debug.Assert( p.Disabled );
            _plugins.Add( p, new RPlugin( p ) );
        }

        internal void AddPurePlugin( PluginData p )
        {
            RPlugin r = DoAddPurePlugin( p );
            if( !r.Locked )
            {
                _drivers.Add( new PurePluginDriver( r ) );
            }
        }

        RPlugin DoAddPurePlugin( PluginData p )
        {
            Debug.Assert( !p.Disabled );
            RPlugin result = new RPlugin( p );
            _plugins.Add( p, result );
            return result;
        }

        internal void AddServiceRoot( ServiceRootData serviceRoot )
        {
            Debug.Assert( !serviceRoot.Disabled );
            if( serviceRoot.TheOnlyPlugin != null )
            {
                RPlugin rp = DoAddPurePlugin( serviceRoot.TheOnlyPlugin );
                if( rp.Locked )
                {
                    new ServiceLockedDriver( this, rp );
                }
                else
                {
                    _drivers.Add( new ServiceSingleImplementationDriver( this, rp ) );
                }
            }
            else
            {
                Debug.Assert( serviceRoot.TotalAvailablePluginCount > 1, "There is more than one possible plugin." );
                _drivers.Add( new ServiceRootDriver( this, serviceRoot ) );
            }
        }

        internal void Initialize()
        {
            _cardinality = 1.0;
            foreach( IDriver d in _drivers ) _cardinality *= d.Cardinality;

        }

   }
}
