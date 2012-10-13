using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CK.Plugin;
using CK.Tests.Plugin;

namespace CK.Tests.Plugin
{
    public interface IReferenceServiceC : IDynamicService
    {
        bool ServiceCIsNotNull { get; }
    }

    public abstract class PluginBase : IPlugin, IReferenceServiceC
    {
        public bool ServiceCIsNotNull { get; private set; }

        public bool Setup( IPluginSetupInfo info )
        {
            return true;
        }

        public void Start()
        {
            IServiceC s = GetServiceC();
            ServiceCIsNotNull = s != null;
        }

        public void Teardown()
        {
        }

        public void Stop()
        {
        }

        protected abstract IServiceC GetServiceC();
    }
}
