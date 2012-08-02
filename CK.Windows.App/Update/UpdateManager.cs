using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Diagnostics;
using System.Security.AccessControl;
using System.Security.Principal;

namespace CK.Windows.App
{
    /// <summary>
    /// Very simple implementation of an update system that relies on external updater program.
    /// </summary>
    static public class UpdateManager
    {
        static string _sharedDir;
        static string _privateDir;

        /// <summary>
        /// Gets the directory used to store and lookup for available updates. 
        /// </summary>
        public static string SharedUpdateDirectory { get { return _sharedDir; } }

        /// <summary>
        /// Gets the personnal directory of the current user.
        /// Used to check whether he wants to be reminded of the fact that a new update is available.
        /// </summary>
        public static string PrivateUpdateDirectory { get { return _privateDir; } }

        /// <summary>
        /// Intializes the UpdateManager by setting the shared and private directories used to put and retrieve files necessary to handle the automatic updates
        /// </summary>
        /// <param name="sharedUpdateDirectory">The path to a folder shared among users. The UpdateManager will put the update.exe file and UpdateDone files in this folder.</param>
        /// <param name="privateUpdateDirectory">The path to a private folder. The UpdateManager will put in this folder the infos concerning the current user, like the fact that he doesn't want ot be reminded of new available updates.</param>
        public static void Initialize( string sharedUpdateDirectory, string privateUpdateDirectory )
        {
            _sharedDir = sharedUpdateDirectory;
            _privateDir = privateUpdateDirectory;
        }

        /// <summary>
        /// Handles the launching of the downloaded updates.
        /// Aksks the user if he wants to launch an available update. If yes, executes the updates.exe found in the sharedUpdateDirectory set in the Initialize phase.
        /// Handles the deleting of files when the update has been done (when an UpdateDone file can be found in the sharedUpdateDirectory)
        /// </summary>
        /// <returns></returns>
        public static bool LaunchExistingUpdater()
        {
            string updateFile = _sharedDir + "Updates\\Update.exe";
            string isUdpateDone = _sharedDir + "Updates\\UpdateDone";
            string stopReminderFile = _privateDir + "Updates\\StopReminder";

            if( File.Exists( isUdpateDone ) )
            {
                if( File.Exists( updateFile ) ) File.Delete( updateFile );
                if( File.Exists( stopReminderFile ) ) File.Delete( stopReminderFile );
                File.Delete( isUdpateDone );
            }
            if( File.Exists( updateFile ) && !File.Exists( stopReminderFile ) )
            {
                MsgBox b = new MsgBox( Update.R.UpdateMessage, Update.R.Update );
                b.SetCheckbox( Update.R.RememberMyDecision );
                b.SetButtons( Update.R.Yes, Update.R.No );
                b.ShowDialog();

                DialogBoxResult result = b.DialogBoxResult;

                if( result == DialogBoxResult.Button1 )
                {
                    Process.Start( updateFile );
                    return false;
                }
                else if( b.CheckboxChecked )
                {
                    string updateDir = Path.Combine( _privateDir, "Updates" );
                    if( !Directory.Exists( updateDir ) ) Directory.CreateDirectory( updateDir );

                    File.Create( stopReminderFile );
                    FileSecurity f = File.GetAccessControl( stopReminderFile );
                    var sid = new SecurityIdentifier( WellKnownSidType.BuiltinUsersSid, null );
                    NTAccount account = (NTAccount)sid.Translate( typeof( NTAccount ) );
                    f.AddAccessRule( new FileSystemAccessRule( account, FileSystemRights.Modify, AccessControlType.Allow ) );
                    File.SetAccessControl( stopReminderFile, f );
                }
            }
            return true;
        }
    }
}
