using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CK.Plugin.Runner.Tests.Planner
{
    public class ServiceReferenceInfoDesc : IServiceReferenceInfo
    {
        PluginInfoDesc _plugin;
        ServiceInfoDesc _service;

        internal ServiceReferenceInfoDesc( PluginInfoDesc p, ServiceInfoDesc s, RunningRequirement r )
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

        public string PropertyName { get; set; }

        public bool IsIServiceWrapped { get; set; }

        public RunningRequirement Requirements { get; set; }

        public bool HasError { get; set; }
        
        public string ErrorMessage { get; set; }
    }
}
