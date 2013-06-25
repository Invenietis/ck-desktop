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
