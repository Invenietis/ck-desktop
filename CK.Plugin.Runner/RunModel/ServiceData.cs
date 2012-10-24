using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CK.Plugin.Hosting
{
    enum ServiceDisabledReason
    {
        None,
        Config,
        GeneralizationIsDisabledByConfig,
        MultipleSpecializationsMustExistByConfig,
        AnotherSpecializationMustExistByConfig,
        AnotherSpecializationHasPluginThatMustExistByConfig,
        DirectGeneralizationHasPluginThatMustExistByConfig,
        GeneralizationIsDisabled,
        AnotherSpecializationMustExist,
        NoPlugin,
        AllPluginsAreDisabled,
        MultiplePluginsMustExistByConfig,
    }

    enum ServiceRunningRequirementReason
    {
        Config,
        FromSingleImplementationConfig,
        FromMustExistReference,
        FromSpecialization,
        FromGeneralization,
    }

    class ServiceData
    {
        ServiceDisabledReason _disabledReason;
        List<PluginData> _mustExistReferencer;
        PluginData _cacheTheOnlyPlugin;
        ServiceRunningRequirementReason _runningRequirementReason;

        internal ServiceData( IServiceInfo s, ServiceData generalization, SolvedConfigStatus serviceStatus )
        {
            ServiceInfo = s;
            if( generalization != null )
            {
                GeneralizationRoot = generalization.GeneralizationRoot;
                PrevSpecialization = Generalization.FirstSpecialization;
                if( PrevSpecialization == null ) Generalization.FirstSpecialization = this;
                else PrevSpecialization.NextSpecialization = this;
                Generalization.LastSpecialization = this;
                ++Generalization.SpecializationCount;
                SpecializationLevel = generalization.SpecializationLevel + 1;
            }
            else
            {
                Generalization = generalization;
                GeneralizationRoot = null;
                SpecializationLevel = 0;
            }
            if( (ServiceSolvedStatus = serviceStatus) == SolvedConfigStatus.Disabled )
            {
                _disabledReason = ServiceDisabledReason.Config;
            }
            else if( Generalization != null && Generalization.Disabled )
            {
                _disabledReason = ServiceDisabledReason.GeneralizationIsDisabledByConfig;
            }
            if( !Disabled ) MinimalRunningRequirement = (RunningRequirement)serviceStatus;
            _runningRequirementReason = ServiceRunningRequirementReason.Config;
        }

        public readonly IServiceInfo ServiceInfo;

        /// <summary>
        /// The generalization service if any. When not null, it is necessarily not disabled (otherwise, this specialization 
        /// is also disabled and hence does not exist).
        /// </summary>
        public readonly ServiceData Generalization;

        /// <summary>
        /// Level of specialization. 0 for root.
        /// </summary>
        public readonly int SpecializationLevel;

        /// <summary>
        /// Root of Generalization if this service is a specialization.
        /// Null otherwise (if it is a root one).
        /// </summary>
        public readonly ServiceRootData GeneralizationRoot;

        /// <summary>
        /// The SolvedConfigStatus of the Service. 
        /// </summary>
        public readonly SolvedConfigStatus ServiceSolvedStatus;

        /// <summary>
        /// Gets the service that must exist. It may be this one or any specialization and can evolve
        /// only to become more specialized service.
        /// </summary>
        public ServiceData MustExistService { get; private set; }

        /// <summary>
        /// Gets whether this service must exist or run. It is initialized by the configuration, but may evolve
        /// depending on our plugins or on the plugins that require this service.
        /// </summary>
        public RunningRequirement MinimalRunningRequirement { get; private set; }

        /// <summary>
        /// Gets the strongest reason that explains this service MinimalRunningRequirement. 
        /// </summary>
        public ServiceRunningRequirementReason RunningRequirementReason { get; private set; }

        /// <summary>
        /// Gets the only available plugin, or null if zero or more than one plugins exists.
        /// This enables the <see cref="MinimalRunningRequirement"/> propagation from service to its lonely associated plugin.
        /// </summary>
        public PluginData TheSingleImplementation 
        {
            get 
            {
                if( _disabledReason != ServiceDisabledReason.None ) return null;
                int nbAvailable = TotalPluginCount - TotalDisabledPluginCount;
                Debug.Assert( nbAvailable != 0 || Disabled, "nbAvailable == 0 => Disabled." );
                if( nbAvailable == 0 || nbAvailable > 1 ) return null;
                if( _cacheTheOnlyPlugin == null )
                {
                    var specHasIt = SpecializationsApply( s => s.TheSingleImplementation != null );
                    if( specHasIt != null ) _cacheTheOnlyPlugin = specHasIt.TheSingleImplementation;
                    else _cacheTheOnlyPlugin = PluginsApply( p => !p.Disabled );
                    Debug.Assert( _cacheTheOnlyPlugin != null, "nbAvailable == 1 ==> one non-disabled plugin exists." );
                }
                return _cacheTheOnlyPlugin;
            }
        }

        #region Specialization linked list & count.
        /// <summary>
        /// Head of the linked list of ServiceData that specialize Service.
        /// </summary>
        public ServiceData FirstSpecialization;

        /// <summary>
        /// Tail of the linked list of ServiceData that specialize Service.
        /// </summary>
        public ServiceData LastSpecialization;

        /// <summary>
        /// Linked list to another ServiceData that specialize Service.
        /// </summary>
        public ServiceData NextSpecialization;

        /// <summary>
        /// Linked list to another ServiceData that specialize Service.
        /// </summary>
        public ServiceData PrevSpecialization;

        /// <summary>
        /// Number of direct specializations.
        /// </summary>
        public int SpecializationCount;


        #endregion

        /// <summary>
        /// Head of the linked list of PluginData that implement this exact Service (not specialized ones).
        /// </summary>
        public PluginData FirstPlugin;
        
        /// <summary>
        /// Tail of the linked list of PluginData that implement this exact Service (not specialized ones).
        /// </summary>
        public PluginData LastPlugin;

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

        internal void AddPlugin( PluginData p )
        {
            p.PrevPluginForService = FirstPlugin;
            if( FirstPlugin == null ) FirstPlugin = p;
            else FirstPlugin.NextPluginForService = p;
            LastPlugin = p;
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

        internal void OnPluginDisabled( PluginData p )
        {
            Debug.Assert( p.Service == this && p.Disabled );
            ++DisabledPluginCount;
            // We must update and check the total number of plugins.
            ServiceData g = this;
            while( g != null )
            {
                ++g.TotalDisabledPluginCount;
                int nbAvailable = g.TotalPluginCount - g.TotalDisabledPluginCount;
                if( nbAvailable == 0 )
                {
                    if( !g.Disabled ) SetDisabled( ServiceDisabledReason.AllPluginsAreDisabled );
                }
                g = g.Generalization;
            }
        }

        public bool SpecializationMustExist
        {
            get 
            {
                ServiceRootData r = GeneralizationRoot ?? (ServiceRootData)this;
                return r.MustExistService != null && r.MustExistService.SpecializationLevel > SpecializationLevel;
            }
        }

        /// <summary>
        /// Gets whether this service is disabled. 
        /// </summary>
        public bool Disabled
        {
            get { return _disabledReason == ServiceDisabledReason.None; }
        }

        /// <summary>
        /// Gets the first reason why this service is disabled. 
        /// </summary>
        public ServiceDisabledReason DisabledReason
        {
            get { return _disabledReason; }
        }

        internal void SetDisabled( ServiceDisabledReason r )
        {
            Debug.Assert( r != ServiceDisabledReason.None );
            Debug.Assert( _disabledReason == ServiceDisabledReason.None );
            _disabledReason = r;
            SpecializationsApply( s => { if( !s.Disabled ) s.SetDisabled( ServiceDisabledReason.GeneralizationIsDisabled ); return false; } );
            PluginsApply( p => { if( !p.Disabled ) p.SetDisabled( PluginDisabledReason.ServiceIsDisabled ); return false; } );
            if( _mustExistReferencer != null ) 
            {
                foreach( PluginData p in _mustExistReferencer )
                {
                    if( !p.Disabled ) p.SetDisabled( PluginDisabledReason.MustExistReferenceIsDisabled );
                }
            }
        }

        internal void SetRunningRequirement( RunningRequirement r, ServiceRunningRequirementReason reason )
        {
            // This can be called on an already disabled service.
            Debug.Assert( r > MinimalRunningRequirement );
            MinimalRunningRequirement = r;
            _runningRequirementReason = reason;
        }
        
        internal ServiceData SpecializationsApply( Predicate<ServiceData> beforeSpecializations )
        {
            ServiceData spec = FirstSpecialization;
            while( spec != null )
            {
                if( beforeSpecializations( spec ) ) return spec;
                ServiceData result = spec.SpecializationsApply( beforeSpecializations );
                if( result != null ) return result;
                spec = spec.NextSpecialization;
            }
            return null;
        }

        internal PluginData PluginsApply( Predicate<PluginData> a )
        {
            PluginData p = FirstPlugin;
            while( p != null )
            {
                if( a( p ) ) return p;
                p = p.NextPluginForService;
            }
            return null;
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
                    if( spec.DisabledReason == ServiceDisabledReason.MultipleSpecializationsMustExistByConfig )
                    {
                        Debug.Assert( mustExist == null, "Since a conflict has been detected below, returned mustExist is null." );
                        SetDisabled( ServiceDisabledReason.MultipleSpecializationsMustExistByConfig );
                        specMustExist = null;
                    }
                    else
                    {
                        Debug.Assert( spec.Disabled == false, "Since it was not disabled before calling GetMustExistService, it could only be ServiceDisabledReason.MultipleSpecializationsMustExist." );
                        // We may stop as soon as a conflict is detected, but we continue the loop to let any branches detect other potential conflicts.
                        if( !Disabled && mustExist != null )
                        {
                            if( specMustExist != null )
                            {
                                SetDisabled( ServiceDisabledReason.MultipleSpecializationsMustExistByConfig );
                                specMustExist = null;
                            }
                            else
                            {
                                specMustExist = mustExist;
                                directSpecThatHasMustExist = spec;
                            }
                        }
                    }
                }
                spec = spec.NextSpecialization;
            }
            Debug.Assert( !Disabled || specMustExist == null, "(Conflict below <==> Disabled) => specMustExist has been set to null." );
            if( !Disabled )
            { 
                // No specialization is required to exist, is it our case?
                if( specMustExist == null )
                {
                    Debug.Assert( ServiceSolvedStatus != SolvedConfigStatus.Disabled, "Caution: Disabled is greater than MustExist." );
                    if( ServiceSolvedStatus >= SolvedConfigStatus.MustExist )
                    {
                        SetServiceMustExist();
                        specMustExist = this;
                    }
                }
                Debug.Assert( specMustExist == MustExistService );
                Debug.Assert( !Disabled, "The fact that this service (or a specialization) must exist, can not disable this service." );
            }
            return specMustExist;
        }

        void SetServiceMustExist()
        {
            if( MustExistService != this )
            {
                Debug.Assert( MustExistService == null, "MustExistService can only be specialized." );
                MustExistService = this;
                if( Generalization != null ) Generalization.SetSpecializedServiceMustExist( this, this );
            }
        }

        void SetSpecializedServiceMustExist( ServiceData directSpecThatHasMustExist, ServiceData mustExist )
        {
            if( MustExistService == mustExist ) return;
            Debug.Assert( MustExistService == null || MustExistService.SpecializationLevel < mustExist.SpecializationLevel, "MustExistService can only be specialized." );
            // A specialization must exist: 
            // - it must be the only one, others are disabled.
            // - no plugins can be enabled at this level.
            Debug.Assert( directSpecThatHasMustExist != null && directSpecThatHasMustExist.MinimalRunningRequirement == mustExist.MinimalRunningRequirement );
            ServiceData spec = FirstSpecialization;
            while( spec != null )
            {
                if( spec != directSpecThatHasMustExist && !spec.Disabled )
                {
                    spec.SetDisabled( ServiceDisabledReason.AnotherSpecializationMustExistByConfig );
                }
                spec = spec.NextSpecialization;
            }
            PluginData p = FirstPlugin;
            while( p != null )
            {
                if( !p.Disabled ) p.SetDisabled( PluginDisabledReason.ServicePluginsAreDisabledSinceSpecializationMustExist );
                p = p.NextPluginForService;
            }
            if( MinimalRunningRequirement < mustExist.MinimalRunningRequirement )
            {
                SetRunningRequirement( mustExist.MinimalRunningRequirement, ServiceRunningRequirementReason.FromSpecialization );
            }
            else if( MinimalRunningRequirement > mustExist.MinimalRunningRequirement )
            {
                spec = mustExist;
                do
                {
                    spec.SetRunningRequirement( MinimalRunningRequirement, ServiceRunningRequirementReason.FromGeneralization );
                    spec = spec.Generalization;
                }
                while( MinimalRunningRequirement > spec.MinimalRunningRequirement );
            }
            MustExistService = mustExist;
            if( Generalization != null ) Generalization.SetSpecializedServiceMustExist( this, mustExist );
        }

        protected PluginData InitializeFromPluginsAndGetMustExistPlugin()
        {
            Debug.Assert( !Disabled, "Must NOT be called on already disabled service." );
            // Handle the case where TotalPluginCount is zero (there is no implementation).
            // or where TotalDisabledPluginCount is the same as TotalPluginCount.
            if( TotalPluginCount == 0 )
            {
                SetDisabled( ServiceDisabledReason.NoPlugin );
                return null;
            }
            if( TotalDisabledPluginCount == TotalPluginCount )
            {
                SetDisabled( ServiceDisabledReason.AllPluginsAreDisabled );
                return null;
            }
            // If SpecializationMustExist, we must disable all our plugins.
            if( SpecializationMustExist )
            {
                PluginsApply( p => { p.SetDisabled( PluginDisabledReason.ServicePluginsAreDisabledSinceSpecializationMustExist ); return false; } );
            }
            // Handles plugins below us that MustRun.
            ServiceData directSpecForWhichOnePluginMustExist = null;
            PluginData pluginMustExist = null;
            ServiceData spec = FirstSpecialization;
            while( spec != null )
            {
                if( !spec.Disabled )
                {
                    var mustExist = spec.InitializeFromPluginsAndGetMustExistPlugin();
                    // MustExist for specialization is ignored if a conflict has been detected.
                    if( spec.DisabledReason == ServiceDisabledReason.MultiplePluginsMustExistByConfig )
                    {
                        Debug.Assert( mustExist == null, "Since a conflict has been detected below, returned mustExist is null." );
                        if( !Disabled ) SetDisabled( ServiceDisabledReason.MultiplePluginsMustExistByConfig );
                        pluginMustExist = null;
                    }
                    else
                    {
                        // We may stop as soon as a conflict is detected, but we continue the loop to let any branches detect other potential conflicts.
                        // If mustExist is not null, the specialization is not disabled.
                        Debug.Assert( mustExist == null || !spec.Disabled, "mustExist != null => spec.Disabled == false" );
                        if( !Disabled && mustExist != null )
                        {
                            if( pluginMustExist != null )
                            {
                                SetDisabled( ServiceDisabledReason.MultiplePluginsMustExistByConfig );
                                pluginMustExist = null;
                            }
                            else
                            {
                                pluginMustExist = mustExist;
                                directSpecForWhichOnePluginMustExist = spec;
                            }
                        }
                    }
                    spec = spec.NextSpecialization;
                }
            }
            Debug.Assert( !Disabled || pluginMustExist == null, "(Conflict below <==> Disabled) => pluginMustExist has been set to null." );
            if( !Disabled )
            {
                if( pluginMustExist == null )
                {
                    // No specialization required that one of its implementation run, what about our plugins?
                    PluginData p = FirstPlugin;
                    while( p != null )
                    {
                        if( p.PluginSolvedStatus >= SolvedConfigStatus.MustExist )
                        {
                            if( pluginMustExist != null )
                            {
                                SetDisabled( ServiceDisabledReason.MultiplePluginsMustExistByConfig );
                                pluginMustExist = null;
                                break;
                            }
                            else pluginMustExist = p;
                        }
                        p = p.NextPluginForService;
                    }
                    // If one of our plugin must be the only one, our specializations can be disabled.
                    if( pluginMustExist != null )
                    {
                        spec = FirstSpecialization;
                        while( spec != null )
                        {
                            if( !spec.Disabled ) spec.SetDisabled( ServiceDisabledReason.DirectGeneralizationHasPluginThatMustExistByConfig );
                            spec = spec.NextSpecialization;
                        }
                    }
                }
                else
                {
                    // One plugin of our specializations must be the only one: its sibling specializations can be disabled.
                    Debug.Assert( directSpecForWhichOnePluginMustExist != null );
                    spec = FirstSpecialization;
                    while( spec != null )
                    {
                        if( spec != directSpecForWhichOnePluginMustExist && !spec.Disabled )
                        {
                            spec.SetDisabled( ServiceDisabledReason.AnotherSpecializationHasPluginThatMustExistByConfig );
                        }
                        spec = spec.NextSpecialization;
                    }
                }
            }
            Debug.Assert( !Disabled || pluginMustExist == null, "Disabled => pluginMustExist is null." );
            // The MustExist plugin is necessarily TheSingleImplementation...
            if( pluginMustExist != null ) _cacheTheOnlyPlugin = pluginMustExist;
            // ...but the single implementation may exist without beeing MustExist or MustExistAndRun.
            var theOne = TheSingleImplementation;
            if( theOne != null )
            {
                // When the one exists, its minimal running requirement is the same as the service.
                if( MinimalRunningRequirement > theOne.MinimalRunningRequirement )
                {
                    theOne.SetRunningRequirement( MinimalRunningRequirement, PluginRunningRequirementReason.FromServiceConfigToSingleImplementation );
                }
                else if( MinimalRunningRequirement < theOne.MinimalRunningRequirement )
                {
                    SetRunningRequirement( theOne.MinimalRunningRequirement, ServiceRunningRequirementReason.FromSingleImplementationConfig );
                }
            }
            return pluginMustExist;
        }

        internal void AddMustExistReferencer( PluginData plugin )
        {
            Debug.Assert( !Disabled && !plugin.Disabled );
            if( _mustExistReferencer == null ) _mustExistReferencer = new List<PluginData>();
            _mustExistReferencer.Add( plugin );
        }

        internal void PropagateMinimalRunningRequirementFrom( PluginData plugin, RunningRequirement refRequirement, Dictionary<IServiceInfo, ServiceData> services )
        {
            Debug.Assert( !Disabled && !plugin.Disabled && plugin.MinimalRunningRequirement >= RunningRequirement.MustExist && refRequirement >= RunningRequirement.OptionalTryStart );

            // Routes the call to the Service associated to the single implementation if it exists.
            if( TheSingleImplementation != null ) TheSingleImplementation.Service.PropagateMinimalRunningRequirementFrom( plugin, refRequirement, services );
            else
            {
                // We are on the single implementation, or no single implementation exists yet.
                Debug.Assert( TheSingleImplementation == null || TheSingleImplementation.Service == this );
                var impliedRequirement = (RunningRequirement)Math.Min( (int)plugin.MinimalRunningRequirement, (int)refRequirement );
                if( impliedRequirement <= MinimalRunningRequirement ) return;

                SetRunningRequirement( impliedRequirement, ServiceRunningRequirementReason.FromMustExistReference );
                // Propagation to service root disables sibling services that disable their plugins.
                if( Generalization != null ) Generalization.PropagateMinimalRunningRequirementFromSpecialization( this, services );
                var theOne = TheSingleImplementation;
                if( theOne != null ) theOne.PropagateMinimalRunningRequirementFromServiceToSingleImplementation( MinimalRunningRequirement, services );
            }
        }

        private void PropagateMinimalRunningRequirementFromSpecialization( ServiceData spec, Dictionary<IServiceInfo, ServiceData> services )
        {
            Debug.Assert( !Disabled && !spec.Disabled );
            if( spec.MinimalRunningRequirement <= MinimalRunningRequirement ) return;

            SetRunningRequirement( spec.MinimalRunningRequirement, ServiceRunningRequirementReason.FromSpecialization );
            if( MinimalRunningRequirement >= RunningRequirement.MustExist )
            {
                ServiceData s = FirstSpecialization;
                while( s != null )
                {
                    if( s != spec && !s.Disabled )
                    {
                        s.SetDisabled( ServiceDisabledReason.AnotherSpecializationMustExist );
                    }
                    s = s.NextSpecialization;
                }
            }
            if( Generalization != null ) Generalization.PropagateMinimalRunningRequirementFromSpecialization( this, services );
            //
            var theOne = TheSingleImplementation;
            if( theOne != null ) theOne.PropagateMinimalRunningRequirementFromServiceToSingleImplementation( MinimalRunningRequirement, services );
        }
    }

}
