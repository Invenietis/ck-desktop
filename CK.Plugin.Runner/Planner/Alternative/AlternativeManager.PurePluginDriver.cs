using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CK.Plugin.Hosting
{
        partial class AlternativeManager
        {
            class PurePluginDriver : IDriver
            {
                public readonly RPlugin RPlugin;

                public PurePluginDriver( RPlugin p )
                {
                    RPlugin = p;
                }

                public int Cardinality { get { return 2; } }
    
            }
        }
}
