#region LGPL License
/*----------------------------------------------------------------------------
* This file (Tests\CK.Plugin.Runner.Tests\CheckServiceReferences.cs) is part of CiviKey. 
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
using CK.Plugin.Config;
using CK.Plugin.Hosting;
using NUnit.Framework;
using CK.Core;

namespace CK.Plugin.Runner
{

    [TestFixture]
    public class CheckServiceReferences
    {
        MiniContext _ctx;

        PluginRunner PluginRunner { get { return _ctx.PluginRunner; } }
        IConfigManager ConfigManager { get { return _ctx.ConfigManager; } }

        static Guid _implService = new Guid( "{C24EE3EA-F078-4974-A346-B34208221B35}" );

        [SetUp]
        public void Setup()
        {
            _ctx = MiniContext.CreateMiniContext( "BasicStartStop" );
        }

        [SetUp]
        [TearDown]
        public void Teardown()
        {
            TestBase.CleanupTestDir();
        }


        void CheckStartStop( Action beforeStart, Action afterStart, Action beforeStop, Action afterStop, bool startSucceed, bool stopSucceed, params Guid[] idToStart )
        {
            // Set a new user action --> start plugins
            for( int i = 0; i < idToStart.Length; i++ )
                ConfigManager.UserConfiguration.LiveUserConfiguration.SetAction( idToStart[i], ConfigUserAction.Started );

            if( beforeStart != null ) beforeStart();

            // So apply the change
            Assert.That( PluginRunner.Apply() == startSucceed );

            if( afterStart != null ) afterStart();

            // Set a new user action --> stop the plugin
            for( int i = 0; i < idToStart.Length; i++ )
            {
                ConfigManager.UserConfiguration.LiveUserConfiguration.SetAction( idToStart[i], ConfigUserAction.Stopped );
            }

            if( beforeStop != null ) beforeStop();

            // So apply the change
            Assert.That( PluginRunner.Apply() == stopSucceed );

            if( afterStop != null ) afterStop();
        }

        void CheckStartStop( Action beforeStart, Action afterStart, Action beforeStop, Action afterStop, params Guid[] idToStart )
        {
            CheckStartStop( beforeStart, afterStart, beforeStop, afterStop, true, true, idToStart );
        }

        void CheckStartAnotherStop( Action beforeStart, Action afterStart, Action beforeStop, Action afterStop, bool startSucceed, bool stopSucceed, Guid idToStart, Guid idToStop )
        {
            // Set a new user action --> start plugins
            _ctx.ConfigManager.UserConfiguration.LiveUserConfiguration.SetAction( idToStart, Config.ConfigUserAction.Started );

            if( beforeStart != null ) beforeStart();

            // So apply the change
            Assert.That( PluginRunner.Apply() == startSucceed );

            if( afterStart != null ) afterStart();

            // Set a new user action --> stop the plugin
            _ctx.ConfigManager.UserConfiguration.LiveUserConfiguration.SetAction( idToStop, Config.ConfigUserAction.Stopped );

            if( beforeStop != null ) beforeStop();

            // So apply the change
            Assert.That( PluginRunner.Apply() == stopSucceed );

            if( afterStop != null ) afterStop();
        }

        private void CheckReferenceToServiceC( bool mustBeNotNull )
        {
            Type t = AssemblyCache.FindLoadedTypeByAssemblyQualifiedName( "CK.Tests.Plugin.IReferenceServiceC, PluginNeedsServiceC" );
            Assert.That( t, Is.Not.Null );
            object o = PluginRunner.ServiceHost.GetRunningProxy( t );
            Assert.That( o, Is.Not.Null );
            object rawImpl = o.GetType().GetProperty( "RawImpl" ).GetValue( o, null );
            Assert.That( rawImpl, Is.Not.Null );
            if( mustBeNotNull )
            {
                Assert.That( rawImpl.GetType().GetProperty( "ServiceCIsNotNull" ).GetValue( rawImpl, null ), Is.True );
            }
            else
            {
                Assert.That( rawImpl.GetType().GetProperty( "ServiceCIsNotNull" ).GetValue( rawImpl, null ), Is.False );
            }
        }

        #region Check all types of service references with fully implemented service.

        [Test]
        /// <summary>
        /// A plugin needs (MustExistAndRun) a service implemented by another plugin.
        /// Check if the plugin that implement the service is auto started to fill the service reference.
        /// </summary>
        public void ServiceReference_Normal_MustExistAndRun()
        {
            Guid id = new Guid( PluginNeedsServiceCIdentifiers.NakedService_MEAR );

            TestBase.CopyPluginToTestDir( "ServiceC.dll" );
            TestBase.CopyPluginToTestDir( "ServiceC.Model.dll" );
            TestBase.CopyPluginToTestDir( "PluginNeedsServiceC.dll" );

            PluginRunner.Discoverer.Discover( TestBase.TestFolderDir, true );

            Action afterStart = () =>
            {
                // Check if the plugin is started, and if the plugin that implement the required service is started too.
                Assert.That( PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( id ) ) );
                Assert.That( PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( _implService ) ) );
                CheckReferenceToServiceC( mustBeNotNull: true );
            };
            Action afterStop = () =>
            {
                // Check if the plugin is stopped.
                Assert.That( !PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( id ) ) );
                // The plugin that implements the service is still running, we don't care about machine resources yet.
                Assert.That( PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( _implService ) ) );
            };

            CheckStartStop( null, afterStart, null, afterStop, id );
        }

        [Test]
        /// <summary>
        /// Start the service that needs the service, and then stop the service. Check that the plugin is stopped.
        /// </summary>
        public void ServiceReference_Normal_MustExistAndRun_ThenStopService()
        {
            Guid id = new Guid( PluginNeedsServiceCIdentifiers.NakedService_MEAR );

            TestBase.CopyPluginToTestDir( "ServiceC.dll" );
            TestBase.CopyPluginToTestDir( "ServiceC.Model.dll" );
            TestBase.CopyPluginToTestDir( "PluginNeedsServiceC.dll" );

            PluginRunner.Discoverer.Discover( TestBase.TestFolderDir, true );

            Action afterStart = () =>
            {
                // Check if the plugin is started, and that the plugin that implements the required service is started too.
                Assert.That( PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( id ) ) );
                Assert.That( PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( _implService ) ) );
                CheckReferenceToServiceC( mustBeNotNull: true );
            };

            CheckStartAnotherStop( null, afterStart, null, afterStart, true, false, id, _implService );

            // Then we try to stop the plugin (the one that needs the service)
            _ctx.ConfigManager.UserConfiguration.LiveUserConfiguration.SetAction( id, Config.ConfigUserAction.Stopped );
            Assert.That( PluginRunner.Apply() );
            Assert.That( !PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( id ) ) );
            Assert.That( !PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( _implService ) ) );
        }

        [Test]
        /// <summary>
        /// A plugin needs (MustExistAndRun) a IService{service} implemented by an other plugin.
        /// Check if the plugin that implement the service is auto started to fill the service reference.
        /// </summary>
        public void ServiceReference_IService_MustExistAndRun()
        {
            Guid id = new Guid( PluginNeedsServiceCIdentifiers.Service_MEAR );

            TestBase.CopyPluginToTestDir( "ServiceC.dll" );
            TestBase.CopyPluginToTestDir( "ServiceC.Model.dll" );
            TestBase.CopyPluginToTestDir( "PluginNeedsServiceC.dll" );

            PluginRunner.Discoverer.Discover( TestBase.TestFolderDir, true );

            Action afterStart = () =>
            {
                // Check if the plugin is started, and if the plugin that implement the required service is started too.
                Assert.That( PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( id ) ) );
                Assert.That( PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( _implService ) ) );
                CheckReferenceToServiceC( mustBeNotNull: true );
            };
            Action afterStop = () =>
            {
                // Check if the plugin is stopped.
                Assert.That( !PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( id ) ) );
                // The plugin that implements the service is still running, we don't care about machine resources yet.
                Assert.That( PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( _implService ) ) );
            };

            CheckStartStop( null, afterStart, null, afterStop, id );
        }

        [Test]
        /// <summary>
        /// Start the that needs the service. And then stop the service. Check that the plugin is stopped.
        /// </summary>
        public void ServiceReference_IService_MustExistAndRun_ThenStopService()
        {
            Guid id = new Guid( PluginNeedsServiceCIdentifiers.Service_MEAR );

            TestBase.CopyPluginToTestDir( "ServiceC.dll" );
            TestBase.CopyPluginToTestDir( "ServiceC.Model.dll" );
            TestBase.CopyPluginToTestDir( "PluginNeedsServiceC.dll" );

            PluginRunner.Discoverer.Discover( TestBase.TestFolderDir, true );

            Action afterStart = () =>
            {
                // Check if the plugin is started, and if the plugin that implement the required service is started too.
                Assert.That( PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( id ) ) );
                Assert.That( PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( _implService ) ) );
                CheckReferenceToServiceC( mustBeNotNull: true );
            };
            Action afterStop = () =>
            {
                // Check if the plugin is stopped.
                Assert.That( PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( id ) ) );
                Assert.That( PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( _implService ) ) );
            };

            CheckStartAnotherStop( null, afterStart, null, afterStop, true, false, id, _implService );
        }


        [Test]
        /// <summary>
        /// A plugin needs (MustExistTryStart) a service implemented by an other plugin.
        /// Check if the plugin that implement the service is auto started to fill the service reference.
        /// </summary>
        public void ServiceReference_Normal_MustExistTryStart()
        {
            #region Init
            
            Guid id = new Guid( "{58C00B79-D882-4C11-BD90-1F25AD664C67}" );

            TestBase.CopyPluginToTestDir( "ServiceC.dll" );
            TestBase.CopyPluginToTestDir( "ServiceC.Model.dll" );
            TestBase.CopyPluginToTestDir( "PluginNeedsServiceC.dll" );

            PluginRunner.Discoverer.Discover( TestBase.TestFolderDir, true );

            #endregion

            #region Asserts
            Action afterStart = () =>
            {
                // Check if the plugin is started, and that the plugin that implements the required service is started too.
                Assert.That( PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( id ) ) );
                // The plugin is available ... so we tried to start it.
                Assert.That( PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( _implService ) ) );
                // The reference is okay.
                CheckReferenceToServiceC( mustBeNotNull: true );
            };
            Action afterStop = () =>
            {
                // Check if the plugin is stopped.
                Assert.That( !PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( id ) ) );
                // The plugin that implements the service is still running, we don't care about machine resources yet.
                Assert.That( PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( _implService ) ) );
            };
            #endregion

            //Run!
            CheckStartStop( null, afterStart, null, afterStop, id );
        }

        [Test]
        /// <summary>
        /// A plugin needs (MustExistTryStart) a IService{service} implemented by an other plugin.
        /// Check if the plugin that implement the service is auto started to fill the service reference.
        /// </summary>
        public void ServiceReference_IService_MustExistTryStart()
        {
            #region Init
            Guid id = new Guid( PluginNeedsServiceCIdentifiers.Service_METS );

            TestBase.CopyPluginToTestDir( "ServiceC.dll" );
            TestBase.CopyPluginToTestDir( "ServiceC.Model.dll" );
            TestBase.CopyPluginToTestDir( "PluginNeedsServiceC.dll" );

            PluginRunner.Discoverer.Discover( TestBase.TestFolderDir, true ); 
            #endregion

            #region Asserts
            Action afterStart = () =>
                {
                    // Check if the plugin is started, and that the plugin that implements the required service is started too.
                    Assert.That( PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( id ) ) );
                    // The plugin is available ... so we tried to start it.
                    Assert.That( PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( _implService ) ) );
                    // The reference is okay.
                    CheckReferenceToServiceC( mustBeNotNull: true );
                };
            Action afterStop = () =>
            {
                // Check if the plugin is stopped.
                Assert.That( !PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( id ) ) );
                // The plugin that implements the service is still running, we don't care about machine resources yet.
                Assert.That( PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( _implService ) ) );
            }; 
            #endregion

            //Run!
            CheckStartStop( null, afterStart, null, afterStop, id );
        }

        [Test]
        /// <summary>
        /// A plugin needs (MustExistTryStart) a service implemented by an other plugin.
        /// Check if the plugin that implement the service is auto started to fill the service reference.
        /// </summary>
        public void ServiceReference_Normal_MustExist()
        {
            #region Init

            Guid id = new Guid( PluginNeedsServiceCIdentifiers.NakedService_ME );

            TestBase.CopyPluginToTestDir( "ServiceC.dll" );
            TestBase.CopyPluginToTestDir( "ServiceC.Model.dll" );
            TestBase.CopyPluginToTestDir( "PluginNeedsServiceC.dll" );

            PluginRunner.Discoverer.Discover( TestBase.TestFolderDir, true );

            #endregion

            #region Asserts
            Action afterStart = () =>
            {
                // Check if the plugin is started, and if the plugin that implement the required service is started too.
                Assert.That( PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( id ) ) );
                // The plugin is available but we did not start it.
                Assert.That( !PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( _implService ) ) );
                // The reference to the non running plugin is null (since IService<> is not used as the reference).
                CheckReferenceToServiceC( mustBeNotNull: false );
            };
            Action afterStop = () =>
            {
                // Check if the plugin is stopped.
                Assert.That( !PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( id ) ) );
                // The plugin that implements the service was not running.
                Assert.That( !PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( _implService ) ) );
            };
            #endregion

            //Run!
            CheckStartStop( null, afterStart, null, afterStop, id );
        }

        [Test]
        /// <summary>
        /// A plugin needs (MustExist) a IService{service} implemented by an other plugin.
        /// Referenced service must exist but not be started.
        /// </summary>
        public void ServiceReference_IService_MustExist()
        {
            #region Init
            Guid id = new Guid( PluginNeedsServiceCIdentifiers.Service_ME );

            TestBase.CopyPluginToTestDir( "ServiceC.dll" );
            TestBase.CopyPluginToTestDir( "ServiceC.Model.dll" );
            TestBase.CopyPluginToTestDir( "PluginNeedsServiceC.dll" );

            PluginRunner.Discoverer.Discover( TestBase.TestFolderDir, true );
            #endregion

            #region Asserts
            Action afterStart = () =>
            {
                // Checks that the plugin is started, and that the plugin that implements the required service is NOT started.
                Assert.That( PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( id ) ) );
                Assert.That( !PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( _implService ) ) );
                // The reference to the non running plugin is NOT null (since IService<> is used as the reference).
                CheckReferenceToServiceC( mustBeNotNull: true );
            };
            Action afterStop = () =>
            {
                // Check if the plugin is stopped.
                Assert.That( !PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( id ) ) );
                Assert.That( !PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( _implService ) ) );
            };
            #endregion

            //Run!
            CheckStartStop( null, afterStart, null, afterStop, id );
        }

        [Test]
        /// <summary>
        /// A plugin needs (MustExistTryStart) a service implemented by an other plugin.
        /// Check if the plugin that implement the service is auto started to fill the service reference.
        /// </summary>
        public void ServiceReference_Normal_OptionalTryStart()
        {
            #region Init

            Guid id = new Guid( PluginNeedsServiceCIdentifiers.NakedService_OTS );

            TestBase.CopyPluginToTestDir( "ServiceC.dll" );
            TestBase.CopyPluginToTestDir( "ServiceC.Model.dll" );
            TestBase.CopyPluginToTestDir( "PluginNeedsServiceC.dll" );

            PluginRunner.Discoverer.Discover( TestBase.TestFolderDir, true );

            #endregion

            #region Asserts
            Action afterStart = () =>
            {
                // Checks that the plugin is started, and that the plugin that implements the required service is started too.
                Assert.That( PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( id ) ) );
                Assert.That( PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( _implService ) ) );
                // The referenced plugin is started: we have a reference to it.
                CheckReferenceToServiceC( mustBeNotNull: true );
            };
            Action afterStop = () =>
            {
                // Check if the plugin is stopped.
                Assert.That( !PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( id ) ) );
                Assert.That( PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( _implService ) ) );
            };
            #endregion

            //Run!
            CheckStartStop( null, afterStart, null, afterStop, id );
        }

        [Test]
        /// <summary>
        /// A plugin needs (MustExistTryStart) a IService{service} implemented by an other plugin.
        /// Check if the plugin that implement the service is auto started to fill the service reference.
        /// </summary>
        public void ServiceReference_IService_OptionalTryStart()
        {
            #region Init
            Guid id = new Guid( PluginNeedsServiceCIdentifiers.Service_OTS );

            TestBase.CopyPluginToTestDir( "ServiceC.dll" );
            TestBase.CopyPluginToTestDir( "ServiceC.Model.dll" );
            TestBase.CopyPluginToTestDir( "PluginNeedsServiceC.dll" );

            PluginRunner.Discoverer.Discover( TestBase.TestFolderDir, true );
            #endregion

            #region Asserts
            Action afterStart = () =>
            {
                // Checks that the plugin is started, and that the plugin that implements the required service is started too.
                Assert.That( PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( id ) ) );
                Assert.That( PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( _implService ) ) );
                CheckReferenceToServiceC( mustBeNotNull: true );
            };
            Action afterStop = () =>
            {
                // Check if the plugin is stopped.
                Assert.That( !PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( id ) ) );
                Assert.That( PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( _implService ) ) );
            };
            #endregion

            //Run!
            CheckStartStop( null, afterStart, null, afterStop, id );
        }

        [Test]
        /// <summary>
        /// A plugin needs (Optional) a service implemented by an other plugin.
        /// Check if the plugin that implement the service is auto started to fill the service reference.
        /// </summary>
        public void ServiceReference_Normal_Optional()
        {
            #region Init

            Guid id = new Guid( PluginNeedsServiceCIdentifiers.NakedService_O );

            TestBase.CopyPluginToTestDir( "ServiceC.dll" );
            TestBase.CopyPluginToTestDir( "ServiceC.Model.dll" );
            TestBase.CopyPluginToTestDir( "PluginNeedsServiceC.dll" );

            PluginRunner.Discoverer.Discover( TestBase.TestFolderDir, true );

            #endregion

            #region Asserts
            Action afterStart = () =>
            {
                // Checks that the plugin is started, and that the plugin that implements the required service is NOT started.
                Assert.That( PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( id ) ) );
                Assert.That( !PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( _implService ) ) );
                // The reference to the non running plugin is null (since IService<> is not used as the reference).
                CheckReferenceToServiceC( mustBeNotNull: false );
            };
            Action afterStop = () =>
            {
                // Check if the plugin is stopped.
                Assert.That( !PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( id ) ) );
                Assert.That( !PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( _implService ) ) );
            };
            #endregion

            //Run!
            CheckStartStop( null, afterStart, null, afterStop, id );
        }

        [Test]
        /// <summary>
        /// A plugin needs (Optional) a IService{service} implemented by an other plugin.
        /// Check if the plugin that implement the service is auto started to fill the service reference.
        /// </summary>
        public void ServiceReference_IService_Optional()
        {
            #region Init
            Guid id = new Guid( PluginNeedsServiceCIdentifiers.Service_O );

            TestBase.CopyPluginToTestDir( "ServiceC.dll" );
            TestBase.CopyPluginToTestDir( "ServiceC.Model.dll" );
            TestBase.CopyPluginToTestDir( "PluginNeedsServiceC.dll" );

            PluginRunner.Discoverer.Discover( TestBase.TestFolderDir, true );
            #endregion

            #region Asserts
            Action afterStart = () =>
            {
                // Check if the plugin is started, and if the plugin that implement the required service is started too.
                Assert.That( PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( id ) ) );
                Assert.That( !PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( _implService ) ) );
                // The reference to the non running plugin is NOT null (since IService<> is used as the reference).
                CheckReferenceToServiceC( mustBeNotNull: true );
            };
            Action afterStop = () =>
            {
                // Check if the plugin is stopped.
                Assert.That( !PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( id ) ) );
                Assert.That( !PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( _implService ) ) );
            };
            #endregion

            //Run!
            CheckStartStop( null, afterStart, null, afterStop, id );
        }

        #endregion

        [Test]
        /// <summary>
        /// Checks all types of service references with a not implemented service.
        /// </summary>
        public void ServiceReference_WhenNoServiceReferenceImplementationExists()
        {
            TestBase.CopyPluginToTestDir( "ServiceC.Model.dll" );
            TestBase.CopyPluginToTestDir( "PluginNeedsServiceC.dll" );
            PluginRunner.Discoverer.Discover( TestBase.TestFolderDir, true );

            CheckNotStart( PluginNeedsServiceCIdentifiers.NakedService_MEAR );
            CheckNotStart( PluginNeedsServiceCIdentifiers.NakedService_ME );
            CheckNotStart( PluginNeedsServiceCIdentifiers.NakedService_METS );
            CheckStarted( PluginNeedsServiceCIdentifiers.NakedService_OTS, referenceMustBeNotNull: false );
            CheckStarted( PluginNeedsServiceCIdentifiers.NakedService_O, referenceMustBeNotNull: false );

            CheckNotStart( PluginNeedsServiceCIdentifiers.Service_MEAR );
            CheckNotStart( PluginNeedsServiceCIdentifiers.Service_ME );
            CheckNotStart( PluginNeedsServiceCIdentifiers.Service_METS );
            CheckStarted( PluginNeedsServiceCIdentifiers.Service_OTS, referenceMustBeNotNull: true );
            CheckStarted( PluginNeedsServiceCIdentifiers.Service_O, referenceMustBeNotNull: true );
        }

        private void CheckNotStart( string pluginId )
        {
            Guid id = new Guid( pluginId );

            Action beforeAndAfterStart = () =>
            {
                // Checks that the plugin is always stopped.
                Assert.That( !PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( id ) ) );
            };

            //Run!
            CheckStartStop( null, beforeAndAfterStart, null, beforeAndAfterStart, false, true, id );
        }

        private void CheckStarted( string pluginId, bool referenceMustBeNotNull )
        {
            Guid id = new Guid( pluginId );

            Action afterStart = () =>
            {
                // Checks that the plugin is always stopped.
                Assert.That( PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( id ) ) );
                CheckReferenceToServiceC( referenceMustBeNotNull );
            };

            Action afterStop = () =>
            {
                // Checks that the plugin is always stopped.
                Assert.That( !PluginRunner.IsPluginRunning( PluginRunner.Discoverer.FindPlugin( id ) ) );
            };

            //Run!
            CheckStartStop( null, afterStart, null, afterStop, true, true, id );
        }

     }
}