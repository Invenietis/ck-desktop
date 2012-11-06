using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using CK.Plugin.Hosting;

namespace CK.Plugin.Runner.Tests.Planner
{
    [TestFixture]
    public class SimpleStructure
    {
        [Test]
        public void BuggyStart()
        {
            DiscovererDesc d = new DiscovererDesc();
            d.Service( "SKeyboardTrigger" );
            d.Service( "SKeyboardDriver" );
            d.Service( "SBasicScroll" );
            d.Plugin( "KeyboardTrigger", "SKeyboardTrigger" ).AddRef( "SKeyboardDriver", RunningRequirement.MustExistAndRun );
            d.Plugin( "BasicScroll", "SBasicScroll" ).AddRef( "SKeyboardDriver", RunningRequirement.MustExistAndRun );
            d.Plugin( "KeyboardDriver", "SKeyboardDriver" );
            d.Plugin( "Skin" ).AddRef( "SBasicScroll", RunningRequirement.Optional );

            d.SetFinalConfig( "Skin", SolvedConfigStatus.MustExistAndRun );

            ConfigurationSolver solver = new ConfigurationSolver( p => false );
            ConfigurationSolverResult result = solver.Initialize( d.FinalConfig, PlanCalculatorStrategy.HonorConfigAndReferenceTryStart, d.AllServiceInfo, d.AllPluginInfo );

            Assert.That( result.ConfigurationSuccess, Is.True );
            result.LiveInfo.FindPlugin( "Skin" );


        }



    }
}
