using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CK.Plugin.Hosting
{
    public interface ILiveServiceInfo : INotifyPropertyChanged
    {
        IServiceInfo ServiceInfo { get; }

        RunningRequirement ConfigRequirement { get; }
        
        RunningStatus Status { get; }

        bool IsRunning { get; }

        ILiveServiceInfo Generalization { get; }

        ILivePluginInfo RunningPlugin { get; }

    }
}
