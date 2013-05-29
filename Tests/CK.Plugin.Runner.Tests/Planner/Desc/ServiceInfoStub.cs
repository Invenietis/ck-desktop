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
