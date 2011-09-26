using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace CK.Windows.App
{
    public class CKAppParameters
    {
        string _commonAppData;
        string _appDataPath;
        string _updaterPath;

        /// <summary>
        /// Initializes a new <see cref="CKAppParameters"/> with an application name and an optional subordinated name. 
        /// These are used to build the <see cref="ApplicationDataPath"/> and <see cref="CommonApplicationDataPath"/>.
        /// This constructor does no more than validating its parameters: it is totally safe and secure as long as <paramref name="appName"/> 
        /// and <see cref="subAppName"/> are valid.
        /// </summary>
        /// <param name="appName">
        /// Name of the application (Civikey-Standard for instance for the Civikey Standard application). 
        /// Must be an indentifier (no /, \ or other special characters in it: see <see cref="Path.GetInvalidPathChars"/>).
        /// </param>
        /// <param name="subAppName">Optional second name (can be null). When not null, it must be an identifier just like <paramref name="appName"/>.</param>
        public CKAppParameters( string appName, string subAppName )
        {
            char[] illegal = Path.GetInvalidPathChars();
            if( String.IsNullOrEmpty( appName ) ) throw new ArgumentNullException( "appName" );
            if( appName.IndexOf( '/' ) >= 0 || appName.IndexOf( '\\' ) >= 0 ) throw new ArgumentException( "appName" );
            if( subAppName != null )
            {
                subAppName = subAppName.Trim();
                if( subAppName.Length == 0 )
                    subAppName = null;
                else
                {
                    if( subAppName.IndexOf( '/' ) >= 0 || subAppName.IndexOf( '\\' ) >= 0 ) throw new ArgumentException( "subAppName" );
                    if( subAppName.Any( c => illegal.Contains( c ) ) ) throw new ArgumentException( "subAppName" );
                }
            }
            if( appName.Any( c => illegal.Contains( c ) ) ) throw new ArgumentException( "appName" );

            AppName = appName;
            SubAppName = subAppName;
        }

        /// <summary>
        /// Builds the path with <see cref="AppName"/> and <see cref="SubAppName"/> and ensures that the folder exists.
        /// </summary>
        /// <param name="pathPrefix">
        /// Typically <see cref="Environment.GetFolderPath"/> called 
        /// with <see cref="Environment.SpecialFolder.CommonApplicationData"/> or <see cref="Environment.SpecialFolder.ApplicationData"/>.
        /// </param>
        /// <returns>The path.</returns>
        protected virtual string EnsureStandardPath( string pathPrefix )
        {
            if( String.IsNullOrWhiteSpace( pathPrefix ) ) throw new ArgumentException( "pathPrefix" );
            if( pathPrefix[pathPrefix.Length - 1] != Path.DirectorySeparatorChar ) pathPrefix += Path.DirectorySeparatorChar;
            string p = pathPrefix
                    + AppName
                    + Path.DirectorySeparatorChar;
            if( SubAppName != null )
                p += SubAppName + Path.DirectorySeparatorChar;
            if( !Directory.Exists( p ) ) Directory.CreateDirectory( p );
            return p;
        }

        /// <summary>
        /// Gets the full path of application-specific data repository, for the current user.
        /// Ends with <see cref="Path.DirectorySeparatorChar"/>.
        /// The directory is created if it does not exist.
        /// </summary>
        public string ApplicationDataPath
        {
            get { return _appDataPath ?? (_appDataPath = EnsureStandardPath( Environment.GetFolderPath( Environment.SpecialFolder.ApplicationData ) )); }
        }

        /// <summary>
        /// Gets the full path of application-specific data repository, for all users.
        /// Ends with <see cref="Path.DirectorySeparatorChar"/>.
        /// The directory is created if it does not exist.
        /// </summary>
        public string CommonApplicationDataPath
        {
            get { return _commonAppData ?? (_commonAppData = EnsureStandardPath( Environment.GetFolderPath( Environment.SpecialFolder.CommonApplicationData ) ) ); }
        }

        /// <summary>
        /// Gets the name of the application. Civikey-Standard for instance for the Civikey Standard application. 
        /// It is an indentifier (no /, \ or other special characters in it: see <see cref="Path.GetInvalidPathChars"/>).
        /// </summary>
        public string AppName { get; private set; }

        /// <summary>
        /// Gets an optional second name (can be null).
        /// When not null, it is an identifier just like <see cref="AppName"/>.
        /// </summary>
        public string SubAppName { get; private set; }

        /// <summary>
        /// Unique name (Global\Install-AppName-SubAppName) that identifies the application and that can be 
        /// used by installers to detect a running instance.
        /// </summary>
        public string GlobalMutexName { get { return @"Global\Install-" + AppName + "-" + SubAppName; } }

        /// <summary>
        /// Unique name (Local\AppName-SubAppName) that identifies the application and that can 
        /// be used to avoid multiple application instance.
        /// </summary>
        public string LocalMutexName { get { return @"Local\" + AppName + "-" + SubAppName; } }

        /// <summary>
        /// Gets the full directory path to use for application updates. 
        /// </summary>
        public string UpdaterPath { get { return _updaterPath ?? (_updaterPath = EnsureStandardPath( Path.GetTempPath() )); } }

    }
}
