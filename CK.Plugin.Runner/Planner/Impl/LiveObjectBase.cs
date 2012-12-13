using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Diagnostics;

namespace CK.Plugin.Hosting
{
    internal class LiveObjectBase : INotifyPropertyChanged
    {
        RunningRequirement _configRequirement;
        RunningStatus _status;


        protected LiveObjectBase( RunningRequirement configRequirement, RunningStatus status )
        {
            _configRequirement = configRequirement;
            _status = status;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged( string propertyName )
        {
            Debug.Assert( GetType().GetProperty( propertyName ) != null );
            var h = PropertyChanged;
            if( h != null ) h( this, new PropertyChangedEventArgs( propertyName ) );
        }

        public RunningRequirement ConfigRequirement
        {
            get { return _configRequirement; }
            set
            {
                if( _configRequirement != value )
                {
                    _configRequirement = value;
                    OnPropertyChanged( "ConfigRequirement" );
                }
            }
        }

        public RunningStatus Status
        {
            get { return _status; }
            set
            {
                if( _status != value )
                {
                    bool wasRunning = IsRunning;
                    _status = value;
                    OnPropertyChanged( "Status" );
                    if( wasRunning != IsRunning ) OnPropertyChanged( "IsRunning" );
                }
            }
        }

        public bool IsRunning
        {
            get { return _status >= RunningStatus.Running; }
        }


    }
}
