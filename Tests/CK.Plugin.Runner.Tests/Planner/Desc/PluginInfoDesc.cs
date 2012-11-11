using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CK.Core;

namespace CK.Plugin.Runner.Tests.Planner
{
    public class PluginInfoDesc : IPluginInfo
    {
        readonly DiscovererDesc _disco;
        List<ServiceReferenceInfoDesc> _serviceRef;
        IReadOnlyList<ServiceReferenceInfoDesc> _serviceRefEx;
        ServiceInfoDesc _service;

        internal PluginInfoDesc( DiscovererDesc disco, string name )
        {
            _disco = disco;
            PluginId = new Guid();
            PluginFullName = name;
            _serviceRef = new List<ServiceReferenceInfoDesc>();
            _serviceRefEx = new ReadOnlyListOnIList<ServiceReferenceInfoDesc>( _serviceRef ); 
        }

        public Guid PluginId { get; set; }

        public string PluginFullName { get; private set; }

        public PluginInfoDesc AddRef( string serviceName, RunningRequirement r )
        {
            _serviceRef.Add( new ServiceReferenceInfoDesc( this, _disco.Services[serviceName], r ) );
            return this;
        }

        IReadOnlyList<IServiceReferenceInfo> IPluginInfo.ServiceReferences
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
                    _service = (ServiceInfoDesc)value;
                    if( _service != null ) _service._plugins.Add( this );
                }
            } 
        }

        public override string ToString()
        {
            return String.Format( "Plugin: {0}", PluginFullName );
        }

        IReadOnlyList<IPluginConfigAccessorInfo> IPluginInfo.EditorsInfo { get { return ReadOnlyListEmpty<IPluginConfigAccessorInfo>.Empty; } }

        IReadOnlyList<IPluginConfigAccessorInfo> IPluginInfo.EditableBy { get { return ReadOnlyListEmpty<IPluginConfigAccessorInfo>.Empty; } }

        string IPluginInfo.Description { get { return null; } }

        bool IPluginInfo.IsOldVersion { get { return false; } }

        Uri IPluginInfo.RefUrl { get { return null; } }

        IReadOnlyList<string> IPluginInfo.Categories { get { return ReadOnlyListEmpty<string>.Empty; } }

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
