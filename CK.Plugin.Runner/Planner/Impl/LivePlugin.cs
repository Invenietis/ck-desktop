using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;

namespace CK.Plugin.Hosting
{
    class LivePlugin : LiveObjectBase, ILivePluginInfo, IComparable<LivePlugin>
    {
        readonly IPluginInfo _pluginInfo;
        ILiveServiceInfo _service;

        internal LivePlugin( IPluginInfo p, RunningRequirement configRequirement, ILiveServiceInfo service, RunningStatus status )
            : base( configRequirement, status )
        {
            _pluginInfo = p;
            _service = service;
        }

        public IPluginInfo PluginInfo
        {
            get { return _pluginInfo; }
        }

        public ILiveServiceInfo Service
        {
            get { return _service; }
            set
            {
                if( _service != value )
                {
                    _service = value;
                    OnPropertyChanged( "Service" );
                }
            }
        }

        int IComparable<LivePlugin>.CompareTo( LivePlugin other )
        {
            return _pluginInfo.PluginId.CompareTo( other._pluginInfo.PluginId );
        }

    }
}
