using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Interop;
using System.Windows.Threading;
using CK.Windows.Interop;

namespace CK.Windows
{
    public partial class CiviKeyWindow
    {
        [ThreadStatic]
        static RestoreHope _hopeRestorer = CreateHope();

        static RestoreHope CreateHope()
        {
            return new RestoreHopeWin8();
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
            public abstract HwndSourceHook GetWndProc( CiviKeyWindow w );

            /// <summary>
            /// Must process the full set of recorded actions at once and clear them.
            /// </summary>
            abstract protected void DoRestoreHope();

        }


    }
}
