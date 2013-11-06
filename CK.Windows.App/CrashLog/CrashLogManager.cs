#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.App\CrashLog\CrashLogManager.cs) is part of CiviKey. 
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
* Copyright © 2007-2012, 
*     Invenietis <http://www.invenietis.com>,
*     In’Tech INFO <http://www.intechinfo.fr>,
* All rights reserved. 
*-----------------------------------------------------------------------------*/
#endregion

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
        static string _crashUploadUrl;

        /// <summary>
        /// Must be called once and only once at the very beginning of the application.
        /// </summary>
        /// <param name="crashLogDirectory">The crash log directory to use.</param>
        /// <param name="crashUploadUrl">The end point url to the web service that can receive crash logs</param>
        public static void Initialize( string crashLogDirectory, string crashUploadUrl )
        {
            if( String.IsNullOrWhiteSpace( crashLogDirectory ) ) throw new ArgumentException( "crashLogDirectory" );
            if( _dir != null ) throw new InvalidOperationException();
            if( String.IsNullOrWhiteSpace( crashUploadUrl ) ) throw new ArgumentException( "crashUploadUrl" );
            if( _crashUploadUrl != null ) throw new InvalidOperationException();
            
            _dir = crashLogDirectory;
            _crashUploadUrl = crashUploadUrl;
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

            CrashLogWindow w = new CrashLogWindow( new CrashLogWindowViewModel( crashPath, _crashUploadUrl ) );
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
