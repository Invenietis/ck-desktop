using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Windows;
using System.Threading;
using System.Diagnostics;
using CK.Windows.App;
using System.IO;

namespace CK.Windows.Demo
{
    /// <summary>
    /// Main code is centralized in <see cref="CKApp"/>.
    /// To define the <see cref="Main"/> here, the "BuildAction" of the App.xaml must be set to "Page".
    /// </summary>
    public partial class App : Application
    {
        public static new App Current { get { return (App)Application.Current; } }

        [STAThread]
        public static void Main( string[] args )
        {
            try
            {
                // Crash logs upload and updater availability is managed during this initialization.
                using( var init = CKApp.Initialize( new CKAppParameters( "CK-Windows", "Demo" ) ) )
                {
                    if( init != null )
                    {
                        // Common logger is actually bound to log4net.
                        // CK-Windows must not depend on log4Net: its initialization must be done here.
                        CommonLogger.Initialize( CKApp.CurrentParameters.ApplicationDataPath + @"AppLogs\", false );
                        CKApp.Run( () =>
                        {
                            App app = new App();
                            app.InitializeComponent();
                            return app;
                        } );
                    }
                }
            }
            catch( Exception ex )
            {
                Console.WriteLine( ex.Message );
            }
        }

    }
}
