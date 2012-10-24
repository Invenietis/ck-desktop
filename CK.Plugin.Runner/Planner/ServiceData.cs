﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CK.Plugin.Hosting
{
    enum ServiceDisabledReason
    {
        None,

        /// <summary>
        /// Initialized by ServiceData constructor.
        /// </summary>
        Config,
        
        /// <summary>
        /// Initialized by ServiceData constructor.
        /// </summary>
        ServiceInfoHasError,
        
        /// <summary>
        /// Initialized by ServiceData constructor.
        /// </summary>
        GeneralizationIsDisabledByConfig,
        
        /// <summary>
        /// Sets by ServiceData.SetRunningRequirement method.
        /// </summary>
        RequirementPropagationToSingleImplementationFailed,

        MultipleSpecializationsMustExistByConfig,
        AnotherSpecializationMustExistByConfig,
        AnotherSpecializationHasPluginThatMustExistByConfig,
        DirectGeneralizationHasPluginThatMustExistByConfig,
        GeneralizationIsDisabled,
        AnotherSpecializationMustExist,
        MustExistSpecializationIsDisabled,
        NoPlugin,
        AllPluginsAreDisabled,
        MultiplePluginsMustExistByConfig,
    }

    enum ServiceRunningRequirementReason
    {
        /// <summary>
        /// Initialized by ServiceData constructor.
        /// </summary>
        Config,
        FromSingleImplementationConfig,
        FromMustExistReference,
        FromSpecialization,
        FromGeneralization,
    }
    
    internal class ServiceData
    {
        ServiceDisabledReason _disabledReason;
        RunningRequirement _runningRequirement;
        ServiceRunningRequirementReason _runningRequirementReason;
        ServiceData _mustExistSpecialization;
        ServiceData _directMustExistSpecialization;
        // This is internal for ServiceRootData to expose it.
        internal PluginData _theOnlyPlugin;
        List<PluginData> _mustExistReferencer;
        
        internal ServiceData( IServiceInfo s, ServiceData generalization, SolvedConfigStatus serviceStatus )
        {
            ServiceInfo = s;
            if( generalization != null )
            {
                GeneralizationRoot = generalization.GeneralizationRoot;
                NextSpecialization = Generalization.FirstSpecialization;
                Generalization.FirstSpecialization = this;
                ++Generalization.SpecializationCount;
            }
            else
            {
                GeneralizationRoot = (ServiceRootData)this;
                Generalization = generalization;
            }
            if( (ServiceSolvedStatus = serviceStatus) == SolvedConfigStatus.Disabled )
            {
                _disabledReason = ServiceDisabledReason.Config;
            }
            else if( s.HasError )
            {
                _disabledReason = ServiceDisabledReason.ServiceInfoHasError;
            }
            else if( Generalization != null && Generalization.Disabled )
            {
                _disabledReason = ServiceDisabledReason.GeneralizationIsDisabledByConfig;
            }
            if( !Disabled ) _runningRequirement = (RunningRequirement)serviceStatus;
            _runningRequirementReason = ServiceRunningRequirementReason.Config;
        }

        public readonly IServiceInfo ServiceInfo;

        /// <summary>
        /// The direct generalization if any.
        /// When null, this instance is a <see cref="ServiceRootData"/>.
        /// </summary>
        public readonly ServiceData Generalization;

        /// <summary>
        /// Root of Generalization. Never null since when this is not a specialization, this is its own root.
        /// </summary>
        public readonly ServiceRootData GeneralizationRoot;

        /// <summary>
        /// The SolvedConfigStatus of the Service. 
        /// </summary>
        public readonly SolvedConfigStatus ServiceSolvedStatus;

        /// <summary>
        /// Gets whether this service is disabled. 
        /// </summary>
        public bool Disabled
        {
            get { return _disabledReason == ServiceDisabledReason.None; }
        }

        public ServiceData MustExistSpecialization
        {
            get { return _mustExistSpecialization; }
        }

        public bool IsGeneralizationOf( ServiceData d )
        {
            var g = d.Generalization;
            if( g == null || d.GeneralizationRoot != GeneralizationRoot ) return false;
            do
            {
                if( g == this ) return true;
                g = g.Generalization;
            }
            while( g != null );
            return false;
        }

        /// <summary>
        /// Gets the first reason why this service is disabled. 
        /// </summary>
        public ServiceDisabledReason DisabledReason
        {
            get { return _disabledReason; }
        }

        internal virtual void SetDisabled( ServiceDisabledReason r )
        {
            Debug.Assert( r != ServiceDisabledReason.None );
            Debug.Assert( _disabledReason == ServiceDisabledReason.None );
            Debug.Assert( !GeneralizationRoot.Disabled, "A root is necessarily not disabled if one of its specialization is not disabled." );
            _disabledReason = r;
            ServiceData spec = FirstSpecialization;
            while( spec != null )
            {
                if( !spec.Disabled ) spec.SetDisabled( ServiceDisabledReason.GeneralizationIsDisabled );
                spec = spec.NextSpecialization;
            }
            PluginData plugin = FirstPlugin;
            while( plugin != null )
            {
                if( !plugin.Disabled ) plugin.SetDisabled( PluginDisabledReason.ServiceIsDisabled );
                plugin = plugin.NextPluginForService;
            }
            Debug.Assert( _theOnlyPlugin == null, "Disabling all plugins must have set it to null." );
            // The _mustExistReferencer list contains plugins that has at least a MustExist reference to this service
            // and have been initialized when this Service was not yet disabled.
            if( _mustExistReferencer != null )
            {
                foreach( PluginData p in _mustExistReferencer )
                {
                    if( !p.Disabled ) p.SetDisabled( PluginDisabledReason.MustExistReferenceIsDisabled );
                }
                // It is useless to keep them.
                _mustExistReferencer = null;
            }
            _directMustExistSpecialization = null;
            _mustExistSpecialization = null;
            if( Generalization != null && !Generalization.Disabled ) Generalization.OnSpecializationDisabled( this );
        }

        void OnSpecializationDisabled( ServiceData s )
        {
            if( _directMustExistSpecialization == s )
            {
                SetDisabled( ServiceDisabledReason.MustExistSpecializationIsDisabled );
            }
        }

        /// <summary>
        /// Gets the minimal running requirement. It is initialized by the configuration, but may evolve.
        /// </summary>
        public RunningRequirement MinimalRunningRequirement
        {
            get { return _mustExistSpecialization != null ? _mustExistSpecialization._runningRequirement : _runningRequirement; }
        }

        /// <summary>
        /// Gets the minimal running requirement for this service (not the one of MustExistSpecialization if it exists).
        /// </summary>
        public RunningRequirement ThisMinimalRunningRequirement
        {
            get { return _runningRequirement; }
        }

        /// <summary>
        /// Gets the strongest reason that explains this service ThisMinimalRunningRequirement. 
        /// </summary>
        public ServiceRunningRequirementReason ThisRunningRequirementReason 
        {
            get { return _runningRequirementReason; }
        }

        /// <summary>
        /// This can be called on an already disabled service and may trigger changes on the whole system.
        /// </summary>
        /// <param name="r"></param>
        /// <param name="reason"></param>
        /// <returns>True if the requirement can be satisfied at this level. False otherwise.</returns>
        internal bool SetRunningRequirement( RunningRequirement r, ServiceRunningRequirementReason reason )
        {
            if( _mustExistSpecialization != null )
            {
                return _mustExistSpecialization.SetRunningRequirement( r, reason );
            }
            if( r <= _runningRequirement )
            {
                if( r >= RunningRequirement.MustExist ) return !Disabled;
                return true;
            }
            // New requirement is stronger than the previous one.
            // Is it compliant with a Disabled service? If yes, it is always satisfied.
            if( r < RunningRequirement.MustExist )
            {
                // The new requirement is OptionalTryStart.
                // This can always be satisfied.
                _runningRequirement = r;
                _runningRequirementReason = reason;
                // Propagate TryStart to the only plugin if it exists.
                if( _theOnlyPlugin != null ) _theOnlyPlugin.SetRunningRequirement( r, PluginRunningRequirementReason.FromServiceToSingleImplementation );
                return true;
            }
            // The new requirement is at least MustExist.
            // If this is already disabled, there is nothing to do.
            if( Disabled ) return false;

            // This service is not yet disabled. We now try to honor the MustExist requirement at the service level.
            // If we fail, this service will be disabled, but we set the requirement to prevent reentrancy.
            // Reentrancy can nevertheless be caused by subsequent requirements MustExistTryStart or MustExistAndRun:
            // we allow this (there will be at most 3 reentrant calls to this method). 
            var current = _runningRequirement;
            _runningRequirement = r;
            _runningRequirementReason = reason;
            if( current < RunningRequirement.MustExist )
            {
                // From a non running requirement to a running requirement.
                var mustExist = GeneralizationRoot.MustExistService;
                //
                // Only 2 possible cases here:
                //
                // - There is no current MustExist Service for our Generalization.
                // - We specialize the current one.
                //
                Debug.Assert( mustExist == null || mustExist.IsGeneralizationOf( this ) );
                // Note: The other cases would be:
                //    - We are a Generalization of the current one. This is not possible since SetRunningRequirement is routed to the _mustExistSpecialization if it exists.
                //    - we are the current one... We would necessarily already be MustExist.
                //    - a specialization exists and we are not a specialization nor a generalization of it: this is not possible since we would have been disabled.
                //
                if( mustExist != null )
                {
                    // If we are specializing, the requirement of the current one may be stronger than our: we update it.
                    if( r < mustExist._runningRequirement )
                    {
                        _runningRequirement = mustExist._runningRequirement;
                        _runningRequirementReason = mustExist._runningRequirementReason;
                    }
                }
                // We must disable all sibling services (and plugins) from this up to mustExist (when mustExist is null, up to the root).
                var g = Generalization;
                if( g != null )
                {
                    // If we are the root, there is nothing to do, except updating the MustExistService (done below).
                    var specThatMustExist = this;
                    do
                    {
                        g._mustExistSpecialization = this;
                        g._directMustExistSpecialization = specThatMustExist;
                        var spec = g.FirstSpecialization;
                        while( spec != null )
                        {
                            if( spec != specThatMustExist && !spec.Disabled ) spec.SetDisabled( ServiceDisabledReason.AnotherSpecializationMustExist );
                            spec = spec.NextSpecialization;
                        }
                        PluginData p = g.FirstPlugin;
                        while( p != null )
                        {
                            if( !p.Disabled ) p.SetDisabled( PluginDisabledReason.ServiceSpecializationMustExist );
                            p = p.NextPluginForService;
                        }
                        specThatMustExist = g;
                        g = g.Generalization;
                    }
                    while( g != mustExist );
                }
                if( Disabled ) return false;
                GeneralizationRoot.MustExistServiceChanged( this );
            }
            Debug.Assert( !Disabled );
            // Now, if the OnlyPlugin exists, propagate the MustExist requirement to it.
            // MustExist requirement triggers MustExist on MustExist plugins to services requirements.
            // (This can be propagated if there is one and only one plugin for the service.)
            if( _theOnlyPlugin != null
                && !_theOnlyPlugin.SetRunningRequirement( r, PluginRunningRequirementReason.FromServiceToSingleImplementation )
                && !Disabled ) 
            {
                SetDisabled( ServiceDisabledReason.RequirementPropagationToSingleImplementationFailed );
            }
            return !Disabled;
        }

        /// <summary>
        /// Head of the linked list of ServiceData that specialize Service.
        /// </summary>
        public ServiceData FirstSpecialization;

        /// <summary>
        /// Linked list to another ServiceData that specialize Service.
        /// </summary>
        public readonly ServiceData NextSpecialization;

        /// <summary>
        /// Number of direct specializations.
        /// </summary>
        public int SpecializationCount;

        /// <summary>
        /// Head of the linked list of PluginData that implement this exact Service (not specialized ones).
        /// </summary>
        public PluginData FirstPlugin;

        /// <summary>
        /// Number of plugins for this service.
        /// </summary>
        public int PluginCount;

        /// <summary>
        /// Number of plugins for this service that are disabled.
        /// </summary>
        public int DisabledPluginCount;

        /// <summary>
        /// Number of total plugins (the ones for this service and for any of our specializations).
        /// </summary>
        public int TotalPluginCount;

        /// <summary>
        /// Number of total plugins that are disabled (the ones for this service and for any of our specializations).
        /// </summary>
        public int TotalDisabledPluginCount;

        /// <summary>
        /// Gets the number of available plugins (<see cref="TotalPluginCount"/> - <see cref="TotalDisabledPluginCount"/>).
        /// </summary>
        public int TotalAvailablePluginCount
        {
            get { return TotalPluginCount - TotalDisabledPluginCount; }
        }

        /// <summary>
        /// First step after object construction.
        /// </summary>
        /// <returns>The deepest specialization that must exist or null if none must exist or a conflict exists.</returns>
        protected ServiceData GetMustExistService()
        {
            Debug.Assert( !Disabled, "Must NOT be called on already disabled service." );
            // Handles direct specializations that MustExist.
            ServiceData directSpecThatHasMustExist = null;
            ServiceData specMustExist = null;
            ServiceData spec = FirstSpecialization;
            while( spec != null )
            {
                if( !spec.Disabled )
                {
                    var mustExist = spec.GetMustExistService();
                    // We may stop as soon as a conflict is detected, but we continue the loop to let any branches detect other potential conflicts.
                    if( !Disabled )
                    {
                        if( spec.DisabledReason == ServiceDisabledReason.MultipleSpecializationsMustExistByConfig )
                        {
                            Debug.Assert( mustExist == null, "Since a conflict has been detected below, returned mustExist is null." );
                            SetDisabled( ServiceDisabledReason.MultipleSpecializationsMustExistByConfig );
                            directSpecThatHasMustExist = specMustExist = null;
                        }
                        else
                        {
                            Debug.Assert( spec.Disabled == false, "Since it was not disabled before calling GetMustExistService, it could only be ServiceDisabledReason.MultipleSpecializationsMustExist." );
                            if( mustExist != null )
                            {
                                if( specMustExist != null )
                                {
                                    SetDisabled( ServiceDisabledReason.MultipleSpecializationsMustExistByConfig );
                                    directSpecThatHasMustExist = specMustExist = null;
                                }
                                else
                                {
                                    specMustExist = mustExist;
                                    directSpecThatHasMustExist = spec;
                                }
                            }
                        }
                    }
                }
                spec = spec.NextSpecialization;
            }
            Debug.Assert( !Disabled || specMustExist == null, "(Conflict below <==> Disabled) => specMustExist has been set to null." );
            Debug.Assert( (specMustExist != null) == (directSpecThatHasMustExist != null) );
            if( !Disabled )
            {
                // No specialization is required to exist, is it our case?
                if( specMustExist == null )
                {
                    Debug.Assert( ServiceSolvedStatus != SolvedConfigStatus.Disabled, "Caution: Disabled is greater than MustExist." );
                    if( ServiceSolvedStatus >= SolvedConfigStatus.MustExist ) specMustExist = this;
                }
                else
                {
                    // A specialization must exist: it must be the only one, others are disabled.
                    spec = FirstSpecialization;
                    while( spec != null )
                    {
                        if( spec != directSpecThatHasMustExist && !spec.Disabled )
                        {
                            spec.SetDisabled( ServiceDisabledReason.AnotherSpecializationMustExistByConfig );
                        }
                        spec = spec.NextSpecialization;
                    }
                    _mustExistSpecialization = specMustExist;
                    _directMustExistSpecialization = directSpecThatHasMustExist;
                    // Since there is a MustExist specialization, it concentrates the running requirements
                    // of all its generalization (here from their configurations).
                    if( _runningRequirement > specMustExist._runningRequirement )
                    {
                        specMustExist._runningRequirement = _runningRequirement;
                        specMustExist._runningRequirementReason = ServiceRunningRequirementReason.FromGeneralization;
                    }
                }
                Debug.Assert( !Disabled, "The fact that this service (or a specialization) must exist, can not disable this service." );
            }
            return specMustExist;
        }

        internal void AddPlugin( PluginData p )
        {
            // Consider its RunningRequirements to detect trivial case: the fact that another plugin 
            // must exist for the same Generalization service (or less trivially that 
            // this must exist plugin conflicts with some MustExist at the services level).
            if( p.MinimalRunningRequirement >= RunningRequirement.MustExist )
            {
                Debug.Assert( !p.Disabled );
                GeneralizationRoot.SetMustExistPluginByConfig( p );
            }
            // Adds the plugin, taking its disabled state into account.
            FirstPlugin.NextPluginForService = FirstPlugin;
            FirstPlugin = p;
            ++PluginCount;
            if( p.Disabled ) ++DisabledPluginCount;
            ServiceData g = this;
            do
            {
                ++g.TotalPluginCount;
                if( p.Disabled ) ++g.TotalDisabledPluginCount;
                g = g.Generalization;
            }
            while( g != null );
        }

        internal void AddMustExistReferencer( PluginData plugin )
        {
            Debug.Assert( !Disabled && !plugin.Disabled );
            if( _mustExistReferencer == null ) _mustExistReferencer = new List<PluginData>();
            _mustExistReferencer.Add( plugin );
        }

        internal void OnAllPluginsAdded()
        {
            Debug.Assert( !Disabled, "Must NOT be called on already disabled service." );
            Debug.Assert( MustExistSpecialization == null || PluginCount == DisabledPluginCount, "If there is a must exist specialization, all our plugins are disabled." );
            // Handle the case where TotalPluginCount is zero (there is no implementation).
            // or where TotalDisabledPluginCount is the same as TotalPluginCount.
            if( TotalPluginCount == 0 )
            {
                SetDisabled( ServiceDisabledReason.NoPlugin );
            }
            int nbAvailable = TotalAvailablePluginCount;
            if( nbAvailable == 0 )
            {
                SetDisabled( ServiceDisabledReason.AllPluginsAreDisabled );
            }
            else if( nbAvailable == 1 )
            {
                RetrieveTheOnlyPlugin( fromConfig: true );
            }
        }

        internal void OnPluginDisabled( PluginData p )
        {
            Debug.Assert( p.Service == this && p.Disabled );
            ++DisabledPluginCount;
            // We must update and check the total number of plugins.
            ServiceData g = this;
            while( g != null )
            {
                ++g.TotalDisabledPluginCount;
                int nbAvailable = TotalAvailablePluginCount;
                if( nbAvailable == 0 )
                {
                    _theOnlyPlugin = null;
                    if( !g.Disabled ) SetDisabled( ServiceDisabledReason.AllPluginsAreDisabled );
                }
                if( nbAvailable == 1 )
                {
                    RetrieveTheOnlyPlugin( fromConfig: false );
                }
                g = g.Generalization;
            }
        }

        private void RetrieveTheOnlyPlugin( bool fromConfig )
        {
            Debug.Assert( _theOnlyPlugin == null && TotalAvailablePluginCount == 1 );
            if( DisabledPluginCount == PluginCount )
            {
                ServiceData spec = FirstSpecialization;
                while( spec != null )
                {
                    if( spec._theOnlyPlugin != null )
                    {
                        _theOnlyPlugin = spec._theOnlyPlugin;
                        break;
                    }
                    spec = spec.NextSpecialization;
                }
            }
            else
            {
                _theOnlyPlugin = FirstPlugin;
                while( _theOnlyPlugin.Disabled ) _theOnlyPlugin = _theOnlyPlugin.NextPluginForService;
            }
            Debug.Assert( _theOnlyPlugin != null );
            // As soon as the only plugin appears, propagate our requirement to it.
            var reason = fromConfig ? PluginRunningRequirementReason.FromServiceConfigToSingleImplementation : PluginRunningRequirementReason.FromServiceToSingleImplementation;
            _theOnlyPlugin.SetRunningRequirement( MinimalRunningRequirement, reason );
        }

    }
}