using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CK.Plugin.Hosting
{
        partial class AlternativeManager
        {
            class ServiceRootDriver : ServiceDriver, IDriver
            {
                internal readonly ServiceRootData _root;
                readonly int _cardinality;

                public ServiceRootDriver( AlternativeManager m, ServiceRootData r )
                    : base( m, null, r )
                {
                    Debug.Assert( !r.Disabled && r.TotalAvailablePluginCount > 1, "There is more than one possible plugin." );
                    _root = r;
                    _cardinality = r.TotalAvailablePluginCount;
                    if( _service.MinimalRunningRequirement != RunningRequirement.MustExistAndRun ) _cardinality += 1;
                }

                public int Cardinality
                {
                    get {  return _cardinality; }
                }
            }
        }
}
