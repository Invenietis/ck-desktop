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

        void Check( IReadOnlyCollection<IPluginInfo> plugins, params string[] pluginFullNames )
        {
            Assert.That( plugins.Select( p => p.PluginFullName ).OrderBy( Util.FuncIdentity ), Is.EquivalentTo( pluginFullNames.OrderBy( Util.FuncIdentity ) ) );
        }


    }
}
