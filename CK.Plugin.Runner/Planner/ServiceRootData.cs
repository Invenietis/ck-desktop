using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CK.Plugin.Hosting
{
    class ServiceRootData : ServiceData
    {
        ServiceData _mustExistService;
        PluginData _mustExistPluginByConfig;

        internal ServiceRootData( IServiceInfo s, SolvedConfigStatus serviceStatus )
            : base( s, null, serviceStatus )
        {
        }

        public ServiceData MustExistService
        {
            get { return _mustExistService; }
        }

        public PluginData TheOnlyPlugin
        {
            get { return _theOnlyPlugin; }
        }

        public PluginData MustExistPluginByConfig
        {
            get { return _mustExistPluginByConfig; }
        }

        internal void InitializeMustExistService()
        {
            Debug.Assert( !Disabled );
            _mustExistService = GetMustExistService();
            if( _mustExistService == null && ServiceSolvedStatus >= SolvedConfigStatus.MustExist ) _mustExistService = this;
        }

        internal override void SetDisabled( ServiceDisabledReason r )
        {
            base.SetDisabled( r );
            _mustExistService = null;
            _mustExistPluginByConfig = null;
        }

        internal void MustExistServiceChanged( ServiceData s )
        {
            Debug.Assert( !Disabled );
            _mustExistService = s;
        }

        internal void SetMustExistPluginByConfig( PluginData p )
        {
            Debug.Assert( !Disabled );
            Debug.Assert( p.MinimalRunningRequirement >= RunningRequirement.MustExist );
            Debug.Assert( p.Service.GeneralizationRoot == this );
            if( _mustExistPluginByConfig == null )
            {
                _mustExistPluginByConfig = p;
            }
            else
            {
                p.SetDisabled( PluginDisabledReason.AnotherPluginAlreadyExistForTheSameService );
            }
        }
    }
}
