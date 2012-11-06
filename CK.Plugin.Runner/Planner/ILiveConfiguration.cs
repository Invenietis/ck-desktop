using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CK.Core;

namespace CK.Plugin.Hosting
{
    public interface ILiveConfiguration
    {
        ILivePluginInfo FindPlugin( IPluginInfo p );

        IObservableReadOnlyCollection<ILivePluginInfo> Plugins { get; }

        ILiveServiceInfo FindService( IServiceInfo p );

        IObservableReadOnlyCollection<ILiveServiceInfo> Services { get; }

    }
}
