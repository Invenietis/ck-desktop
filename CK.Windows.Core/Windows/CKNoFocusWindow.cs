#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Core\Windows\CKNoFocusWindow.cs) is part of CiviKey. 
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

#if DEBUG
#define WINTRACE
#endif

using System;
using System.Windows;
using System.Windows.Interop;
using CK.Core;
using System.Windows.Media;
using CK.Interop;
using System.Windows.Input;
using System.Runtime.InteropServices;
using CK.Windows.Interop;
using System.Windows.Threading;
using System.Collections.Generic;
using System.Diagnostics;
using CK.Windows.Helpers;
using System.Threading;


namespace CK.Windows
{

    /// <summary>
    /// This window inherits from <see cref="CKWindow"/>
    /// It is not activable if If ShowActivated == false and that it is created on thread seperated from the other windows of the application.
    /// </summary>
    public partial class CKNoFocusWindow : CKWindow
    {
        bool _ncbuttondown;
        IntPtr _lastFocused;

        /// <summary>
        /// Default constructor of a <see cref="CKNoFocusWindow"/>
        /// </summary>
        public CKNoFocusWindow( NoFocusManager noFocusManager )
            : base()
        {
            if( Thread.CurrentThread != noFocusManager.NoFocusDispatcher.Thread )
            {
                throw new InvalidOperationException( "The CKwindow must be instanciate in the noFocusManagerDispatcher" );
            }
        }

        IntPtr WndProcWinNoFocusDefault( IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled )
        {
            switch( msg )
            {
                case CK.Windows.Interop.Win.WM_NCLBUTTONDOWN:
                    {
                        _ncbuttondown = true;
                        GetFocus();
                        return hWnd;
                    }
                case CK.Windows.Interop.Win.WM_NCMOUSEMOVE:
                    {
                        if( _ncbuttondown )
                        {
                            ReleaseFocus();
                            _ncbuttondown = false;
                        }
                        return hWnd;
                    }
            }
            return IntPtr.Zero;
        }

        #region OnXXX

        protected override void OnSourceInitialized( EventArgs e )
        {
            base.OnSourceInitialized( e );

            HwndSource hSource = HwndSource.FromHwnd( Hwnd );
            hSource.AddHook( WndProcWinNoFocusDefault );

            if( !ShowActivated )
            {
                SetNoActivateFlag( true );
            }
        }

        protected override void OnStateChanged( EventArgs e )
        {
            base.OnStateChanged( e );
            if( WindowState == System.Windows.WindowState.Maximized )
            {
                WindowState = System.Windows.WindowState.Normal;
                ReleaseFocus();
            }
        }

        #endregion

        #region Methods

        internal void SetNoActivateFlag( bool set )
        {
            if( set )
            {
                Win.Functions.SetWindowLong(
                            Hwnd,
                            Win.WindowLongIndex.GWL_EXSTYLE,
                            (uint)Win.Functions.GetWindowLong( Hwnd, Win.WindowLongIndex.GWL_EXSTYLE ) | Win.WS_EX_NOACTIVATE );
            }
            else
            {
                Win.Functions.SetWindowLong(
                            Hwnd,
                            Win.WindowLongIndex.GWL_EXSTYLE,
                            (uint)Win.Functions.GetWindowLong( Hwnd, Win.WindowLongIndex.GWL_EXSTYLE ) & ~Win.WS_EX_NOACTIVATE );
            }
        }


        void GetFocus()
        {
            _lastFocused = CK.Windows.Interop.Win.Functions.GetForegroundWindow();
            CK.Windows.Interop.Win.Functions.SetForegroundWindow( Hwnd );
        }

        void ReleaseFocus()
        {
            CK.Windows.Interop.Win.Functions.SetForegroundWindow( _lastFocused );
        }

        #endregion
    }
}