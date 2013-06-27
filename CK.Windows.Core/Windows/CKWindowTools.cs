using CK.Windows.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Interop;
using System.Windows.Media;

namespace CK.Windows
{
    /// <summary>
    /// Static class that exposes the TryExtendFrame method. Used to add the Aero "glossy-effect" to a Window.
    /// </summary>
    public static class CKWindowTools
    {
        private const int SW_SHOWNORMAL = 1;
        private const int SW_SHOWMINIMIZED = 2;

        #region WinTrace helpers (commented)
        //[Conditional("WINTRACE")]
        //static void WinTrace( string text )
        //{
        //    Console.WriteLine( "[CiviKeyWindow]{0}.", text );
        //}

        //[Conditional( "WINTRACE" )]
        //static void WinTrace( string format, params object[] p )
        //{
        //    Console.WriteLine( "[CiviKeyWindow]{0}.", String.Format( format, p ) );
        //}

        //[Conditional( "WINTRACE" )]
        //static void WinTrace( IntPtr hWnd, string text )
        //{
        //    Console.WriteLine( "[CiviKeyWindow:0x{0:X}]{1}.", hWnd, text );
        //}

        //[Conditional( "WINTRACE" )]
        //static void WinTrace( CKWindow wnd, string text )
        //{
        //    Console.WriteLine( "[CiviKeyWindow:0x{0:X}]{1}.", wnd.ThisWindowHandle, text );
        //}

        //[Conditional( "WINTRACE" )]
        //static void WinTrace( IntPtr hWnd, string format, params object[] p )
        //{
        //    Console.WriteLine( "[CiviKeyWindow:0x{0:X}]{1}.", hWnd, String.Format( format, p ) );
        //}
        #endregion

        /// <summary>
        /// Applies the WINDOWPLACEMENT configuration set as parameter to the window set as parameter.
        /// Calls the SetWindowPlacement method in user32.dll
        /// </summary>
        /// <param name="w">The window to which the configuration should be applied</param>
        /// <param name="placement">The configuration, in a WINDOWPLACEMENT Struct (see MSDN for more information)</param>
        public static void SetPlacement( IntPtr hwnd, WINDOWPLACEMENT placement )
        {
            placement.length = Marshal.SizeOf( typeof( WINDOWPLACEMENT ) );
            placement.flags = 0;
            placement.showCmd = ( placement.showCmd == SW_SHOWMINIMIZED ? SW_SHOWNORMAL : placement.showCmd );
            SetWindowPlacement( hwnd, ref placement );
        }

        /// <summary>
        /// Gets the WINDOWPLACEMENT configuration of the window set as parameter.
        /// Calls the GetWindowPlacement method in user32.dll
        /// </summary>
        /// <param name="w">The window</param>
        /// <returns>The configuration of the window, in a WINDOWPLACEMENT struct</returns>
        public static WINDOWPLACEMENT GetPlacement( IntPtr hwnd )
        {
            WINDOWPLACEMENT placement = new WINDOWPLACEMENT();
            GetWindowPlacement( hwnd, out placement );
            return placement;
        }

        [System.Runtime.InteropServices.DllImport( "user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall )]
        private static extern bool SetWindowPlacement( IntPtr hWnd, [In] ref WINDOWPLACEMENT lpwndpl );

        [System.Runtime.InteropServices.DllImport( "user32.dll", CharSet = CharSet.Auto, CallingConvention = CallingConvention.StdCall )]
        private static extern bool GetWindowPlacement( IntPtr hWnd, out WINDOWPLACEMENT lpwndpl );
    }

    /// <summary>
    /// RECT structure required by WINDOWPLACEMENT structure 
    /// </summary>
    [Serializable]
    [StructLayout( LayoutKind.Sequential )]
    public struct RECT
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="left"></param>
        /// <param name="top"></param>
        /// <param name="right"></param>
        /// <param name="bottom"></param>
        public RECT( int left, int top, int right, int bottom )
        {
            this.Left = left;
            this.Top = top;
            this.Right = right;
            this.Bottom = bottom;
        }
    }

    /// <summary>
    /// POINT structure required by WINDOWPLACEMENT structure 
    /// </summary>
    [Serializable]
    [StructLayout( LayoutKind.Sequential )]
    public struct POINT
    {
        public int X;
        public int Y;

        /// <summary>
        /// Default constructor
        /// </summary>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        public POINT( int x, int y )
        {
            this.X = x;
            this.Y = y;
        }
    }

    /// <summary>
    /// WINDOWPLACEMENT stores the position, size, and state of a window
    /// See MSDN for this struct documentation : http://msdn.microsoft.com/en-us/library/aa253040(v=vs.60).aspx
    /// </summary>
    [Serializable]
    [StructLayout( LayoutKind.Sequential )]
    public struct WINDOWPLACEMENT
    {
        public int length;
        public int flags;
        public int showCmd;
        public POINT minPosition;
        public POINT maxPosition;
        public RECT normalPosition;
    }
}
