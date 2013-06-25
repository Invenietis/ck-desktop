#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Core\CKWindow.cs) is part of CiviKey. 
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


namespace CK.Windows
{

    /// <summary>
    /// This window inherits from <see cref="CKWindow"/>
    /// It is not activable if If ShowActivated == false and that it is created on thread seperated from the other windows of the application.
    /// </summary>
    public partial class CKNoFocusWindow : CKWindow
    {
        OSDriver _driver;
        bool _ncbuttondown;
        IntPtr _lastFocused;

        /// <summary>
        /// Default constructor of a <see cref="CKNoFocusWindow"/>
        /// </summary>
        public CKNoFocusWindow()
            :base()
        {
        }

        protected override void OnSourceInitialized( EventArgs e )
        {
            base.OnSourceInitialized( e );

            HwndSource hSource = HwndSource.FromHwnd( Hwnd );
            _driver = OSDriver.Create( this, hSource );

            if( !ShowActivated )
            {
                SetNoActivateFlag( true );
            }
        }

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

        Point PointFromLParam( IntPtr lParam )
        {
            return new Point( lParam.ToInt32() & 0xFFFF, lParam.ToInt32() >> 16 );
        }

        void CKNCHitTest( Point p, ref int htCode )
        {
            var point = PointFromScreen( p );
            HitTestResult result = VisualTreeHelper.HitTest( this, point );
            if( result != null )
            {
                //Console.WriteLine( "HitTest: "+ result.VisualHit.GetType().Name ); 
                if( IsDraggableVisual( result.VisualHit ) )
                {
                    htCode = Win.HTCAPTION;
                }
                else if( ResizeMode == System.Windows.ResizeMode.CanResizeWithGrip )
                {
                    if( TreeHelper.FindParentInVisualTree( result.VisualHit, d => d is System.Windows.Controls.Primitives.ResizeGrip ) != null )
                    {
                        if( FlowDirection == FlowDirection.RightToLeft )
                            htCode = Win.HTBOTTOMLEFT;
                        else htCode = Win.HTBOTTOMRIGHT;
                    }
                }
            }
            else
            {
                // Nothing was hit. Assume the extended frame.
                htCode = Win.HTCAPTION;
            }
        }

        /// <summary>
        /// By default, nothing is draggable: this method always returns false.
        /// By overriding this method, any visual elements can be considered as a handle to drag the window.
        /// </summary>
        /// <param name="visualElement">Visual element that could be considered as a handle to drag the window.</param>
        /// <returns>True if the element must drag the window.</returns>
        protected virtual bool IsDraggableVisual( DependencyObject visualElement )
        {
            return false;
        }
        
    }
}