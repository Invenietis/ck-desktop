#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Plugin.Runner\Planner\Impl\LiveObjectBase.cs) is part of CiviKey. 
*  
* CiviKey is free software: you can redistribute it and/or modify 
* it under the terms of the GNU Lesser General Public License as published 
* by the Free Software Foundation, either version 3 of the License, or 
* (at your option) any later version. 
*  
* CiviKey is distributed in the hope that it will be useful, 
* but WITHOUT ANY WARRANTY; without even the implied warranty of
* MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the 
* GNU Lesser General Public License for more details. 
* You should have received a copy of the GNU Lesser General Public License 
* along with CiviKey.  If not, see <http://www.gnu.org/licenses/>. 
*  
* Copyright © 2007-2014, 
*     Invenietis <http://www.invenietis.com>,
*     In’Tech INFO <http://www.intechinfo.fr>,
* All rights reserved. 
*-----------------------------------------------------------------------------*/
#endregion

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
