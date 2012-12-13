using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CK.Plugin.Runner.Tests.Planner
{
    public class ServiceReferenceInfoStub : IServiceReferenceInfo
    {
        PluginInfoStub _plugin;
        ServiceInfoStub _service;

        internal ServiceReferenceInfoStub( PluginInfoStub p, ServiceInfoStub s, RunningRequirement r )
        {
            _plugin = p;
            _service = s;
            Requirements = r;
        }

        public IPluginInfo Owner
        {
            get { return _plugin; }
        }

        public IServiceInfo Reference
        {
            get { return _service; }
        }


        public override string ToString()
        {
            return String.Format( "{0} {1} {2}", _plugin, Requirements, _service );
        }

        public string PropertyName { get; set; }

        public bool IsIServiceWrapped { get; set; }

        public RunningRequirement Requirements { get; set; }

        public bool HasError { get; set; }
        
        public string ErrorMessage { get; set; }
    }
}
