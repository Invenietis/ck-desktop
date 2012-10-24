using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CK.Plugin.Hosting
{
    enum PluginDisabledReason
    {
        None,
        Config,
        ServiceIsDisabled,
        MustExistReferenceIsDisabled,
        ServicePluginsAreDisabledSinceSpecializationMustExist
    }

    enum PluginRunningRequirementReason
    {
        Config,
        FromServiceConfigToSingleImplementation,
        FromServiceToSingleImplementation
    }


    class PluginData
    {
        PluginDisabledReason _disabledReason;
        PluginRunningRequirementReason _runningRequirementReason;

        internal PluginData( Dictionary<IServiceInfo,ServiceData> allServices, IPluginInfo p, ServiceData service, bool isRunning, SolvedConfigStatus pluginStatus )
        {
            PluginInfo = p;
            IsRunning = isRunning;
            Service = service;
            // Updates disabled state first so that AddPlugin can take disabled state into account.
            if( (PluginSolvedStatus = pluginStatus) == SolvedConfigStatus.Disabled )
            {
                _disabledReason = PluginDisabledReason.Config;
            }
            else if( service != null )
            {
                if( service.Disabled )
                {
                    _disabledReason = PluginDisabledReason.ServiceIsDisabled;
                }
                else if( service.SpecializationMustExist )
                {
                    _disabledReason = PluginDisabledReason.ServicePluginsAreDisabledSinceSpecializationMustExist;
                }
            }
            if( service != null )
            {
                service.AddPlugin( this );
            }
            if( !Disabled )
            {               
                MinimalRunningRequirement = (RunningRequirement)pluginStatus;
            }
            _runningRequirementReason = PluginRunningRequirementReason.Config;
        }

        public readonly IPluginInfo PluginInfo;

        /// <summary>
        /// The ServiceData if this plugin implements a Service, Null otherwise.
        /// </summary>
        public readonly ServiceData Service;

        /// <summary>
        /// True if this plunning is currently running. This is used to impact cost (starting a stopped plugin costs a little bit more 
        /// than keeping a running plugin that satisfies the same condition).
        /// </summary>
        public readonly bool IsRunning;
        
        /// <summary>
        /// The SolvedConfigStatus of the plugin itself.
        /// </summary>
        public readonly SolvedConfigStatus PluginSolvedStatus;

        /// <summary>
        /// Gets whether this plugin must exist or run. It is initialized by the configuration, but may evolve
        /// if this plugin is the only one available for a service and that the service must exist or run because a must exist or run
        /// plugin references the service with an "optional try start", "must exist" or "must exist and run" requirement.
        /// </summary>
        public RunningRequirement MinimalRunningRequirement { get; private set; }

        /// <summary>
        /// Gets the strongest reason that explains this plugin MinimalRunningRequirement. 
        /// </summary>
        public PluginRunningRequirementReason MinimalRunningRequirementReason
        {
            get { return _runningRequirementReason; }
        }
        
        /// <summary>
        /// Link to the previous element in the list of sibling PluginData that implement the same Service.
        /// </summary>
        public PluginData PrevPluginForService;
        
        /// <summary>
        /// Link to the next element in the list of sibling PluginData that implement the same Service.
        /// </summary>
        public PluginData NextPluginForService;

        /// <summary>
        /// Gets whether this service is disabled. 
        /// </summary>
        public bool Disabled
        {
            get { return _disabledReason == PluginDisabledReason.None; }
        }

        /// <summary>
        /// Gets the first reason why this plugin is disabled. 
        /// </summary>
        public PluginDisabledReason DisabledReason
        {
            get { return _disabledReason; }
        }

        internal void SetDisabled( PluginDisabledReason r )
        {
            Debug.Assert( r != PluginDisabledReason.None );
            Debug.Assert( _disabledReason == PluginDisabledReason.None );
            _disabledReason = r;
            if( Service != null ) Service.OnPluginDisabled( this );
        }

        internal void SetRunningRequirement( RunningRequirement r, PluginRunningRequirementReason reason )
        {
            // This can be called on an already disabled plugin.
            Debug.Assert( r > MinimalRunningRequirement );
            MinimalRunningRequirement = r;
            _runningRequirementReason = reason;
        }

        internal void BindServiceReferencesAndCheckDirectMustExist( Dictionary<IServiceInfo, ServiceData> services )
        {
            Debug.Assert( !Disabled );
            foreach( var sRef in PluginInfo.ServiceReferences )
            {
                if( sRef.Requirements >= RunningRequirement.MustExist )
                {
                    // If the required service is already disabled, we immediately disable this plugin
                    // and we can leave.
                    // If the required service is not yet disabled, we register this plugin data:
                    // whenever the service is disabled, it will disable the plugin.
                    ServiceData sr = services[sRef.Reference];
                    if( sr.Disabled )
                    {
                        SetDisabled( PluginDisabledReason.MustExistReferenceIsDisabled );
                        return;
                    }
                    sr.AddMustExistReferencer( this );
                }
            }
        }

        internal void PropagateMinimalRunningRequirement( Dictionary<IServiceInfo, ServiceData> services )
        {
            Debug.Assert( !Disabled && MinimalRunningRequirement >= RunningRequirement.MustExist );
            foreach( var sRef in PluginInfo.ServiceReferences )
            {
                if( sRef.Requirements >= RunningRequirement.OptionalTryStart )
                {
                    ServiceData sr = services[sRef.Reference];
                    sr.PropagateMinimalRunningRequirementFrom( this, sRef.Requirements, services );
                    // If the propagation on the service triggered a disabling of one our MustExist references, this 
                    // plugin may become disabled.
                    // If it is the case, we stop to avoid working on a disabled plugin.
                    if( Disabled ) return;
                }
            }
        }

        internal void PropagateMinimalRunningRequirementFromServiceToSingleImplementation( RunningRequirement req, Dictionary<IServiceInfo, ServiceData> services )
        {
            Debug.Assert( !Disabled && Service != null );
            if( req < MinimalRunningRequirement ) return;
            SetRunningRequirement( req, PluginRunningRequirementReason.FromServiceToSingleImplementation );
            if( req >= RunningRequirement.MustExist ) PropagateMinimalRunningRequirement( services );
        }
    }

}
