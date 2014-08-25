#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Plugin.Runner\Planner\ConfigurationSolver\ServiceRootData.Dynamic.cs) is part of CiviKey. 
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
    partial class ServiceRootData : IAlternative
    {
        ServiceData _firstRunnableService;
        ServiceData _lastRunnableService;

        PluginData _runningPlugin;
        int _runningIndex;
        int _runningCount;
        bool _finallyNotRunning;


        public override PluginData RunningPlugin
        {
            get { return _runningPlugin; }
        }

        public int RunningCount
        {
            get { return _runningCount; }
        }

        public new void InitializeDynamicState( PlanCalculatorStrategy strategy )
        {
            _runningPlugin = base.InitializeDynamicState( strategy );
            if( !Disabled && ServiceInfo.IsDynamicService )
            {
                for( int i = 0; i < _allRunnables.Length; ++i )
                {
                    _allRunnables[i].IndexInAllServiceRunnables = i;
                }
                Debug.Assert( MinimalRunningRequirement != RunningRequirement.MustExistAndRun || _runningPlugin != null, "When MustExistAndRun, then InitializeDynamicState found a first running plugin." );
                _runningCount = TotalAvailablePluginCount;
                if( MinimalRunningRequirement != RunningRequirement.MustExistAndRun )
                {
                    _runningCount += 1;
                    _finallyNotRunning = true;
                }
                UpdateStatusFromRunningPlugin();
                _runningIndex = 0;
            }
        }

        internal void AddRunnableService( ServiceData s )
        {
            if( _firstRunnableService == null ) _firstRunnableService = _lastRunnableService = s;
            else
            {
                _lastRunnableService._nextRunnableService = s;
                _lastRunnableService = s;
            }
        }

        internal IAlternative NextAlternative;

        bool IAlternative.MoveNext()
        {
            if( ThisMoveNext() ) return true;
            if( NextAlternative != null ) return NextAlternative.MoveNext();
            return false;
        }

        private bool ThisMoveNext()
        {
            if( _runningPlugin == null ) _runningPlugin = _allRunnables[0];
            else
            {
                int nextIndex = _runningPlugin.IndexInAllServiceRunnables + 1;
                Debug.Assert( nextIndex <= _allRunnables.Length );
                if( nextIndex == _allRunnables.Length )
                {
                    _runningPlugin = _finallyNotRunning ? null : _allRunnables[0];
                }
            }
            UpdateStatusFromRunningPlugin();
            if( ++_runningIndex == _runningCount )
            {
                _runningIndex = 0;
                return false;
            }
            return true;
        }

        private void UpdateStatusFromRunningPlugin()
        {
            Debug.Assert( ServiceInfo.IsDynamicService );
            // Mark all services as stopped if they are not RunningLocked.
            ServiceData s = _firstRunnableService;
            do
            {
                if( s._status != RunningStatus.RunningLocked ) s._status = RunningStatus.Stopped;
                s = s._nextRunnableService;
            }
            while( s != null );

            foreach( PluginData p in _allRunnables )
            {
                if( p == _runningPlugin )
                {
                    p.SetStatusFromService( RunningStatus.Running );
                    ServiceData started = p.Service;
                    while( started != null && started._status == RunningStatus.Stopped )
                    {
                        started._status = RunningStatus.Running;
                        started = started.Generalization;
                    }
                }
                else p.SetStatusFromService( RunningStatus.Stopped );
            }

        }
    }
}
