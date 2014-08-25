#region LGPL License
/*----------------------------------------------------------------------------
* This file (Tests\CK.Plugin.Runner.Tests\Planner\Desc\DiscovererStub.cs) is part of CiviKey. 
*  
* CiviKey is free software: you can redistribute it and/or modify 
* it under the terms of the GNU Lesser General Public License as published 
* by the Free Software Foundation, either version 3 of the License, or 
* (at your option) any later version. 
*  
* CiviKey is distributed in the hope that it will be useful, 
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
* GNU Lesser General Public License for more details. 
* You should have received a copy of the GNU Lesser General Public License 
* along with CiviKey.  If not, see <http://www.gnu.org/licenses/>. 
*  
* Copyright © 2007-2014, 
*     Invenietis <http://www.invenietis.com>,
*     In’Tech INFO <http://www.intechinfo.fr>,
* All rights reserved. 
*-----------------------------------------------------------------------------*/
#endregion

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
