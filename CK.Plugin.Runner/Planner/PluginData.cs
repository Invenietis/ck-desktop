using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CK.Plugin.Hosting
{
    class PluginData
    {
        readonly Dictionary<IServiceInfo,ServiceData> _allServices;
        PluginDisabledReason _disabledReason;
        RunningRequirement _runningRequirement;
        PluginRunningRequirementReason _runningRequirementReason;

        internal PluginData( Dictionary<IServiceInfo,ServiceData> allServices, IPluginInfo p, ServiceData service, bool isRunning, SolvedConfigStatus pluginStatus )
        {
            _allServices = allServices;
            PluginInfo = p;
            IsRunning = isRunning;
            // Updates disabled state first so that AddPlugin can take disabled state into account.
            if( (PluginSolvedStatus = pluginStatus) == SolvedConfigStatus.Disabled )
            {
                _disabledReason = PluginDisabledReason.Config;
            }
            else if( p.HasError )
            {
                _disabledReason = PluginDisabledReason.PluginInfoHasError;
            }
            else if( service != null )
            {
                if( service.Disabled )
                {
                    _disabledReason = PluginDisabledReason.ServiceIsDisabled;
                }
                else if( service.MustExistSpecialization != null )
                {
                    _disabledReason = PluginDisabledReason.ServiceSpecializationMustExist;
                }
            }
            // Register MustExist references to Services from this plugin.
            foreach( var sRef in PluginInfo.ServiceReferences )
            {
                if( sRef.Requirements >= RunningRequirement.MustExist )
                {
                    // If the required service is already disabled, we immediately disable this plugin.
                    // If the required service is not yet disabled, we register this plugin data:
                    // whenever the service is disabled, it will disable the plugin.
                    ServiceData sr = allServices[sRef.Reference];
                    if( sr.Disabled )
                    {
                        SetDisabled( PluginDisabledReason.MustExistReferenceIsDisabled );
                        break;
                    }
                    sr.AddMustExistReferencer( this );
                }
            }
            // Updates RunningRequirement so that AddPlugin can take MustExist into account.
            _runningRequirementReason = PluginRunningRequirementReason.Config;
            if( !Disabled ) _runningRequirement = (RunningRequirement)pluginStatus;
            if( service != null )
            {
                service.AddPlugin( this );
                // Sets Service after AddPlugin call to avoid calling Service.OnPluginDisabled 
                // if the AddPlugin or references checks above disables it.
                Service = service;
            }
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
        public RunningRequirement MinimalRunningRequirement
        {
            get { return _runningRequirement; }
        }

        /// <summary>
        /// Gets the strongest reason that explains this plugin MinimalRunningRequirement. 
        /// </summary>
        public PluginRunningRequirementReason MinimalRunningRequirementReason
        {
            get { return _runningRequirementReason; }
        }
        
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

        internal bool SetRunningRequirement( RunningRequirement r, PluginRunningRequirementReason reason )
        {
            if( r <= _runningRequirement )
            {
                if( r >= RunningRequirement.MustExist ) return !Disabled;
                return true;
            }
            // New requirement is stronger than the previous one.
            // Is it compliant with a Disabled plugin? If yes, it is always satisfied.
            if( r < RunningRequirement.MustExist )
            {
                // The new requirement is OptionalTryStart.
                // This can always be satisfied.
                _runningRequirement = r;
                _runningRequirementReason = reason;
                return true;
            }
            // The new requirement is at least MustExist.
            // If this is already disabled, there is nothing to do.
            if( Disabled ) return false;

            _runningRequirement = r;
            _runningRequirementReason = reason;

            return CheckReferencesWhenMustExist();
        }

        internal bool CheckReferencesWhenMustExist()
        {
            Debug.Assert( !Disabled && _runningRequirement >= RunningRequirement.MustExist );
            foreach( var sRef in PluginInfo.ServiceReferences )
            {
                RunningRequirement propagation = sRef.Requirements;
                if( _runningRequirement < propagation ) propagation = _runningRequirement;

                ServiceData sr = _allServices[sRef.Reference];
                if( !sr.SetRunningRequirement( _runningRequirement, ServiceRunningRequirementReason.FromMustExistReference ) )
                {
                    if( !Disabled ) SetDisabled( PluginDisabledReason.RequirementPropagationToReferenceFailed );
                    break;
                }
            }
            return !Disabled;
        }
    }

}
