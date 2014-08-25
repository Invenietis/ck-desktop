#region LGPL License
/*----------------------------------------------------------------------------
* This file (Tests\CK.Plugin.Runner.Tests\Planner\SimpleStructure.cs) is part of CiviKey. 
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
using NUnit.Framework;
using CK.Plugin.Hosting;
using CK.Core;

namespace CK.Plugin.Runner.Tests.Planner
{
    [TestFixture]
    public class SimpleStructure
    {
        [Test]
        public void BuggyStart()
        {
            DiscovererStub d = SkinAndKeyboardStructure();

            d.SetFinalConfig( "Skin", SolvedConfigStatus.MustExistAndRun );
            {
                ConfigurationSolver solver = new ConfigurationSolver( p => false );
                IConfigurationSolverResult result = solver.Initialize( d.FinalConfig, PlanCalculatorStrategy.HonorConfigAndReferenceTryStart, d.AllServiceInfo, d.AllPluginInfo );
                Assert.That( result.ConfigurationSuccess );
                Check( result.RunningPlugins, "Skin" );
                Check( result.StoppedPlugins, "KeyboardDriver", "BasicScroll", "KeyboardTrigger" );
                Check( result.DisabledPlugins );
            }
            d.SetFinalConfig( "BasicScroll", SolvedConfigStatus.MustExistAndRun );
            {
                ConfigurationSolver solver = new ConfigurationSolver( p => false );
                IConfigurationSolverResult result = solver.Initialize( d.FinalConfig, PlanCalculatorStrategy.HonorConfigAndReferenceTryStart, d.AllServiceInfo, d.AllPluginInfo );
                Assert.That( result.ConfigurationSuccess );
                Check( result.RunningPlugins, "Skin", "BasicScroll", "KeyboardDriver" );
                Check( result.StoppedPlugins, "KeyboardTrigger" );
                Check( result.DisabledPlugins );
            }
            d.SetFinalConfig( "BasicScroll", SolvedConfigStatus.Optional );
            d.SetFinalConfig( "SBasicScroll", SolvedConfigStatus.MustExistAndRun );
            {
                ConfigurationSolver solver = new ConfigurationSolver( p => false );
                IConfigurationSolverResult result = solver.Initialize( d.FinalConfig, PlanCalculatorStrategy.HonorConfigAndReferenceTryStart, d.AllServiceInfo, d.AllPluginInfo );
                Assert.That( result.ConfigurationSuccess );
                Check( result.RunningPlugins, "Skin", "BasicScroll", "KeyboardDriver" );
                Check( result.StoppedPlugins, "KeyboardTrigger" );
                Check( result.DisabledPlugins );
            }
            d.SetFinalConfig( "SBasicScroll", SolvedConfigStatus.Optional );
            d.SetFinalConfig( "BasicScroll", SolvedConfigStatus.OptionalTryStart );
            {
                ConfigurationSolver solver = new ConfigurationSolver( p => false );
                IConfigurationSolverResult result = solver.Initialize( d.FinalConfig, PlanCalculatorStrategy.HonorConfigAndReferenceTryStart, d.AllServiceInfo, d.AllPluginInfo );
                Assert.That( result.ConfigurationSuccess );
                Check( result.RunningPlugins, "Skin", "BasicScroll", "KeyboardDriver" );
                Check( result.StoppedPlugins, "KeyboardTrigger" );
                Check( result.DisabledPlugins );
            }
            d.SetFinalConfig( "BasicScroll", SolvedConfigStatus.Optional );
            d.SetFinalConfig( "SBasicScroll", SolvedConfigStatus.OptionalTryStart );
            {
                ConfigurationSolver solver = new ConfigurationSolver( p => false );
                IConfigurationSolverResult result = solver.Initialize( d.FinalConfig, PlanCalculatorStrategy.HonorConfigAndReferenceTryStart, d.AllServiceInfo, d.AllPluginInfo );
                Assert.That( result.ConfigurationSuccess );
                Check( result.RunningPlugins, "Skin", "BasicScroll", "KeyboardDriver" );
                Check( result.StoppedPlugins, "KeyboardTrigger" );
                Check( result.DisabledPlugins );
            }  
        }

        //
        // Skin =OPT=> SBasicScroll
        //             BasicScroll =MER=> SKeyboardDriver
        // KeyboardTrigger =MER=> SKeyboardDriver
        //
        private static DiscovererStub SkinAndKeyboardStructure()
        {
            DiscovererStub d = new DiscovererStub();
            d.Service( "SKeyboardTrigger" );
            d.Service( "SKeyboardDriver" );
            d.Service( "SBasicScroll" );
            d.Plugin( "KeyboardTrigger", "SKeyboardTrigger" ).AddRef( "SKeyboardDriver", RunningRequirement.MustExistAndRun );
            d.Plugin( "BasicScroll", "SBasicScroll" ).AddRef( "SKeyboardDriver", RunningRequirement.MustExistAndRun );
            d.Plugin( "KeyboardDriver", "SKeyboardDriver" );
            d.Plugin( "Skin" ).AddRef( "SBasicScroll", RunningRequirement.Optional );
            return d;
        }

        void Check( ICKReadOnlyCollection<IPluginInfo> plugins, params string[] pluginFullNames )
        {
            Assert.That( plugins.Select( p => p.PluginFullName ).OrderBy( Util.FuncIdentity ), Is.EquivalentTo( pluginFullNames.OrderBy( Util.FuncIdentity ) ) );
        }


    }
}
