using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;

namespace CK.Plugin.Hosting
{
    partial class PluginData
    {
        RunningStatus _status;
        bool _shouldInitiallyRun;
        bool _isStructurallyRunnable;
        public bool IsRunning;
        PluginServiceRelation[] _serviceReferences;

        internal int IndexInAllServiceRunnables;


        public bool ShouldInitiallyRun
        {
            get { return _shouldInitiallyRun; }
        }

        public bool IsStructurallyRunnable
        {
            get { return _isStructurallyRunnable; }
        }

        public RunningStatus Status
        {
            get { return _status; }
        }

        public void InitializeDynamicState( PlanCalculatorStrategy strategy, Predicate<IPluginInfo> isPluginRunning )
        {
            if( Disabled ) _status = RunningStatus.Disabled;
            else
            {
                IsRunning = isPluginRunning( PluginInfo );
                if( MinimalRunningRequirement == RunningRequirement.MustExistAndRun )
                {
                    _shouldInitiallyRun = true;
                    _status = RunningStatus.RunningLocked;
                }
                else
                {
                    _shouldInitiallyRun = ComputeShouldInitiallyRun( strategy );
                    _status = _shouldInitiallyRun ? RunningStatus.Running : RunningStatus.Stopped;
                }
                _isStructurallyRunnable = true;
                int iS = 0;
                _serviceReferences = new PluginServiceRelation[ PluginInfo.ServiceReferences.Count ];
                foreach( var r in PluginInfo.ServiceReferences )
                {
                    var rel = new PluginServiceRelation( this, r.Requirements, _allServices[r.Reference] );
                    _serviceReferences[iS++] = rel;
                    if( !rel.IsStructurallySatisfied ) _isStructurallyRunnable = false;
                }
            }

        }

        bool ComputeShouldInitiallyRun( PlanCalculatorStrategy strategy )
        {
            Debug.Assert( !Disabled && MinimalRunningRequirement != RunningRequirement.MustExistAndRun );
            bool shouldRun;
            switch( strategy )
            {
                case PlanCalculatorStrategy.Minimal:
                    shouldRun = false;
                    break;
                case PlanCalculatorStrategy.HonorConfigTryStart:
                    shouldRun = IsRunning || IsTryStartByConfig;
                    break;
                case PlanCalculatorStrategy.HonorConfigTryStartIgnoreIsRunning:
                    shouldRun = IsTryStartByConfig;
                    break;
                case PlanCalculatorStrategy.HonorConfigAndReferenceTryStart:
                    shouldRun = IsRunning || IsTryStartByConfigOrReference;
                    break;
                case PlanCalculatorStrategy.HonorConfigAndReferenceTryStartIgnoreIsRunning:
                    shouldRun = IsTryStartByConfigOrReference;
                    break;
                case PlanCalculatorStrategy.Maximal:
                default:
                    shouldRun = true;
                    break;
            }
            return shouldRun;
        }

        internal void SetStatusFromService( RunningStatus s )
        {
            Debug.Assert( s == RunningStatus.Stopped || s == RunningStatus.Running );
            Debug.Assert( _status != RunningStatus.RunningLocked || s == RunningStatus.Running, "RunningLocked (ie. this MustExistAndRun) => Service sets it to running." );
            if( _status != RunningStatus.RunningLocked ) _status = s; 
        }

    }
}
