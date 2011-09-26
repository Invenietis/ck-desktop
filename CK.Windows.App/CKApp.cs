using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Diagnostics;
using System.Windows;
using System.Reflection;

namespace CK.Windows.App
{
    static public class CKApp
    {
        static CKAppParameters _params;

        static public CKAppParameters CurrentParameters { get { return _params; } }

        static public IDisposable Initialize( CKAppParameters app )
        {
            if( _params != null ) throw new InvalidOperationException( "initonce" );
            _params = app;

            // This is the mutex used as a flag for installer.
            // Since it is "Global\", it is shared among multiple terminal services client.
            // We keep a reference on it. It will be released at the end of the process.
            Mutex mutextForInstaller = new Mutex( false, _params.GlobalMutexName );
            // This unique Application key is "local": it is not shared among terminal server connections.
            bool isNew;
            Mutex mutex = new Mutex( true, @"Local\" + _params.LocalMutexName, out isNew );
            if( !isNew )
            {
                mutex.Dispose();
                return null;
            }
            Debug.Assert( _params.ApplicationDataPath.EndsWith( @"\" ) );
                
            // This call also initializes the CrashLogManager subsystem.
            AppRecoveryManager.Initialize( _params.ApplicationDataPath + @"CrashLogs\" );
            try
            {
                // Here is where handling any existing CrashLogDirectory must be processed.
                // If the logs are sent (or are to be sent), this directory must no more exist otherwise the user will
                // be prompted again next time the application runs.
                CrashLogManager.HandleExistingCrashLogs();
                
                // Handle existing updates after crash logs: this way the crash mechanism
                // can be updated!
                UpdateManager.Initialize( _params.UpdaterPath );
                if( !UpdateManager.LaunchExistingUpdater() )
                {
                    mutex.Dispose();
                    return null;
                }
                return mutex;
            }
            catch( Exception ex )
            {
                CrashLogManager.CreateNew().WriteLineException( ex );
                mutex.Dispose();
                return null;
            }
        }
        
        static public void Run( Func<Application> createAndInitializeApp )
        {
            SplashScreen splashScreen = new SplashScreen( Assembly.GetCallingAssembly(), "Views/splash.png" );
            splashScreen.Show( false );

            Application app = createAndInitializeApp();
            // Due to a bug in the SplashScreen control, we need to catch exceptions while trying to close it.
            try
            {
                splashScreen.Close( TimeSpan.FromSeconds( 0.3 ) );
            }
            catch
            {
                try
                {
                    splashScreen.Close( TimeSpan.Zero );
                }
                catch { }
            }
            app.Run();
        }

    }
}
