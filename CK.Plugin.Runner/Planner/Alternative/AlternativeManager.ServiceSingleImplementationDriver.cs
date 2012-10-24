using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CK.Plugin.Hosting
{
        partial class AlternativeManager
        {
            class ServiceSingleImplementationDriver : ServiceDriverBase, IDriver
            {
                readonly RPlugin RPlugin;

                public ServiceSingleImplementationDriver( AlternativeManager m, RPlugin p )
                    : base( m, p.PluginData.Service )
                {
                    Debug.Assert( !p.Locked );
                    RPlugin = p;
                }

                public int Cardinality { get { return 2; } }

            }
        }
}
