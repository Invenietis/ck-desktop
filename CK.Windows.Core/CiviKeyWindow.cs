#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Core\NoFocusWindow.cs) is part of CiviKey. 
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

namespace CK.Windows
{
    public partial class CiviKeyWindow : Window
    {
        /// <summary>
        /// "Restore hope" uses quite low priority dispatching and this is a good thing:
        /// it does not interfere at all with the application/windows life cycle.
        /// </summary>
        const DispatcherPriority RestoreHopePriority = DispatcherPriority.ApplicationIdle;

        IntPtr _hwnd;
        bool _isExtendedFrame;

        protected override void OnSourceInitialized( EventArgs e )
        {
            _hwnd = new WindowInteropHelper( this ).Handle;
            WinTrace( _hwnd, "Source initialized." );
            HwndSource hSource = HwndSource.FromHwnd( _hwnd );

            if( !ShowActivated )
            {
                Win.Functions.SetWindowLong(
                            _hwnd,
                            Win.WindowLongIndex.GWL_EXSTYLE,
                            (uint)Win.Functions.GetWindowLong( _hwnd, Win.WindowLongIndex.GWL_EXSTYLE ) |
                            (uint)Win.WS_EX.NOACTIVATE );
            }
            hSource.AddHook( _hopeRestorer.GetWndProc( this ) );
            hSource.CompositionTarget.BackgroundColor = Color.FromArgb( 0, 0, 0, 0 );
            hSource.CompositionTarget.RenderMode = RenderMode.Default;
            TryExtendFrame();
            base.OnSourceInitialized( e );
        }

        void TryExtendFrame()
        {
            try
            {
                WinTrace( _hwnd, "Frame extended." );
                _isExtendedFrame = OSVersionInfo.IsWindowsVistaOrGreater && Dwm.Functions.IsCompositionEnabled();
                if( _isExtendedFrame )
                {
                    // Negative margins have special meaning to DwmExtendFrameIntoClientArea.
                    // Negative margins create the "sheet of glass" effect, where the client 
                    // area is rendered as a solid surface without a window border.
                    Win.Margins m = new CK.Windows.Interop.Win.Margins() { LeftWidth = -1, RightWidth = -1, TopHeight = -1, BottomHeight = -1 };
                    Dwm.Functions.ExtendFrameIntoClientArea( _hwnd, ref m );
                }
            }
            catch
            {
                _isExtendedFrame = false;
                Background = new SolidColorBrush( Colors.WhiteSmoke );
            }
        }

        /// <summary>
        /// Called for activable Windows (when <see cref="Window.ShowActivated"/> is true).
        /// This can be used to manually restore the focus to previously active window.
        /// </summary>
        /// <param name="hWnd">The handle window of the window that just lost the focus.</param>
        /// <param name="isExternalWindow">True if the window is in another process. False if it is an internal (activable) window.</param>
        protected virtual void SetActiveTarget( IntPtr hWnd, bool isExternalWindow )
        {
        }

        /// <summary>
        /// Gets the Win32 window handle of this <see cref="Window"/> object.
        /// Available once <see cref="Window.SourceInitialized"/> has been raised.
        /// </summary>
        protected IntPtr ThisWindowHandle 
        { 
            get { return _hwnd; } 
        }

        Point PointFromLParam( IntPtr lParam )
        {
            return new Point( lParam.ToInt32() & 0xFFFF, lParam.ToInt32() >> 16 );
        }

        bool IsOnExtendedFrame( Point p )
        {
            var point = PointFromScreen( p );
            Visual client = Content as Visual;
            if( client != null )
            {
                HitTestResult result = VisualTreeHelper.HitTest( client, point );
                if( result != null ) return IsDraggableVisual( result.VisualHit );
            }
            // Nothing was hit - assume that this area is covered by frame extensions anyway
            return true;
        }

        /// <summary>
        /// By default, the <see cref="ContentControl.Content"/> is draggable. Any other element are not.
        /// By overriding this method, other visual elements can be considered as a handle to drag the window.
        /// </summary>
        /// <param name="visualElement">Visual element that could be considered as a handle to drag the window.</param>
        /// <returns>True if the element must drag the window.</returns>
        protected virtual bool IsDraggableVisual( DependencyObject visualElement )
        {
            return visualElement == Content;
        }

        #region WinTrace helpers
        [Conditional("WINTRACE")]
        static void WinTrace( string text )
        {
            Console.WriteLine( "[CiviKeyWindow]{0}.", text );
        }

        [Conditional( "WINTRACE" )]
        static void WinTrace( string format, params object[] p )
        {
            Console.WriteLine( "[CiviKeyWindow]{0}.", String.Format( format, p ) );
        }
        static void WinTrace( IntPtr hWnd, string text )
        {
            Console.WriteLine( "[CiviKeyWindow:0x{0:X}]{1}.", hWnd, text );
        }

        [Conditional( "WINTRACE" )]
        static void WinTrace( IntPtr hWnd, string format, params object[] p )
        {
            Console.WriteLine( "[CiviKeyWindow:0x{0:X}]{1}.", hWnd, String.Format( format, p ) );
        }
        #endregion
    }

}