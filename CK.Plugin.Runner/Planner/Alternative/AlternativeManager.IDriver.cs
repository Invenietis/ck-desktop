using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CK.Plugin.Hosting
{
        partial class AlternativeManager
        {
            interface IDriver
            {
                int Cardinality { get; }
            }

            interface IServiceDriver
            {
                void BoostStartWeight( int weight );
            }
        }
}
