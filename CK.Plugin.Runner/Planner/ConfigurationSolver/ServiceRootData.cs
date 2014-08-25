#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Plugin.Runner\Planner\ConfigurationSolver\ServiceRootData.cs) is part of CiviKey. 
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
using System.Diagnostics;

namespace CK.Plugin.Hosting
{
    partial class ServiceRootData : ServiceData
    {
        ServiceData _mustExistService;
        PluginData _mustExistPluginByConfig;

        internal ServiceRootData( Dictionary<IServiceInfo, ServiceData> allServices, IServiceInfo s, SolvedConfigStatus serviceStatus, Func<IServiceInfo,bool> isExternalServiceAvailable )
            : base( allServices, s, null, serviceStatus, isExternalServiceAvailable )
        {
        }

        public ServiceData MustExistService
        {
            get { return _mustExistService; }
        }

        public PluginData TheOnlyPlugin
        {
            get { return _theOnlyPlugin; }
        }

        public PluginData MustExistPluginByConfig
        {
            get { return _mustExistPluginByConfig; }
        }

        internal void InitializeMustExistService()
        {
            Debug.Assert( !Disabled );
            _mustExistService = GetMustExistService();
            if( _mustExistService == null && ServiceSolvedStatus >= SolvedConfigStatus.MustExist ) _mustExistService = this;
        }

        internal override void OnAllPluginsAdded()
        {
            Debug.Assert( !Disabled );
            base.OnAllPluginsAdded();
            if( !Disabled && _mustExistPluginByConfig != null )
            {
                _mustExistPluginByConfig.Service.SetAsMustExistService( fromMustExistPlugin: true );
            }
        }

        internal override void SetDisabled( ServiceDisabledReason r )
        {
            base.SetDisabled( r );
            _mustExistService = null;
            _mustExistPluginByConfig = null;
        }

        internal void MustExistServiceChanged( ServiceData s )
        {
            Debug.Assert( !Disabled );
            _mustExistService = s;
        }

        /// <summary>
        /// Called by ServiceData.PluginData during plugin registration.
        /// This does not immediatly call ServiceData.SetAsMustExistService() in order to offer PluginDisabledReason.AnotherPluginAlreadyExistForTheSameService reason
        /// rather than PluginDisabledReason.ServiceSpecializationMustExist for next conflicting plugins.
        /// </summary>
        internal void SetMustExistPluginByConfig( PluginData p )
        {
            Debug.Assert( !Disabled );
            Debug.Assert( p.MinimalRunningRequirement >= RunningRequirement.MustExist );
            Debug.Assert( p.Service == null || p.Service.GeneralizationRoot == this, "When called from PluginData ctor, Service is not yet set." );
            if( _mustExistPluginByConfig == null )
            {
                _mustExistPluginByConfig = p;
            }
            else
            {
                p.SetDisabled( PluginDisabledReason.AnotherPluginAlreadyExistForTheSameService );
            }
        }
    }
}
