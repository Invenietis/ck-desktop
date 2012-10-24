using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using CK.Core;

namespace CK.Plugin.Hosting
{
    partial class AlternativeManager
    {
        class RPlugin
        {
            public readonly PluginData PluginData;
            RunningStatus _runningStatus;
            ServiceRefWeight[] _serviceRefWeights;

            struct ServiceRefWeight
            {
                public readonly int Weight;
                public readonly IServiceDriver Service;
                
                public ServiceRefWeight( RunningRequirement r, IServiceDriver s )
                {
                    Debug.Assert( r != RunningRequirement.MustExist && r != RunningRequirement.Optional );
                    switch( r )
                    {
                        case RunningRequirement.MustExistAndRun: Weight = 0xFFFFF; break;
                        case RunningRequirement.OptionalTryStart:
                        case RunningRequirement.MustExistTryStart: Weight = 1; break;
                        default: 
                            Debug.Fail( "MustExist & Optional have no weight." ); 
                            Weight = 0; 
                            break;
                    }
                    Service = s;
                }
            }

            public RPlugin( PluginData p )
            {
                PluginData = p;
                // _runningStatus = p.RunningStatus;
            }

            public RunningStatus RunningStatus
            {
                get { return _runningStatus; }
            }

            public bool Locked
            {
                get { return _runningStatus == Plugin.RunningStatus.Disabled || _runningStatus == Plugin.RunningStatus.RunningLocked; }
            }

            public bool BestInitialState( PlanCalculatorStrategy strategy )
            {
                bool bestInitialState;
                switch( strategy )
                {
                    case PlanCalculatorStrategy.Minimal:
                        bestInitialState = false;
                        break;
                    case PlanCalculatorStrategy.HonorConfigTryStart:
                        bestInitialState = PluginData.IsRunning
                                            || PluginData.PluginSolvedStatus == SolvedConfigStatus.OptionalTryStart
                                            || PluginData.PluginSolvedStatus == SolvedConfigStatus.MustExistTryStart
                                            || ((PluginData.MinimalRunningRequirement == RunningRequirement.OptionalTryStart || PluginData.MinimalRunningRequirement == RunningRequirement.MustExistTryStart)
                                                 && PluginData.MinimalRunningRequirementReason != PluginRunningRequirementReason.FromServiceToSingleImplementation);
                        break;
                    case PlanCalculatorStrategy.HonorConfigTryStartIgnoreIsRunning:
                        bestInitialState = PluginData.PluginSolvedStatus == SolvedConfigStatus.OptionalTryStart
                                            || PluginData.PluginSolvedStatus == SolvedConfigStatus.MustExistTryStart
                                            || ((PluginData.MinimalRunningRequirement == RunningRequirement.OptionalTryStart || PluginData.MinimalRunningRequirement == RunningRequirement.MustExistTryStart)
                                                 && PluginData.MinimalRunningRequirementReason != PluginRunningRequirementReason.FromServiceToSingleImplementation);
                        break;
                    case PlanCalculatorStrategy.HonorConfigAndReferenceTryStart:
                        bestInitialState = PluginData.IsRunning || PluginData.MinimalRunningRequirement == RunningRequirement.OptionalTryStart || PluginData.MinimalRunningRequirement == RunningRequirement.MustExistTryStart;
                        break;
                    case PlanCalculatorStrategy.HonorConfigAndReferenceTryStartIgnoreIsRunning:
                        bestInitialState = PluginData.MinimalRunningRequirement == RunningRequirement.OptionalTryStart || PluginData.MinimalRunningRequirement == RunningRequirement.MustExistTryStart;
                        break;
                    case PlanCalculatorStrategy.Maximal:
                    default:
                        bestInitialState = true;
                        break;
                }
                return bestInitialState;
            }

            private ServiceRefWeight[] EnsureServiceRefWeight( AlternativeManager m )
            {
                if( _serviceRefWeights == null )
                {
                    _serviceRefWeights = PluginData.PluginInfo.ServiceReferences
                                                                    .Where( r => r.Requirements != RunningRequirement.MustExist && r.Requirements != RunningRequirement.Optional )
                                                                    .Select( r => new ServiceRefWeight( r.Requirements, m._knownServices.GetValueWithDefault( r.Reference, null ) ) )
                                                                    .Where( t => t.Service != null )
                                                                    .ToArray();
                }
                return _serviceRefWeights;
            }

            internal void Start( AlternativeManager m )
            {
                Debug.Assert( _runningStatus == Plugin.RunningStatus.Stopped );
                _runningStatus = Plugin.RunningStatus.Running;
                foreach( ServiceRefWeight r in _serviceRefWeights )
                {
                    r.Service.BoostStartWeight( r.Weight );
                }
            }

        }

    }
}
