#region LGPL License
/*----------------------------------------------------------------------------
* This file (Tests\CK.Plugin.Runner.Tests\Planner\Desc\ServiceInfoStub.cs) is part of CiviKey. 
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
using CK.Core;

namespace CK.Plugin.Runner.Tests.Planner
{
    public class ServiceInfoStub : IServiceInfo
    {
        readonly DiscovererStub _disco;
        internal List<PluginInfoStub> _plugins;
        ICKReadOnlyList<PluginInfoStub> _pluginsEx;
        ServiceInfoStub _generalization;

        internal ServiceInfoStub( DiscovererStub disco, string name )
        {
            _disco = disco;
            ServiceFullName = name;
            _plugins = new List<PluginInfoStub>();
            _pluginsEx = new CKReadOnlyListOnIList<PluginInfoStub>( _plugins );
            IsDynamicService = true;
        }

        public string ServiceFullName  { get; private set; }

        public bool IsDynamicService { get; set; }

        public IAssemblyInfo AssemblyInfo  { get; set; }

        public ICKReadOnlyList<IPluginInfo> Implementations
        {
            get { return _pluginsEx; }
        }

        public IServiceInfo Generalization
        {
            get { return _generalization; }
            set { _generalization = (ServiceInfoStub)value; }
        }

        public override string ToString()
        {
            return String.Format( "Service: {0}", ServiceFullName );
        }

        string IServiceInfo.AssemblyQualifiedName { get { return null; } }

        ICKReadOnlyCollection<ISimpleMethodInfo> IServiceInfo.MethodsInfoCollection { get { return CKReadOnlyListEmpty<ISimpleMethodInfo>.Empty; } }

        ICKReadOnlyCollection<ISimpleEventInfo> IServiceInfo.EventsInfoCollection { get { return CKReadOnlyListEmpty<ISimpleEventInfo>.Empty; } }

        ICKReadOnlyCollection<ISimplePropertyInfo> IServiceInfo.PropertiesInfoCollection { get { return CKReadOnlyListEmpty<ISimplePropertyInfo>.Empty; } }

        bool IDiscoveredInfo.HasError { get { return false; } }

        string IDiscoveredInfo.ErrorMessage { get { return null; } }

        int IComparable<IServiceInfo>.CompareTo( IServiceInfo other )
        {
            return ServiceFullName.CompareTo( other.ServiceFullName );
        }
    }
}
