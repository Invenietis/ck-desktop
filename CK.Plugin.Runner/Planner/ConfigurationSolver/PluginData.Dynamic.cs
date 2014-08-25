#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Plugin.Runner\Planner\ConfigurationSolver\PluginData.Dynamic.cs) is part of CiviKey. 
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
    partial class PluginData : IAlternative
    {
        RunningStatus _status;
        bool _shouldInitiallyRun;
        bool _isStructurallyRunnable;
        public bool IsCurrentlyRunning;
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
                IsCurrentlyRunning = isPluginRunning( PluginInfo );
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
                    shouldRun = IsCurrentlyRunning || IsTryStartByConfig;
                    break;
                case PlanCalculatorStrategy.HonorConfigTryStartIgnoreIsRunning:
                    shouldRun = IsTryStartByConfig;
                    break;
                case PlanCalculatorStrategy.HonorConfigAndReferenceTryStart:
                    shouldRun = IsCurrentlyRunning || IsTryStartByConfigOrReference;
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

        public int ComputeRunningCost()
        {
            int cost = 0;
            foreach( var r in _serviceReferences )
            {
                switch( r.Requirement )
                {
                    case RunningRequirement.MustExistAndRun:
                        if( !r.Service.IsRunning ) return 0xFFFFFF;
                        break;
                    case RunningRequirement.MustExistTryStart:
                        if( r.Service.Disabled ) return 0xFFFFFF;
                        if( !r.Service.IsRunning ) cost += 10;
                        break;
                    case RunningRequirement.MustExist:
                        if( r.Service.Disabled ) return 0xFFFFFF;
                        break;
                    case RunningRequirement.OptionalTryStart:
                        // If a service is disabled, it is useless to increment the cost
                        // since no configuration will be able to satisfy it.
                        if( r.Service.Status == RunningStatus.Stopped ) cost += 10;
                        break;
                }
            }
            return cost;
        }

        internal IAlternative NextAlternative;

        bool IAlternative.MoveNext()
        {
            Debug.Assert( Service == null, "This is only for independent plugins." );
            Debug.Assert( _status == RunningStatus.Stopped || _status == RunningStatus.Running );
            if( ThisMoveNext() ) return true;
            if( NextAlternative != null ) return NextAlternative.MoveNext();
            return false;
        }

        private bool ThisMoveNext()
        {
            if( _status == RunningStatus.Running )
            {
                _status = RunningStatus.Stopped;
                return _shouldInitiallyRun;
            }
            _status = RunningStatus.Running;
            return !_shouldInitiallyRun;
        }
    }
}
