using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CK.Core;

namespace CK.Plugin.Runner.Tests.Planner
{
    public class PluginInfoStub : IPluginInfo
    {
        readonly DiscovererStub _disco;
        List<ServiceReferenceInfoStub> _serviceRef;
        ICKReadOnlyList<ServiceReferenceInfoStub> _serviceRefEx;
        ServiceInfoStub _service;

        internal PluginInfoStub( DiscovererStub disco, string name )
        {
            _disco = disco;
            PluginId = new Guid();
            PluginFullName = name;
            _serviceRef = new List<ServiceReferenceInfoStub>();
            _serviceRefEx = new CKReadOnlyListOnIList<ServiceReferenceInfoStub>( _serviceRef ); 
        }

        public Guid PluginId { get; set; }

        public string PluginFullName { get; private set; }

        public PluginInfoStub AddRef( string serviceName, RunningRequirement r )
        {
            _serviceRef.Add( new ServiceReferenceInfoStub( this, _disco.Services[serviceName], r ) );
            return this;
        }

        ICKReadOnlyList<IServiceReferenceInfo> IPluginInfo.ServiceReferences
        {
            get { return _serviceRefEx; }
        }

        public IServiceInfo Service 
        {
            get { return _service;}
            set 
            {
                if( _service != value )
                {
                    if( _service != null ) _service._plugins.Remove( this );
                    _service = (ServiceInfoStub)value;
                    if( _service != null ) _service._plugins.Add( this );
                }
            } 
        }

        public override string ToString()
        {
            return String.Format( "Plugin: {0}", PluginFullName );
        }

        ICKReadOnlyList<IPluginConfigAccessorInfo> IPluginInfo.EditorsInfo { get { return CKReadOnlyListEmpty<IPluginConfigAccessorInfo>.Empty; } }

        ICKReadOnlyList<IPluginConfigAccessorInfo> IPluginInfo.EditableBy { get { return CKReadOnlyListEmpty<IPluginConfigAccessorInfo>.Empty; } }

        string IPluginInfo.Description { get { return null; } }

        bool IPluginInfo.IsOldVersion { get { return false; } }

        Uri IPluginInfo.RefUrl { get { return null; } }

        ICKReadOnlyList<string> IPluginInfo.Categories { get { return CKReadOnlyListEmpty<string>.Empty; } }

        Uri IPluginInfo.IconUri { get { return null; } }

        IAssemblyInfo IPluginInfo.AssemblyInfo { get { return null; } }

        bool IDiscoveredInfo.HasError { get { return false; } }

        string IDiscoveredInfo.ErrorMessage { get { return null; } }

        Version IVersionedUniqueId.Version { get { return Util.EmptyVersion; } }

        string INamedVersionedUniqueId.PublicName { get { return PluginFullName; } }

        Guid IUniqueId.UniqueId { get { return PluginId; } }

        int IComparable<IPluginInfo>.CompareTo( IPluginInfo other )
        {
            return PluginFullName.CompareTo( other.PluginFullName );
        }

    }
}
