using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;
using CK.Windows;
using CK.Windows.Core;

namespace CK.Windows
{
    public class NoFocusManager : IDisposable
    {
        static NoFocusManager _default;

        public static NoFocusManager Default
        {
            get { return _default ?? (_default = new NoFocusManager()); }
        }

        /// <summary>
        /// Create a CKWindow in the default NoFocus Thread.
        /// Invokes the function set as parameter in the CKWindow Thread
        /// </summary>
        /// <typeparam name="T">The type of the window, must inherit from <see cref="CKWindow"/></typeparam>
        /// <param name="createFunction">The function that launches the "new" of the window</param>
        /// <returns>An object of the type set as parameter, created in the CKWindow Thread</returns>
        public T Create<T>( Func<NoFocusManager, T> createFunction )
            where T : CKNoFocusWindow
        {
            return Default.CreateNoFocusWindow( createFunction );
        }


        readonly Dispatcher _innerDispatcher;
        readonly Dispatcher _externalDispatcher;

        public NoFocusManager( string name = "NoFocusManager" )
        {
            // 
            if( Application.Current.Dispatcher != Dispatcher.CurrentDispatcher )
            {
                throw new NotSupportedException( "Current implementation only support that NoFocusManager be created from the main Application." );
            }
            // This is the definitive part.
            _externalDispatcher = Dispatcher.CurrentDispatcher;
            WPFThread secondThread = new WPFThread( name );
            _innerDispatcher = secondThread.Dispatcher;
        }

        /// <summary>
        /// Gets the dispatcher of the NoFocusManager Thread.
        /// </summary>
        public Dispatcher NoFocusDispatcher
        {
            get { return _innerDispatcher; }
        }

        /// <summary>
        /// Gets the dispatcher of the external world.
        /// </summary>
        public Dispatcher ExternalDispatcher
        {
            get { return _externalDispatcher; }
        }

        /// <summary>
        /// Shuts down the underlying Thread. 
        /// Should ONLY be called when CiviKey is shutting down.
        /// </summary>
        public void Shutdown()
        {
            _innerDispatcher.BeginInvokeShutdown( DispatcherPriority.ApplicationIdle );
        }

        /// <summary>
        /// Create a CKWindow in the CKWindow Thread.
        /// Invokes the function set as parameter in the CKWindow Thread
        /// </summary>
        /// <typeparam name="T">The type of the window, must inherit from <see cref="CKWindow"/></typeparam>
        /// <param name="createFunction">The function that launches the "new" of the window</param>
        /// <returns>An object of the type set as parameter, created in the CKWindow Thread</returns>
        public T CreateNoFocusWindow<T>( Func<NoFocusManager,T> createFunction )
            where T : CKNoFocusWindow
        {
            T window = default( T );
            _innerDispatcher.Invoke( (Action)(() =>
            {
                window = createFunction( this );
            } )
            , null );

            return window;
        }

        #region IDisposable Members

        public void Dispose()
        {
            Shutdown();
        }

        #endregion
    }
}
