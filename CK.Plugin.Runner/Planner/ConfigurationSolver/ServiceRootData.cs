﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CK.Core;

namespace CK.Plugin.Hosting
{
    partial class ServiceRootData : ServiceData
    {
        ServiceData _mustExistService;
        PluginData _mustExistPluginByConfig;

        internal ServiceRootData( Dictionary<IServiceInfo, ServiceData> allServices, IServiceInfo s, SolvedConfigStatus serviceStatus, Func<IServiceInfo,bool> isExternalServiceAvailable, IActivityMonitor monitor = null )
            : base( allServices, s, null, serviceStatus, isExternalServiceAvailable, monitor )
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
