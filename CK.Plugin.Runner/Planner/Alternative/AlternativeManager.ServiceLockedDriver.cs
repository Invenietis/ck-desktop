using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CK.Plugin.Hosting
{
        partial class AlternativeManager
        {
            class ServiceLockedDriver : IServiceDriver
            {
                readonly RPlugin RPlugin;

                public ServiceLockedDriver( AlternativeManager m, RPlugin p )
                {
                    Debug.Assert( p.Locked );
                    Debug.Assert( p.PluginData.Service.GeneralizationRoot.TotalAvailablePluginCount == 1 );
                    RPlugin = p;
                    IServiceInfo s = p.PluginData.Service.ServiceInfo;
                    while( s != null )
                    {
                        m._knownServices.Add( s, this );
                        s = s.Generalization;
                    }
                }

                public void BoostStartWeight( int weight )
                {

                }
            }
        }
}
