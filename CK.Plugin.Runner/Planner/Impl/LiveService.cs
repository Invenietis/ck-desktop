#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Plugin.Runner\Planner\Impl\LiveService.cs) is part of CiviKey. 
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
