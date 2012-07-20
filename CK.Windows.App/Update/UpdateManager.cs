using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Windows;
using System.Diagnostics;

namespace CK.Windows.App
{
    /// <summary>
    /// Very simple implementation of an update system that relies on external updater program.
    /// </summary>
    static public class UpdateManager
    {
        static string _dir;

        /// <summary>
        /// Gets the directory used to store and lookup for available updates. 
        /// </summary>
        public static string UpdateDirectory { get { return _dir; } }

        public static void Initialize( string updateDirectory )
        {
            _dir = updateDirectory;
        }

        public static bool LaunchExistingUpdater()
        {
            string updateFile = _dir + "Updates\\Update.exe";
            string isUdpateDone = _dir + "UpdateDone";
            if( File.Exists( isUdpateDone ) )
            {
                if( File.Exists( updateFile ) ) File.Delete( updateFile );
                File.Delete( isUdpateDone );
            }
            if( File.Exists( updateFile ) )
            {
                if( MessageBox.Show( Update.R.UpdateMessage, Update.R.Update, MessageBoxButton.YesNo ) == MessageBoxResult.Yes )
                {
                    Process.Start( updateFile );
                    return false;
                }
            }
            return true;
        }
    }
}
