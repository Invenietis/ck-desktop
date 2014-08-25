#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Core\NoFocusManager.cs) is part of CiviKey. 
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
