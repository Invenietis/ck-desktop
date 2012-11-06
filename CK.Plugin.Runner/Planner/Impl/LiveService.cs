using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;

namespace CK.Plugin.Hosting
{
    class LiveService : LiveObjectBase, ILiveServiceInfo, IComparable<LiveService>
    {
        IServiceInfo _serviceInfo;
        ILiveServiceInfo _generalization;
        ILivePluginInfo _runningPlugin;

        internal LiveService( IServiceInfo serviceInfo, RunningRequirement configRequirement, ILiveServiceInfo generalization, ILivePluginInfo runningPlugin, RunningStatus status )
            : base( configRequirement, status )
        {
            _serviceInfo = serviceInfo;
            _generalization = generalization;
            _runningPlugin = runningPlugin;
        }

        public IServiceInfo ServiceInfo
        {
            get { return _serviceInfo; }
        }

        public ILiveServiceInfo Generalization
        {
            get { return _generalization; }
            set
            {
                if( _generalization != value )
                {
                    _generalization = value;
                    OnPropertyChanged( "Generalization" );
                }
            }
        }

        public ILivePluginInfo RunningPlugin
        {
            get { return _runningPlugin; }
            set
            {
                if( _runningPlugin != value )
                {
                    _runningPlugin = value;
                    OnPropertyChanged( "RunningPlugin" );
                }
            }
        }

        int IComparable<LiveService>.CompareTo( LiveService other )
        {
            return _serviceInfo.CompareTo( other._serviceInfo );
        }
    }
}
