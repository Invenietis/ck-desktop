#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Plugin.Config\UserAndSystemConfig\LiveUserConfigurationChangingEventArgs.cs) is part of CiviKey. 
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
using CK.Core;
using System.ComponentModel;
using CK.Plugin.Config.Model;

namespace CK.Plugin.Config
{
    public class LiveUserConfigurationChangingEventArgs : CancelEventArgs
    {
        public ChangeStatus ChangeAction { get; private set; }

        public Guid PluginID { get; private set; }

        public ConfigUserAction Action { get; private set; }

        public LiveUserConfigurationChangingEventArgs( ChangeStatus changeAction, Guid pluginID, ConfigUserAction action )
        {
            ChangeAction = changeAction;
            PluginID = pluginID;
            Action = action;
        }
    }
}
