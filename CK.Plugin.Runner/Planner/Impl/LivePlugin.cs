#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Plugin.Runner\Planner\Impl\LivePlugin.cs) is part of CiviKey. 
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
