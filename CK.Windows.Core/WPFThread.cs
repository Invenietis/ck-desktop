#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Core\WPFThread.cs) is part of CiviKey. 
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Threading;

namespace CK.Windows.Core
{
    public class WPFThread
    {
        public readonly Dispatcher Dispatcher;
        readonly object _lock;

        public WPFThread( string name )
        {
            _lock = new object();
            Thread t = new Thread( StartDispatcher );
            t.Name = name;
            t.SetApartmentState( ApartmentState.STA );
            lock( _lock )
            {
                t.Start();
                Monitor.Wait( _lock );
            }
            Dispatcher = Dispatcher.FromThread( t );
        }

        void StartDispatcher()
        {
            // This creates the Dispatcher and pushes the job.
            Dispatcher.CurrentDispatcher.BeginInvoke( (System.Action)DispatcherStarted, null );
            // Initializes a SynchronizationContext (for tasks ot other components that would require one). 
            SynchronizationContext.SetSynchronizationContext( new DispatcherSynchronizationContext( Dispatcher.CurrentDispatcher ) );
            Dispatcher.Run();
        }

        void DispatcherStarted()
        {
            lock( _lock )
            {
                Monitor.Pulse( _lock );
            }
        }
    }
}
