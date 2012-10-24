using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CK.Plugin.Hosting
{
    class ServiceRootData : ServiceData
    {
        internal ServiceRootData( IServiceInfo s, SolvedConfigStatus serviceStatus )
            : base( s, null, serviceStatus )
        {
        }

        public PluginData MustExistPlugin { get; set; }

        internal bool SetMustExistService()
        {
            if( !Disabled ) MustExistService = GetMustExistService();
            return !Disabled;
        }

        internal bool InitializeFromPluginsAndSetMustExistPlugin()
        {
            if( !Disabled ) 
            {
                MustExistPlugin = InitializeFromPluginsAndGetMustExistPlugin();
                if( MustExistPlugin != null ) MustExistService = MustExistPlugin.Service;
                Debug.Assert( MustExistPlugin == null || TheSingleImplementation == MustExistPlugin );
                Debug.Assert( TheSingleImplementation == null || TheSingleImplementation.MinimalRunningRequirement == MinimalRunningRequirement );
            }
            return !Disabled;
        }

    }
}
