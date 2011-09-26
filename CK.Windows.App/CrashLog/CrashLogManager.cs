using System;
using System.IO;
using System.Text;

namespace CK.Windows.App
{

    /// <summary>
    /// Offers crash log management. 
    /// </summary>
    static public class CrashLogManager
    {
        static string _dir;

        /// <summary>
        /// Must be called once and only once at the very beginning of the application.
        /// </summary>
        /// <param name="crashLogDirectory">The crash log directory to use.</param>
        public static void Initialize( string crashLogDirectory )
        {
            if( String.IsNullOrWhiteSpace( crashLogDirectory ) ) throw new ArgumentException( "crashLogDirectory" ); 
            if( _dir != null ) throw new InvalidOperationException();
            _dir = crashLogDirectory;
        }

        /// <summary>
        /// Gets the crash log directory. Initialized by <see cref="Initialize"/>.
        /// </summary>
        public static string CrashLogDirectory { get { return _dir; } }

        /// <summary>
        /// Displays a window that enables the user to upload any existing
        /// crash logs.
        /// </summary>
        public static void HandleExistingCrashLogs()
        {
            string crashPath = CrashLogDirectory;
            if( !Directory.Exists( _dir ) ) return;

            CrashLogWindow w = new CrashLogWindow( new CrashLogWindowViewModel( crashPath ) );
            w.ShowDialog();

            if( Directory.GetFiles( crashPath ).Length == 0 )
            {
                Directory.Delete( crashPath, true );
            }
        }

        /// <summary>
        /// Obtains a new <see cref="CrashLogWriter"/> to write crash information. 
        /// </summary>
        /// <param name="crashFilePrefix">Can be used to "scope" crash files. Defaults to "crashLog".</param>
        /// <returns>Always returns a non null CrashLogWriter (be it bound to a <see cref="TextWriter.Null"/> if something really bad happens).</returns>
        static public CrashLogWriter CreateNew( string crashFilePrefix = "crashLog" )
        {
            StreamWriter w = null;
            try
            {
                if( !Directory.Exists( CrashLogDirectory ) ) Directory.CreateDirectory( CrashLogDirectory );
                string date = DateTime.UtcNow.ToString( "u" );
                string path = Path.Combine( CrashLogDirectory, String.Format( "{0}-{1}.log", crashFilePrefix, date.Replace( ':', '-' ) ) );
                w = new StreamWriter( path, true, Encoding.UTF8 );
                w.AutoFlush = true;
                CrashLogWriter.WriteLineProperty( w, "UniqueID", Guid.NewGuid().ToString() );
                CrashLogWriter.WriteLineProperty( w, "UtcDate", date );
                return new CrashLogWriter( w );
            }
            catch( Exception )
            {
                try { if( w != null ) w.Dispose(); }
                catch { }
                return new CrashLogWriter( TextWriter.Null );
            }
        }
    }
}
