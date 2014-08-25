#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Core\Helpers\WindowHelper.cs) is part of CiviKey. 
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
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows;
using System.Windows.Interop;

namespace CK.Windows.Helpers
{
    /// <summary>
    /// Holds a static method that enables knowing if a window is overlayed by another one.
    /// </summary>
    public class WindowHelper
    {
        private delegate bool CallBackPtr( int hwnd, int lParam );
        private static CallBackPtr callBackPtr;

        /// <summary>
        /// The enumerated pointers of actually visible windows
        /// </summary>
        public static List<IntPtr> enumedwindowPtrs = new List<IntPtr>();
        /// <summary>
        /// The enumerated rectangles of actually visible windows
        /// </summary>
        public static List<Rectangle> enumedwindowRects = new List<Rectangle>();

        /// <summary>
        /// Does a hit test for specified window (checks whether it is currently visible to user)
        /// </summary>
        /// <param name="window">The window to hit test</param>
        /// <returns>Returns a boolean saying whether all points of the window are visible to the user or not</returns>
        public static bool IsOverLayed( Window window )
        {
            IntPtr handle = new WindowInteropHelper( window ).Handle;
            Rectangle rec = new Rectangle( (int)window.Left, (int)window.Top, (int)window.ActualWidth, (int)window.ActualHeight );
            return IsOverLayed( rec, handle );
        }

        /// <summary>
        /// Does a hit test for specified control (is point of control visible to user)
        /// </summary>
        /// <param name="ctrlRect">the rectangle (usually Bounds) of the control</param>
        /// <param name="ctrlHandle">the handle for the control</param>
        /// <returns>boolean value indicating if ctrlRect is overlayed by another control </returns>
        public static bool IsOverLayed( Rectangle ctrlRect, IntPtr ctrlHandle )
        {
            // clear results
            enumedwindowPtrs.Clear();
            enumedwindowRects.Clear();

            // Create callback and start enumeration
            callBackPtr = new CallBackPtr( EnumCallBack );
            EnumDesktopWindows( IntPtr.Zero, callBackPtr, 0 );

            // Go from last to first window, and substract them from the ctrlRect area
            Region r = new Region( ctrlRect );

            bool StartClipping = false;
            for( int i = enumedwindowRects.Count - 1; i >= 0; i-- )
            {
                if( StartClipping )
                {
                    r.Exclude( enumedwindowRects[i] );
                }

                if( enumedwindowPtrs[i] == ctrlHandle ) StartClipping = true;
            }

            //Creating a list of points scattered on the edges of the window.
            IList<System.Drawing.Point> pointList = new List<System.Drawing.Point>();
            pointList.Add( new System.Drawing.Point( ctrlRect.X, ctrlRect.Y ) );
            pointList.Add( new System.Drawing.Point( ctrlRect.X + ctrlRect.Width - 2, ctrlRect.Y ) );
            pointList.Add( new System.Drawing.Point( ctrlRect.X + ctrlRect.Width - 2, ctrlRect.Y + ctrlRect.Height - 2 ) );
            pointList.Add( new System.Drawing.Point( ctrlRect.X, ctrlRect.Y + ctrlRect.Height - 2 ) );

            //TODO : choose the scale considering the width and height of the window.
            int scale = 50;
            int xOffset = 0;
            int yOffset = 0;
            int offset = 2;
            for( int x = 0; x <= scale; x++ )
            {
                for( int y = 0; y <= scale; y++ )
                {
                    if( y == scale ) yOffset = offset;
                    else yOffset = 0;

                    if( x == scale ) xOffset = offset;
                    else xOffset = 0;

                    pointList.Add( new System.Drawing.Point( ctrlRect.X + ctrlRect.Width / scale * x - xOffset, ctrlRect.Y + ctrlRect.Height / scale * y - yOffset ) );
                }
            }

            //Checking if theses points are visible by the user
            foreach( var item in pointList )
            {
                if( !r.IsVisible( item ) ) return true;
            }

            return false;
        }

        /// <summary>
        /// Window enumeration callback
        /// </summary>
        private static bool EnumCallBack( int hwnd, int lParam )
        {
            // If window is visible and not minimized (isiconic)
            if( IsWindow( (IntPtr)hwnd ) && IsWindowVisible( (IntPtr)hwnd ) && !IsIconic( (IntPtr)hwnd ) )
            {
                // add the handle and windowrect to "found windows" collection
                enumedwindowPtrs.Add( (IntPtr)hwnd );

                RECT rct;

                if( GetWindowRect( (IntPtr)hwnd, out rct ) )
                {
                    // add rect to list
                    enumedwindowRects.Add( new Rectangle( rct.Left, rct.Top, rct.Right - rct.Left, rct.Bottom - rct.Top ) );
                }
                else
                {
                    // invalid, make empty rectangle
                    enumedwindowRects.Add( new Rectangle( 0, 0, 0, 0 ) );
                }
            }

            return true;
        }


        [DllImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        static extern bool IsWindowVisible( IntPtr hWnd );

        [DllImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        static extern bool IsWindow( IntPtr hWnd );

        [DllImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        static extern bool IsIconic( IntPtr hWnd );

        [DllImport( "user32.dll" )]
        private static extern int EnumDesktopWindows( IntPtr hDesktop, CallBackPtr callPtr, int lPar );

        [DllImport( "user32.dll" )]
        [return: MarshalAs( UnmanagedType.Bool )]
        static extern bool GetWindowRect( IntPtr hWnd, out RECT lpRect );

        [StructLayout( LayoutKind.Sequential )]
        private struct RECT
        {
            public int Left;        // x position of upper-left corner
            public int Top;         // y position of upper-left corner
            public int Right;       // x position of lower-right corner
            public int Bottom;      // y position of lower-right corner

            public override string ToString()
            {
                return Left + "," + Top + "," + Right + "," + Bottom;
            }
        }
    }
}
