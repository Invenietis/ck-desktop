using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CK.Core;

namespace CK.Plugin.Hosting
{
        partial class AlternativeManager
        {
            abstract class ServiceDriverBase : IServiceDriver
            {
                internal readonly ServiceData _service;
                internal int _refWeight;
                
                public ServiceDriverBase( AlternativeManager m, ServiceData s )
                {
                    _service = s;
                    IServiceInfo si = s.ServiceInfo;
                    while( si != null && !m._knownServices.ContainsKey( si ) )
                    {
                        m._knownServices.Add( si, this );
                        si = si.Generalization;
                    }
                }

                public void SetRunningRef( bool run, RunningRequirement runningRequirement )
                {
                    if( run ) _refWeight += (int)runningRequirement;
                    else _refWeight -= (int)runningRequirement;
                }

                protected static RPlugin[] ReadPlugins( AlternativeManager m, ServiceData s )
                {
                    if( s.PluginCount == 0 ) return null;
                    RPlugin[] result = new RPlugin[s.PluginCount];
                    int i = 0;
                    PluginData p = s.FirstPlugin;
                    while( p != null )
                    {
                        if( !p.Disabled )
                        {
                            var rp = new RPlugin( p );
                            m._plugins.Add( p, rp );
                            result[i++] = rp;
                        }
                        p = p.NextPluginForService;
                    }
                    return result;
                }

                public void BoostStartWeight( int weight )
                {
                    throw new NotImplementedException();
                }

            }

            class ServiceDriver : ServiceDriverBase
            {
                internal readonly ServiceDriver _firstSpecialization;
                internal readonly ServiceDriver _nextSpecialization;
                internal readonly RPlugin[] _plugins;
                internal bool _running;

                public ServiceDriver( AlternativeManager m, ServiceDriver next, ServiceData s )
                    : base( m, s )
                {
                    Debug.Assert( s.PluginCount > 0 );
                    _plugins = ReadPlugins( m, s );
                    _nextSpecialization = next;
                    _firstSpecialization = HandleSpecialization( m, null, s );
                }

                static ServiceDriver HandleSpecialization( AlternativeManager m, ServiceDriver next, ServiceData s )
                {
                    ServiceData spec = s.FirstSpecialization;
                    while( spec != null )
                    {
                        if( !spec.Disabled )
                        {
                            if( spec.PluginCount > 0 ) next = new ServiceDriver( m, next, spec );
                            else next = HandleSpecialization( m, next, spec );
                        }
                        spec = spec.NextSpecialization;
                    }
                    return next;
                }
            }

        }
}
