#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Demo\App.xaml.cs) is part of CiviKey. 
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
