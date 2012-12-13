using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;

namespace CK.Plugin.Hosting
{
    public interface ILivePluginInfo : INotifyPropertyChanged
    {
        IPluginInfo PluginInfo { get; }

        RunningRequirement ConfigRequirement { get; }
        
        RunningStatus Status { get; }

        bool IsRunning { get; }

        ILiveServiceInfo Service { get; }
    }
}
