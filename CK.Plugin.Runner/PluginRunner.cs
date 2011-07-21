﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CK.Plugin.Discoverer;
using System.Reflection;
using CK.Plugin.Config;
using CK.Core;
using System.Diagnostics;

namespace CK.Plugin.Hosting
{

    public partial class PluginRunner : ISimplePluginRunner
    {
        PluginDiscoverer _discoverer;
        RunnerRequirements _requirements;
        RunningConfiguration _runningConfig;       
        PluginHost _host;
        IServiceProvider _externalServiceProvider;
        object _contextObject;
        PlanCalculator _planCalculator;

        public event EventHandler ApplyDone;

        public PluginRunner( IServiceProvider externalServiceProvider, IConfigManager cfg )
        {
            _externalServiceProvider = externalServiceProvider;
            _config = cfg;
            _configAccessors = new Dictionary<INamedVersionedUniqueId, PluginConfigAccessor>();
            _config.Extended.Container.Changed += OnConfigContainerChanged;
            _discoverer = new PluginDiscoverer();
            _runningConfig = new RunningConfiguration( this );
            _requirements = new RunnerRequirements( this );

            _host = new PluginHost();
            // 0 - For creation.
            _host.PluginCreator = CreatePlugin;
            // 1 - Before Setup call: plugin is configured.
            _host.PluginConfigurator = ConfigurePlugin;
            // 2 - Before Start: plugin is aware of its environment.
            _host.ServiceReferencesBinder = ConfigureServiceReferences;
        }

        public void Initialize( object contextObject )
        {
            _contextObject = contextObject;
            _requirements.Initialize();
            _runningConfig.Initialize();
        }

        public bool Disabled
        {
            get { return _requirements.RunnerDisabled; }
            set { _requirements.RunnerDisabled = value; }
        }

        public IPluginDiscoverer Discoverer
        {
            get { return _discoverer; }
        }

        public IPluginHost PluginHost
        {
            get { return _host; }
        }

        public IServiceHost ServiceHost
        {
            get { return _host.ServiceHost; }
        }

        public ILogCenter LogCenter
        {
            get { return _host.LogCenter; }
        }

        public RunnerRequirements RunnerRequirements
        {
            get { return _requirements; }
        }

        public RunningConfiguration RunningConfiguration
        {
            get { return _runningConfig; }
        }

        public bool IsPluginRunning( IPluginInfo pluginInfo )
        {
            return _host.IsPluginRunning( pluginInfo );
        }
       
        public bool Apply()
        {
            if( _planCalculator != null ) throw new InvalidOperationException( Runner.R.ReentrantApplyCall );
            bool errorWhileApplying = false;
            if( _runningConfig.IsDirty )
            {
                // Allocates a new PlanCalculator and reuses it as long as reapplying is needed.
                _planCalculator = new PlanCalculator( _discoverer, _host.IsPluginRunning );
                try
                {
                    do
                    {
                        RunnerRequirementsSnapshot requirements = new RunnerRequirementsSnapshot( _requirements );
                        SolvedPluginConfigurationSnapshot configSnapshot = new SolvedPluginConfigurationSnapshot( _config.SolvedPluginConfiguration );

                        // During call to ObtainBestPlan, no reentrancy can occur (ObtainBestPlan does not call
                        // any external functions or objects nor does it raise any event).
                        // Once obtained, the best plan is also available through _planCalculator.LastBestPlan property.
                        ExecutionPlan bestPlan = _planCalculator.ObtainBestPlan( requirements.FinalConfigSnapshot );
                        if( bestPlan.Impossible )
                        {
                            errorWhileApplying = true;
                        }
                        else
                        {
                            // Here is where rentrancy may occur.
                            // Starting/stopping any plugin may start/stop others or enable/disable this runner.
                            var result = _host.Execute( bestPlan.PluginsToDisable, bestPlan.PluginsToStop, bestPlan.PluginsToStart );
                            if( result.Status != ExecutionPlanResultStatus.Success )
                            {
                                if( result.Culprit != null ) _requirements.SetRunningError( result );
                                errorWhileApplying = true;
                            }
                            else
                            {
                                _planCalculator.ReapplyNeeded = false;
                                _requirements.UpdateRunningStatus( requirements.FinalConfigSnapshot );
                                _runningConfig.Apply( configSnapshot, requirements );
                            }
                        }
                    }
                    while( _planCalculator.ReapplyNeeded && !errorWhileApplying );
                    
                    if( ApplyDone != null ) ApplyDone( this, EventArgs.Empty );
                }
                finally
                {
                    _planCalculator = null;
                }                
            }
            return !errorWhileApplying;
        }

        IPlugin CreatePlugin( IPluginInfo info )
        {
            Assembly a = Assembly.Load( info.AssemblyInfo.AssemblyName );
            Type t = a.GetType( info.PluginFullName, true );
            var cSP = t.GetConstructor( new Type[] { typeof( IServiceProvider ) } );
            if( cSP != null ) return (IPlugin)cSP.Invoke( new object[] { _contextObject } );
            return (IPlugin)Activator.CreateInstance( t );
        }

        void ConfigurePlugin( IPluginProxy p )
        {
            _config.Extended.Container.Ensure( p );
            ConfigureConfigAccessors( p );
        }

        void ConfigureServiceReferences( IReadOnlyCollection<IPluginProxy> newPluginsLoaded )
        {            
            foreach( var p in newPluginsLoaded )
            {
                HashSet<PropertyInfo> processedProperties = new HashSet<PropertyInfo>();
                Type pType = p.RealPluginObject.GetType();

                foreach( IServiceReferenceInfo r in p.PluginKey.ServiceReferences )
                {
                    PropertyInfo pService = pType.GetProperty( r.PropertyName );
                    processedProperties.Add( pService );
                    if( r.Reference.IsDynamicService )
                    {
                        object refService = _host.ServiceHost.GetProxy( pService.PropertyType );
                        pService.SetValue( p.RealPluginObject, refService, null );
                    }
                    else
                    {
                        InjectExternalService( pService, p.RealPluginObject );
                    }
                }

                foreach( PropertyInfo prop in pType.GetProperties().Except( processedProperties ) )
                {
                    foreach( CustomAttributeData attr in CustomAttributeData.GetCustomAttributes( prop ) )
                    {
                        if( attr.Constructor.DeclaringType.FullName == typeof( RequiredServiceAttribute ).FullName )
                            InjectExternalService( prop, p.RealPluginObject );
                    }
                }
            }
        }

        void InjectExternalService( PropertyInfo property, object obj )
        {
            object refService = _externalServiceProvider.GetService( property.PropertyType );
            property.SetValue( obj, refService, null );
        }

        public event EventHandler  IsDirtyChanged;

        public bool IsDirty { get; private set; }

        internal void SetDirty( bool dirty )
        {
            if( _planCalculator != null && dirty )
            {
                _planCalculator.ReapplyNeeded = true;
            }
            else
            {
                if( IsDirty != dirty )
                {
                    IsDirty = dirty;
                    if( IsDirtyChanged != null ) IsDirtyChanged( this, EventArgs.Empty );
                }
            }
        }

        #region ISimplePluginRunner Members

        bool ISimplePluginRunner.Add( RequirementLayer r, bool allowDuplicate )
        {
            return _requirements.Add( r, allowDuplicate );
        }

        bool ISimplePluginRunner.Remove( RequirementLayer r, bool removeAll )
        {
            return _requirements.Remove( r, removeAll );
        }

        #endregion
    }
}
