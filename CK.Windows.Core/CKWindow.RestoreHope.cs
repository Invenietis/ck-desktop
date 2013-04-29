#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Core\CKWindow.RestoreHope.cs) is part of CiviKey. 
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
* Copyright © 2007-2013, 
*     Invenietis <http://www.invenietis.com>,
*     In’Tech INFO <http://www.intechinfo.fr>,
* All rights reserved. 
*-----------------------------------------------------------------------------*/
#endregion

#if DEBUG
#define WINTRACE
#endif

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Interop;
using System.Windows.Threading;
using CK.Windows.Interop;

namespace CK.Windows
{
    public partial class CKWindow
    {
        [ThreadStatic]
        static RestoreHope _hopeRestorer = CreateHope();

        static RestoreHope CreateHope()
        {
            RestoreHope h = new RestoreHopeWin8();
            WinTrace( "{0} creation for thread ManageThreaddId = {1}.", h.GetType().Name, System.Threading.Thread.CurrentThread.ManagedThreadId );
            return h;
        }

        abstract class RestoreHope
        {
            readonly DispatcherOperationCallback _restoreHope;
            IntPtr _currentMsgTarget;
            DispatcherOperation _currentHope;

            protected RestoreHope()
            {
                _restoreHope = new DispatcherOperationCallback( RestoreHopeAction );
            }

            /// <summary>
            /// Ensures that <see cref="DoRestoreHope"/> will be called.
            /// </summary>
            public void TriggerRestoreHope()
            {
                if( _currentHope == null )
                {
                    _currentHope = Dispatcher.CurrentDispatcher.BeginInvoke( RestoreHopePriority, _restoreHope, null );
                }
                else WinTrace( "Reentrant RestoreHope." );
            }

            object RestoreHopeAction( object p )
            {
                // Handle reentrancy by clearing the current operation.
                _currentHope = null;
                DoRestoreHope();
                return null;
            }

            /// <summary>
            /// Gets the handle that currently receives a message from this (<see cref="SendHopeMessage"/> is being called).
            /// Null otherwise.
            /// </summary>
            public IntPtr CurrentHWndMessageTarget
            {
                get { return _currentMsgTarget; }
            }

            /// <summary>
            /// Relays to <see cref="Win.Functions.SendMessage"/> after having set <see cref="CurrentHWndMessageTarget"/>
            /// and clearing it once done.
            /// </summary>
            /// <param name="hWnd">See <see cref="Win.Functions.SendMessage"/>.</param>
            /// <param name="msg">See <see cref="Win.Functions.SendMessage"/>.</param>
            /// <param name="wParam">See <see cref="Win.Functions.SendMessage"/>.</param>
            /// <param name="lParam">See <see cref="Win.Functions.SendMessage"/>.</param>
            protected void SendHopeMessage( IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam )
            {
                _currentMsgTarget = hWnd;
                Win.Functions.SendMessage( hWnd, msg, wParam, lParam );
                _currentMsgTarget = IntPtr.Zero;

            }

            /// <summary>
            /// Selects the window hook.
            /// This is called during source initialization: ther selected hook 
            /// is associated to the window.
            /// </summary>
            /// <param name="w">A CiviKey window.</param>
            /// <returns>The hook to use.</returns>
            public abstract HwndSourceHook GetWndProc( CKWindow w );

            /// <summary>
            /// Must process the full set of recorded actions at once and clear them.
            /// </summary>
            abstract protected void DoRestoreHope();

        }


    }
}
