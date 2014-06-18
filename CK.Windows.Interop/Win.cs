#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Interop\Win.cs) is part of CiviKey. 
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
using System.Runtime.InteropServices;
using System.Security;
using CK.Core;
using CK.Interop;

namespace CK.Windows.Interop
{
    public static class Win
    {
        /// <summary>
        /// Gets Win32 native functions related to Window management.
        /// </summary>
        public static readonly User32Api Functions = PInvoker.GetInvoker<User32Api>();

        /// <summary>
        /// True constant is 1 in Windows.
        /// </summary>
        public static IntPtr PtrTrue = new IntPtr( 1 );
        
        /// <summary>
        /// True constant is 0 in Window.
        /// </summary>
        public static readonly IntPtr PtrFalse = IntPtr.Zero;

        /// <summary>
        /// Get and SetWindowLong index. 
        /// </summary>
        public enum WindowLongIndex 
        {
            GWL_WNDPROC       = -4,
            GWL_HINSTANCE     = -6,
            GWL_HWNDPARENT    = -8,
            GWL_STYLE         = -16,
            GWL_EXSTYLE       = -20,
            GWL_USERDATA      = -21,
            GWL_ID            = -12,
        }

        
        /// <summary>
        /// WM_MOUSEACTIVATE return value: activates the window, and does not discard the mouse message.
        /// </summary>
        public const int MA_ACTIVATE =  1;
        /// <summary>
        /// WM_MOUSEACTIVATE return value: activates the window, and discards the mouse message.
        /// </summary>
        public const int MA_ACTIVATEANDEAT = 2;
        /// <summary>
        /// WM_MOUSEACTIVATE return value: does not activate the window, and does not discard the mouse message.
        /// </summary>
        public const int MA_NOACTIVATE = 3;
        /// <summary>
        /// WM_MOUSEACTIVATE return value: does not activate the window, but discards the mouse message.
        /// </summary>
        public const int MA_NOACTIVATEANDEAT = 4;


        /// <summary>
        /// WM_ACTIVATE wParam low order value: activated by some method other than a mouse click (for example, by a call to 
        /// the SetActiveWindow function or by use of the keyboard interface to select the window).
        /// </summary>
        public const int WA_ACTIVE = 1;
        /// <summary>
        /// WM_ACTIVATE wParam low order value: activated by a mouse click.
        /// </summary>
        public const int WA_CLICKACTIVE = 2;
        /// <summary>
        /// WM_ACTIVATE wParam low order value: window ha been deactivated.
        /// </summary>
        public const int WA_INACTIVE = 0;

        #region Return code for WM_NCHITTEST
        /// <summary>
        /// Return code for WM_NCHITTEST: Client Area.
        /// </summary>
        public const int HTCLIENT = 1;
        /// <summary>
        /// Return code for WM_NCHITTEST: Caption of the window.
        /// </summary>
        public const int HTCAPTION = 2;
        /// <summary>
        /// Return code for WM_NCHITTEST: In the lower-left corner of a border of a resizable window (the user can click the mouse to resize the window diagonally).
        /// </summary>
        public const int HTBOTTOMLEFT = 16;
        /// <summary>
        /// Return code for WM_NCHITTEST: In the lower-right corner of a border of a resizable window (the user can click the mouse to resize the window diagonally).
        /// </summary>
        public const int HTBOTTOMRIGHT = 17;
        /// <summary>
        /// Return code for WM_NCHITTEST: In the lower-horizontal border of a resizable window (the user can click the mouse to resize the window vertically).
        /// </summary>
        public const int HTBOTTOM = 15;
         /// <summary>
        /// Return code for WM_NCHITTEST: In the border of a window that does not have a sizing border.
        /// </summary>
        public const int HTBORDER = 18;
        /// <summary>
        /// Return code for WM_NCHITTEST: In a Close button.
        /// </summary>
        public const int HTCLOSE = 20;
        /// <summary>
        /// Return code for WM_NCHITTEST: On the screen background or on a dividing line between windows (same as HTNOWHERE, except that the DefWindowProc function produces a system beep to indicate an error).
        /// </summary>
        public const int HTERROR = -2;
        /// <summary>
        /// Return code for WM_NCHITTEST: In a Maximize button.
        /// </summary>
        public const int HTMAXBUTTON = 9;
        /// <summary>
        /// Return code for WM_NCHITTEST: In a Minimize button.
        /// </summary>
        public const int HTMINBUTTON = 8; 
        /// <summary>
        /// Return code for WM_NCHITTEST: On the screen background or on a dividing line between windows.
        /// </summary>
        public const int HTNOWHERE = 0;
        /// <summary>
        /// Return code for WM_NCHITTEST: In the right border of a resizable window (the user can click the mouse to resize the window horizontally).
        /// </summary>
        public const int HTRIGHT = 11;
        /// <summary>
        /// Return code for WM_NCHITTEST: In a size box (same as HTGROWBOX).
        /// </summary>
        public const int HTSIZE = 4;
        /// <summary>
        /// Return code for WM_NCHITTEST: In a window menu or in a Close button in a child window.
        /// </summary>
        public const int HTSYSMENU = 3;
        /// <summary>
        /// Return code for WM_NCHITTEST: In the upper-horizontal border of a window.
        /// </summary>
        public const int HTTOP = 12;
        /// <summary>
        /// Return code for WM_NCHITTEST: In the upper-left corner of a window border.
        /// </summary>
        public const int HTTOPLEFT = 13;
        /// <summary>
        /// Return code for WM_NCHITTEST: In the upper-right corner of a window border.
        /// </summary>
        public const int HTTOPRIGHT = 14;
        /// <summary>
        /// Return code for WM_NCHITTEST: In a window currently covered by another window in the same thread (the message will be sent to 
        /// underlying windows in the same thread until one of them returns a code that is not HTTRANSPARENT).
        /// </summary>
        public const int HTTRANSPARENT = -1; 
        #endregion

        /// <summary>
        /// Window Styles.
        /// The following styles can be specified wherever a window style is required. After the control has been created, these styles cannot be modified, except as noted.
        /// </summary>
        [Flags]
        public enum WS : uint
        {
            /// <summary>The window has a thin-line border.</summary>
            BORDER = 0x800000,

            /// <summary>The window has a title bar (includes the BORDER style).</summary>
            CAPTION = 0xc00000,

            /// <summary>The window is a child window. A window with this style cannot have a menu bar. This style cannot be used with the POPUP style.</summary>
            CHILD = 0x40000000,

            /// <summary>
            /// Excludes the area occupied by child windows when drawing occurs within the parent window. This style is used when creating the parent window.
            /// </summary>
            CLIPCHILDREN = 0x2000000,

            /// <summary>
            /// Clips child windows relative to each other; that is, when a particular child window receives a WM_PAINT message, the CLIPSIBLINGS style clips all other overlapping child windows out of the region of the child window to be updated.
            /// If CLIPSIBLINGS is not specified and child windows overlap, it is possible, when drawing within the client area of a child window, to draw within the client area of a neighboring child window.
            /// </summary>
            CLIPSIBLINGS = 0x4000000,

            /// <summary>
            /// The window is initially disabled. A disabled window cannot receive input from the user. To change this after a window has been created, use the EnableWindow function.
            /// </summary>
            DISABLED = 0x8000000,

            /// <summary>
            /// The window has a border of a style typically used with dialog boxes. A window with this style cannot have a title bar.
            /// </summary>
            DLGFRAME = 0x400000,

            /// <summary>
            /// The window is the first control of a group of controls. The group consists of this first control and all controls defined after it, up to the next control with the GROUP style.
            /// The first control in each group usually has the TABSTOP style so that the user can move from group to group. The user can subsequently change the keyboard focus from one control in the group to the next control in the group by using the direction keys.
            /// You can turn this style on and off to change dialog box navigation. To change this style after a window has been created, use the SetWindowLong function.
            /// </summary>
            GROUP = 0x20000,

            /// <summary>The window has a horizontal scroll bar.</summary>
            HSCROLL = 0x100000,

            /// <summary>The window is initially maximized.</summary>
            MAXIMIZE = 0x1000000,

            /// <summary>The window has a maximize button. Cannot be combined with the EX_CONTEXTHELP style. The SYSMENU style must also be specified.</summary>
            MAXIMIZEBOX = 0x10000,

            /// <summary>The window is initially minimized.</summary>
            MINIMIZE = 0x20000000,

            /// <summary>The window has a minimize button. Cannot be combined with the EX_CONTEXTHELP style. The SYSMENU style must also be specified.</summary>
            MINIMIZEBOX = 0x20000,

            /// <summary>The window is an overlapped window. An overlapped window has a title bar and a border.</summary>
            OVERLAPPED = 0x0,

            /// <summary>The window is an overlapped window.</summary>
            OVERLAPPEDWINDOW = OVERLAPPED | CAPTION | SYSMENU | SIZEFRAME | MINIMIZEBOX | MAXIMIZEBOX,

            /// <summary>The window is a pop-up window. This style cannot be used with the CHILD style.</summary>
            POPUP = 0x80000000u,

            /// <summary>The window is a pop-up window. The CAPTION and POPUPWINDOW styles must be combined to make the window menu visible.</summary>
            POPUPWINDOW = POPUP | BORDER | SYSMENU,

            /// <summary>The window has a sizing border.</summary>
            SIZEFRAME = 0x40000,

            /// <summary>The window has a window menu on its title bar. The CAPTION style must also be specified.</summary>
            SYSMENU = 0x80000,

            /// <summary>
            /// The window is a control that can receive the keyboard focus when the user presses the TAB key.
            /// Pressing the TAB key changes the keyboard focus to the next control with the TABSTOP style.  
            /// You can turn this style on and off to change dialog box navigation. To change this style after a window has been created, use the SetWindowLong function.
            /// For user-created windows and modeless dialogs to work with tab stops, alter the message loop to call the IsDialogMessage function.
            /// </summary>
            TABSTOP = 0x10000,

            /// <summary>The window is initially visible. This style can be turned on and off by using the ShowWindow or SetWindowPos function.</summary>
            VISIBLE = 0x10000000,

            /// <summary>The window has a vertical scroll bar.</summary>
            VSCROLL = 0x200000
        }

        [Flags]
        [Obsolete("use the const uint WS_EX_ instead.", true)]
        public enum WS_EX : uint
        {
            /// <summary>
            /// Specifies that a window created with this style accepts drag-drop files.
            /// </summary>
            ACCEPTFILES = 0x00000010,
            /// <summary>
            /// Forces a top-level window onto the taskbar when the window is visible.
            /// </summary>
            APPWINDOW = 0x00040000,
            /// <summary>
            /// Specifies that a window has a border with a sunken edge.
            /// </summary>
            CLIENTEDGE = 0x00000200,
            /// <summary>
            /// Windows XP: Paints all descendants of a window in bottom-to-top painting order using double-buffering. For more information, see Remarks. This cannot be used if the window has a class style of either CS_OWNDC or CS_CLASSDC.
            /// </summary>
            COMPOSITED = 0x02000000,
            /// <summary>
            /// Includes a question mark in the title bar of the window. When the user clicks the question mark, the cursor changes to a question mark with a pointer. If the user then clicks a child window, the child receives a WM_HELP message. The child window should pass the message to the parent window procedure, which should call the WinHelp function using the HELP_WM_HELP command. The Help application displays a pop-up window that typically contains help for the child window.
            /// CONTEXTHELP cannot be used with the WS_MAXIMIZEBOX or WS_MINIMIZEBOX styles.
            /// </summary>
            CONTEXTHELP = 0x00000400,
            /// <summary>
            /// The window itself contains child windows that should take part in dialog box navigation. If this style is specified, the dialog manager recurses into children of this window when performing navigation operations such as handling the TAB key, an arrow key, or a keyboard mnemonic.
            /// </summary>
            CONTROLPARENT = 0x00010000,
            /// <summary>
            /// Creates a window that has a double border; the window can, optionally, be created with a title bar by specifying the WS_CAPTION style in the dwStyle parameter.
            /// </summary>
            DLGMODALFRAME = 0x00000001,
            /// <summary>
            /// Windows 2000/XP: Creates a layered window. Note that this cannot be used for child windows. Also, this cannot be used if the window has a class style of either CS_OWNDC or CS_CLASSDC.
            /// </summary>
            LAYERED = 0x00080000,
            /// <summary>
            /// Arabic and Hebrew versions of Windows 98/Me, Windows 2000/XP: Creates a window whose horizontal origin is on the right edge. Increasing horizontal values advance to the left.
            /// </summary>
            LAYOUTRTL = 0x00400000,
            /// <summary>
            /// Creates a window that has generic left-aligned properties. This is the default.
            /// </summary>
            LEFT = 0x00000000,
            /// <summary>
            /// If the shell language is Hebrew, Arabic, or another language that supports reading order alignment, the vertical scroll bar (if present) is to the left of the client area. For other languages, the style is ignored.
            /// </summary>
            LEFTSCROLLBAR = 0x00004000,
            /// <summary>
            /// The window text is displayed using left-to-right reading-order properties. This is the default.
            /// </summary>
            LTRREADING = 0x00000000,
            /// <summary>
            /// Creates a multiple-document interface (MDI) child window.
            /// </summary>
            MDICHILD = 0x00000040,
            /// <summary>
            /// Windows 2000/XP: A top-level window created with this style does not become the foreground window when the user clicks it. The system does not bring this window to the foreground when the user minimizes or closes the foreground window.
            /// To activate the window, use the SetActiveWindow or SetForegroundWindow function.
            /// The window does not appear on the taskbar by default. To force the window to appear on the taskbar, use the APPWINDOW style.
            /// </summary>
            NOACTIVATE = 0x08000000,
            /// <summary>
            /// Windows 2000/XP: A window created with this style does not pass its window layout to its child windows.
            /// </summary>
            NOINHERITLAYOUT = 0x00100000,
            /// <summary>
            /// Specifies that a child window created with this style does not send the WM_PARENTNOTIFY message to its parent window when it is created or destroyed.
            /// </summary>
            NOPARENTNOTIFY = 0x00000004,
            /// <summary>
            /// Combines the CLIENTEDGE and WINDOWEDGE styles.
            /// </summary>
            OVERLAPPEDWINDOW = WINDOWEDGE | CLIENTEDGE,
            /// <summary>
            /// Combines the WINDOWEDGE, TOOLWINDOW, and TOPMOST styles.
            /// </summary>
            PALETTEWINDOW = WINDOWEDGE | TOOLWINDOW | TOPMOST,
            /// <summary>
            /// The window has generic "right-aligned" properties. This depends on the window class. This style has an effect only if the shell language is Hebrew, Arabic, or another language that supports reading-order alignment; otherwise, the style is ignored.
            /// Using the RIGHT style for static or edit controls has the same effect as using the SS_RIGHT or ES_RIGHT style, respectively. Using this style with button controls has the same effect as using BS_RIGHT and BS_RIGHTBUTTON styles.
            /// </summary>
            RIGHT = 0x00001000,
            /// <summary>
            /// Vertical scroll bar (if present) is to the right of the client area. This is the default.
            /// </summary>
            RIGHTSCROLLBAR = 0x00000000,
            /// <summary>
            /// If the shell language is Hebrew, Arabic, or another language that supports reading-order alignment, the window text is displayed using right-to-left reading-order properties. For other languages, the style is ignored.
            /// </summary>
            RTLREADING = 0x00002000,
            /// <summary>
            /// Creates a window with a three-dimensional border style intended to be used for items that do not accept user input.
            /// </summary>
            STATICEDGE = 0x00020000,
            /// <summary>
            /// Creates a tool window; that is, a window intended to be used as a floating toolbar. A tool window has a title bar that is shorter than a normal title bar, and the window title is drawn using a smaller font. A tool window does not appear in the taskbar or in the dialog that appears when the user presses ALT+TAB. If a tool window has a system menu, its icon is not displayed on the title bar. However, you can display the system menu by right-clicking or by typing ALT+SPACE.
            /// </summary>
            TOOLWINDOW = 0x00000080,
            /// <summary>
            /// Specifies that a window created with this style should be placed above all non-topmost windows and should stay above them, even when the window is deactivated. To add or remove this style, use the SetWindowPos function.
            /// </summary>
            TOPMOST = 0x00000008,
            /// <summary>
            /// Specifies that a window created with this style should not be painted until siblings beneath the window (that were created by the same thread) have been painted. The window appears transparent because the bits of underlying sibling windows have already been painted.
            /// To achieve transparency without these restrictions, use the SetWindowRgn function.
            /// </summary>
            TRANSPARENT = 0x00000020,
            /// <summary>
            /// Specifies that a window has a border with a raised edge.
            /// </summary>
            WINDOWEDGE = 0x00000100
        }

        #region WS_EX_ flags
                /// <summary>
        /// Specifies that a window created with this style accepts drag-drop files.
        /// </summary>
        public const uint WS_EX_ACCEPTFILES = 0x00000010;
        /// <summary>
        /// Forces a top-level window onto the taskbar when the window is visible.
        /// </summary>
        public const uint WS_EX_APPWINDOW = 0x00040000;
        /// <summary>
        /// Specifies that a window has a border with a sunken edge.
        /// </summary>
        public const uint WS_EX_CLIENTEDGE = 0x00000200;
        /// <summary>
        /// Windows XP: Paints all descendants of a window in bottom-to-top painting order using double-buffering. For more information, see Remarks. This cannot be used if the window has a class style of either CS_OWNDC or CS_CLASSDC.
        /// </summary>
        public const uint WS_EX_COMPOSITED = 0x02000000;
        /// <summary>
        /// Includes a question mark in the title bar of the window. When the user clicks the question mark, the cursor changes to a question mark with a pointer. If the user then clicks a child window, the child receives a WM_HELP message. The child window should pass the message to the parent window procedure, which should call the WinHelp function using the HELP_WM_HELP command. The Help application displays a pop-up window that typically contains help for the child window.
        /// CONTEXTHELP cannot be used with the WS_MAXIMIZEBOX or WS_MINIMIZEBOX styles.
        /// </summary>
        public const uint WS_EX_CONTEXTHELP = 0x00000400;
        /// <summary>
        /// The window itself contains child windows that should take part in dialog box navigation. If this style is specified, the dialog manager recurses into children of this window when performing navigation operations such as handling the TAB key, an arrow key, or a keyboard mnemonic.
        /// </summary>
        public const uint WS_EX_CONTROLPARENT = 0x00010000;
        /// <summary>
        /// Creates a window that has a double border; the window can, optionally, be created with a title bar by specifying the WS_CAPTION style in the dwStyle parameter.
        /// </summary>
        public const uint WS_EX_DLGMODALFRAME = 0x00000001;
        /// <summary>
        /// Windows 2000/XP: Creates a layered window. Note that this cannot be used for child windows. Also, this cannot be used if the window has a class style of either CS_OWNDC or CS_CLASSDC.
        /// </summary>
        public const uint WS_EX_LAYERED = 0x00080000;
        /// <summary>
        /// Arabic and Hebrew versions of Windows 98/Me, Windows 2000/XP: Creates a window whose horizontal origin is on the right edge. Increasing horizontal values advance to the left.
        /// </summary>
        public const uint WS_EX_LAYOUTRTL = 0x00400000;
        /// <summary>
        /// Creates a window that has generic left-aligned properties. This is the default.
        /// </summary>
        public const uint WS_EX_LEFT = 0x00000000;
        /// <summary>
        /// If the shell language is Hebrew, Arabic, or another language that supports reading order alignment, the vertical scroll bar (if present) is to the left of the client area. For other languages, the style is ignored.
        /// </summary>
        public const uint WS_EX_LEFTSCROLLBAR = 0x00004000;
        /// <summary>
        /// The window text is displayed using left-to-right reading-order properties. This is the default.
        /// </summary>
        public const uint WS_EX_LTRREADING = 0x00000000;
        /// <summary>
        /// Creates a multiple-document interface (MDI) child window.
        /// </summary>
        public const uint WS_EX_MDICHILD = 0x00000040;
        /// <summary>
        /// Windows 2000/XP: A top-level window created with this style does not become the foreground window when the user clicks it. The system does not bring this window to the foreground when the user minimizes or closes the foreground window.
        /// To activate the window, use the SetActiveWindow or SetForegroundWindow function.
        /// The window does not appear on the taskbar by default. To force the window to appear on the taskbar, use the APPWINDOW style.
        /// </summary>
        public const uint WS_EX_NOACTIVATE = 0x08000000;
        /// <summary>
        /// Windows 2000/XP: A window created with this style does not pass its window layout to its child windows.
        /// </summary>
        public const uint WS_EX_NOINHERITLAYOUT = 0x00100000;
        /// <summary>
        /// Specifies that a child window created with this style does not send the WM_PARENTNOTIFY message to its parent window when it is created or destroyed.
        /// </summary>
        public const uint WS_EX_NOPARENTNOTIFY = 0x00000004;
        /// <summary>
        /// Combines the CLIENTEDGE and WINDOWEDGE styles.
        /// </summary>
        public const uint WS_EX_OVERLAPPEDWINDOW = WS_EX_WINDOWEDGE | WS_EX_CLIENTEDGE;
        /// <summary>
        /// Combines the WINDOWEDGE, TOOLWINDOW, and TOPMOST styles.
        /// </summary>
        public const uint WS_EX_PALETTEWINDOW = WS_EX_WINDOWEDGE | WS_EX_TOOLWINDOW | WS_EX_TOPMOST;
        /// <summary>
        /// The window has generic "right-aligned" properties. This depends on the window class. This style has an effect only if the shell language is Hebrew, Arabic, or another language that supports reading-order alignment; otherwise, the style is ignored.
        /// Using the RIGHT style for static or edit controls has the same effect as using the SS_RIGHT or ES_RIGHT style, respectively. Using this style with button controls has the same effect as using BS_RIGHT and BS_RIGHTBUTTON styles.
        /// </summary>
        public const uint WS_EX_RIGHT = 0x00001000;
        /// <summary>
        /// Vertical scroll bar (if present) is to the right of the client area. This is the default.
        /// </summary>
        public const uint WS_EX_RIGHTSCROLLBAR = 0x00000000;
        /// <summary>
        /// If the shell language is Hebrew, Arabic, or another language that supports reading-order alignment, the window text is displayed using right-to-left reading-order properties. For other languages, the style is ignored.
        /// </summary>
        public const uint WS_EX_RTLREADING = 0x00002000;
        /// <summary>
        /// Creates a window with a three-dimensional border style intended to be used for items that do not accept user input.
        /// </summary>
        public const uint WS_EX_STATICEDGE = 0x00020000;
        /// <summary>
        /// Creates a tool window; that is, a window intended to be used as a floating toolbar. A tool window has a title bar that is shorter than a normal title bar, and the window title is drawn using a smaller font. A tool window does not appear in the taskbar or in the dialog that appears when the user presses ALT+TAB. If a tool window has a system menu, its icon is not displayed on the title bar. However, you can display the system menu by right-clicking or by typing ALT+SPACE.
        /// </summary>
        public const uint WS_EX_TOOLWINDOW = 0x00000080;
        /// <summary>
        /// Specifies that a window created with this style should be placed above all non-topmost windows and should stay above them, even when the window is deactivated. To add or remove this style, use the SetWindowPos function.
        /// </summary>
        public const uint WS_EX_TOPMOST = 0x00000008;
        /// <summary>
        /// Specifies that a window created with this style should not be painted until siblings beneath the window (that were created by the same thread) have been painted. The window appears transparent because the bits of underlying sibling windows have already been painted.
        /// To achieve transparency without these restrictions, use the SetWindowRgn function.
        /// </summary>
        public const uint WS_EX_TRANSPARENT = 0x00000020;
        /// <summary>
        /// Specifies that a window has a border with a raised edge.
        /// </summary>
        public const uint WS_EX_WINDOWEDGE = 0x00000100;
        #endregion

        #region WM_ constants: Windows Messages defined in winuser.h from Windows SDK v6.1. Documentation from MSDN.
        /// <summary>
        /// The WM_ACTIVATE message is sent to both the window being activated and the window being deactivated. If the windows use the same input queue, the message is sent synchronously, first to the window procedure of the top-level window being deactivated, then to the window procedure of the top-level window being activated. If the windows use different input queues, the message is sent asynchronously, so the window is activated immediately.
        /// The wParam contains a pointer to the last focused window
        /// </summary>
        public const int WM_ACTIVATE = 0x0006;

        /// <summary>
        /// An application sends the WM_SETREDRAW message to a window to allow changes in that window to be redrawn or to prevent changes in that window from being redrawn.
        /// </summary>
        public const int WM_SETREDRAW = 0x000B;

        /// <summary>
        /// The WM_ACTIVATEAPP message is sent when a window belonging to a different application than the active window is about to be activated. 
        /// The message is sent to the application whose window is being activated and to the application whose window is being deactivated.
        /// </summary>
        public const int WM_ACTIVATEAPP = 0x001C;

        /// <summary>
        /// The WM_NCHITTEST message is sent to a window when the cursor moves, or when a mouse button is pressed or released. 
        /// If the mouse is not captured, the message is sent to the window beneath the cursor. Otherwise, the message is sent to the window that has captured the mouse.
        /// </summary>
        public const int WM_NCHITTEST = 0x0084;

        /// <summary>
        /// The WM_NCACTIVATE message is sent to a window when its nonclient area needs to be changed to indicate an active or inactive state.
        /// </summary>
        public const int WM_NCACTIVATE = 0x0086;

        /// <summary>
        /// The system will send a window the WM_DWMCOMPOSITIONCHANGED message to indicate that the availability of desktop composition has changed.
        /// </summary>
        public const int WM_DWMCOMPOSITIONCHANGED = 0x031E;

            /// <summary>
        /// The WM_NULL message performs no operation. An application sends the WM_NULL message if it wants to post a message that the recipient window will ignore.
        /// </summary>
        public const int WM_NULL = 0x0000;
        /// <summary>
        /// The WM_CREATE message is sent when an application requests that a window be created by calling the CreateWindowEx or CreateWindow function. (The message is sent before the function returns.) The window procedure of the new window receives this message after the window is created, but before the window becomes visible.
        /// </summary>
        public const int WM_CREATE = 0x0001;
        /// <summary>
        /// The WM_DESTROY message is sent when a window is being destroyed. It is sent to the window procedure of the window being destroyed after the window is removed from the screen.
        /// This message is sent first to the window being destroyed and then to the child windows (if any) as they are destroyed. During the processing of the message, it can be assumed that all child windows still exist.
        /// /// </summary>
        public const int WM_DESTROY = 0x0002;
        /// <summary>
        /// The WM_MOVE message is sent after a window has been moved.
        /// </summary>
        public const int WM_MOVE = 0x0003;
        /// <summary>
        /// The WM_SIZE message is sent to a window after its size has changed.
        /// </summary>
        public const int WM_SIZE = 0x0005;
        /// <summary>
        /// The WM_SETFOCUS message is sent to a window after it has gained the keyboard focus.
        /// </summary>
        public const int WM_SETFOCUS = 0x0007;
        /// <summary>
        /// The WM_KILLFOCUS message is sent to a window immediately before it loses the keyboard focus.
        /// </summary>
        public const int WM_KILLFOCUS = 0x0008;
        /// <summary>
        /// The WM_ENABLE message is sent when an application changes the enabled state of a window. It is sent to the window whose enabled state is changing. This message is sent before the EnableWindow function returns, but after the enabled state (WS_DISABLED style bit) of the window has changed.
        /// </summary>
        public const int WM_ENABLE = 0x000A;
        /// <summary>
        /// An application sends a WM_SETTEXT message to set the text of a window.
        /// </summary>
        public const int WM_SETTEXT = 0x000C;
        /// <summary>
        /// An application sends a WM_GETTEXT message to copy the text that corresponds to a window into a buffer provided by the caller.
        /// </summary>
        public const int WM_GETTEXT = 0x000D;
        /// <summary>
        /// An application sends a WM_GETTEXTLENGTH message to determine the length, in characters, of the text associated with a window.
        /// </summary>
        public const int WM_GETTEXTLENGTH = 0x000E;
        /// <summary>
        /// The WM_PAINT message is sent when the system or another application makes a request to paint a portion of an application's window. The message is sent when the UpdateWindow or RedrawWindow function is called, or by the DispatchMessage function when the application obtains a WM_PAINT message by using the GetMessage or PeekMessage function.
        /// </summary>
        public const int WM_PAINT = 0x000F;
        /// <summary>
        /// The WM_CLOSE message is sent as a signal that a window or an application should terminate.
        /// </summary>
        public const int WM_CLOSE = 0x0010;
        /// <summary>
        /// The WM_QUERYENDSESSION message is sent when the user chooses to end the session or when an application calls one of the system shutdown functions. If any application returns zero, the session is not ended. The system stops sending WM_QUERYENDSESSION messages as soon as one application returns zero.
        /// After processing this message, the system sends the WM_ENDSESSION message with the wParam parameter set to the results of the WM_QUERYENDSESSION message.
        /// </summary>
        public const int WM_QUERYENDSESSION = 0x0011;
        /// <summary>
        /// The WM_QUERYOPEN message is sent to an icon when the user requests that the window be restored to its previous size and position.
        /// </summary>
        public const int WM_QUERYOPEN = 0x0013;
        /// <summary>
        /// The WM_ENDSESSION message is sent to an application after the system processes the results of the WM_QUERYENDSESSION message. The WM_ENDSESSION message informs the application whether the session is ending.
        /// </summary>
        public const int WM_ENDSESSION = 0x0016;
        /// <summary>
        /// The WM_QUIT message indicates a request to terminate an application and is generated when the application calls the PostQuitMessage function. It causes the GetMessage function to return zero.
        /// </summary>
        public const int WM_QUIT = 0x0012;
        /// <summary>
        /// The WM_ERASEBKGND message is sent when the window background must be erased (for example, when a window is resized). The message is sent to prepare an invalidated portion of a window for painting.
        /// </summary>
        public const int WM_ERASEBKGND = 0x0014;
        /// <summary>
        /// This message is sent to all top-level windows when a change is made to a system color setting.
        /// </summary>
        public const int WM_SYSCOLORCHANGE = 0x0015;
        /// <summary>
        /// The WM_SHOWWINDOW message is sent to a window when the window is about to be hidden or shown.
        /// </summary>
        public const int WM_SHOWWINDOW = 0x0018;
        /// <summary>
        /// An application sends the WM_WININICHANGE message to all top-level windows after making a change to the WIN.INI file. The SystemParametersInfo function sends this message after an application uses the function to change a setting in WIN.INI.
        /// Note  The WM_WININICHANGE message is provided only for compatibility with earlier versions of the system. Applications should use the WM_SETTINGCHANGE message.
        /// </summary>
        public const int WM_WININICHANGE = 0x001A;
        /// <summary>
        /// An application sends the WM_WININICHANGE message to all top-level windows after making a change to the WIN.INI file. The SystemParametersInfo function sends this message after an application uses the function to change a setting in WIN.INI.
        /// Note  The WM_WININICHANGE message is provided only for compatibility with earlier versions of the system. Applications should use the WM_SETTINGCHANGE message.
        /// </summary>
        public const int WM_SETTINGCHANGE = WM_WININICHANGE;
        /// <summary>
        /// The WM_DEVMODECHANGE message is sent to all top-level windows whenever the user changes device-mode settings.
        /// </summary>
        public const int WM_DEVMODECHANGE = 0x001B;
        /// <summary>
        /// An application sends the WM_FONTCHANGE message to all top-level windows in the system after changing the pool of font resources.
        /// </summary>
        public const int WM_FONTCHANGE = 0x001D;
        /// <summary>
        /// A message that is sent whenever there is a change in the system time.
        /// </summary>
        public const int WM_TIMECHANGE = 0x001E;
        /// <summary>
        /// The WM_CANCELMODE message is sent to cancel certain modes, such as mouse capture. For example, the system sends this message to the active window when a dialog box or message box is displayed. Certain functions also send this message explicitly to the specified window regardless of whether it is the active window. For example, the EnableWindow function sends this message when disabling the specified window.
        /// </summary>
        public const int WM_CANCELMODE = 0x001F;
        /// <summary>
        /// The WM_SETCURSOR message is sent to a window if the mouse causes the cursor to move within a window and mouse input is not captured.
        /// </summary>
        public const int WM_SETCURSOR = 0x0020;
        /// <summary>
        /// The WM_MOUSEACTIVATE message is sent when the cursor is in an inactive window and the user presses a mouse button. The parent window receives this message only if the child window passes it to the DefWindowProc function.
        /// </summary>
        public const int WM_MOUSEACTIVATE = 0x0021;
        /// <summary>
        /// The WM_CHILDACTIVATE message is sent to a child window when the user clicks the window's title bar or when the window is activated, moved, or sized.
        /// </summary>
        public const int WM_CHILDACTIVATE = 0x0022;
        /// <summary>
        /// The WM_QUEUESYNC message is sent by a computer-based training (CBT) application to separate user-input messages from other messages sent through the WH_JOURNALPLAYBACK Hook procedure.
        /// </summary>
        public const int WM_QUEUESYNC = 0x0023;
        /// <summary>
        /// The WM_GETMINMAXINFO message is sent to a window when the size or position of the window is about to change. An application can use this message to override the window's default maximized size and position, or its default minimum or maximum tracking size.
        /// </summary>
        public const int WM_GETMINMAXINFO = 0x0024;
        /// <summary>
        /// Windows NT 3.51 and earlier: The WM_PAINTICON message is sent to a minimized window when the icon is to be painted. This message is not sent by newer versions of Microsoft Windows, except in unusual circumstances explained in the Remarks.
        /// </summary>
        public const int WM_PAINTICON = 0x0026;
        /// <summary>
        /// Windows NT 3.51 and earlier: The WM_ICONERASEBKGND message is sent to a minimized window when the background of the icon must be filled before painting the icon. A window receives this message only if a class icon is defined for the window; otherwise, WM_ERASEBKGND is sent. This message is not sent by newer versions of Windows.
        /// </summary>
        public const int WM_ICONERASEBKGND = 0x0027;
        /// <summary>
        /// The WM_NEXTDLGCTL message is sent to a dialog box procedure to set the keyboard focus to a different control in the dialog box.
        /// </summary>
        public const int WM_NEXTDLGCTL = 0x0028;
        /// <summary>
        /// The WM_SPOOLERSTATUS message is sent from Print Manager whenever a job is added to or removed from the Print Manager queue.
        /// </summary>
        public const int WM_SPOOLERSTATUS = 0x002A;
        /// <summary>
        /// The WM_DRAWITEM message is sent to the parent window of an owner-drawn button, combo box, list box, or menu when a visual aspect of the button, combo box, list box, or menu has changed.
        /// </summary>
        public const int WM_DRAWITEM = 0x002B;
        /// <summary>
        /// The WM_MEASUREITEM message is sent to the owner window of a combo box, list box, list view control, or menu item when the control or menu is created.
        /// </summary>
        public const int WM_MEASUREITEM = 0x002C;
        /// <summary>
        /// Sent to the owner of a list box or combo box when the list box or combo box is destroyed or when items are removed by the LB_DELETESTRING, LB_RESETCONTENT, CB_DELETESTRING, or CB_RESETCONTENT message. The system sends a WM_DELETEITEM message for each deleted item. The system sends the WM_DELETEITEM message for any deleted list box or combo box item with nonzero item data.
        /// </summary>
        public const int WM_DELETEITEM = 0x002D;
        /// <summary>
        /// Sent by a list box with the LBS_WANTKEYBOARDINPUT style to its owner in response to a WM_KEYDOWN message.
        /// </summary>
        public const int WM_VKEYTOITEM = 0x002E;
        /// <summary>
        /// Sent by a list box with the LBS_WANTKEYBOARDINPUT style to its owner in response to a WM_CHAR message.
        /// </summary>
        public const int WM_CHARTOITEM = 0x002F;
        /// <summary>
        /// An application sends a WM_SETFONT message to specify the font that a control is to use when drawing text.
        /// </summary>
        public const int WM_SETFONT = 0x0030;
        /// <summary>
        /// An application sends a WM_GETFONT message to a control to retrieve the font with which the control is currently drawing its text.
        /// </summary>
        public const int WM_GETFONT = 0x0031;
        /// <summary>
        /// An application sends a WM_SETHOTKEY message to a window to associate a hot key with the window. When the user presses the hot key, the system activates the window.
        /// </summary>
        public const int WM_SETHOTKEY = 0x0032;
        /// <summary>
        /// An application sends a WM_GETHOTKEY message to determine the hot key associated with a window.
        /// </summary>
        public const int WM_GETHOTKEY = 0x0033;
        /// <summary>
        /// The WM_QUERYDRAGICON message is sent to a minimized (iconic) window. The window is about to be dragged by the user but does not have an icon defined for its class. An application can return a handle to an icon or cursor. The system displays this cursor or icon while the user drags the icon.
        /// </summary>
        public const int WM_QUERYDRAGICON = 0x0037;
        /// <summary>
        /// The system sends the WM_COMPAREITEM message to determine the relative position of a new item in the sorted list of an owner-drawn combo box or list box. Whenever the application adds a new item, the system sends this message to the owner of a combo box or list box created with the CBS_SORT or LBS_SORT style.
        /// </summary>
        public const int WM_COMPAREITEM = 0x0039;
        /// <summary>
        /// Active Accessibility sends the WM_GETOBJECT message to obtain information about an accessible object contained in a server application.
        /// Applications never send this message directly. It is sent only by Active Accessibility in response to calls to AccessibleObjectFromPoint, AccessibleObjectFromEvent, or AccessibleObjectFromWindow. However, server applications handle this message.
        /// </summary>
        public const int WM_GETOBJECT = 0x003D;
        /// <summary>
        /// The WM_COMPACTING message is sent to all top-level windows when the system detects more than 12.5 percent of system time over a 30- to 60-second interval is being spent compacting memory. This indicates that system memory is low.
        /// </summary>
        public const int WM_COMPACTING = 0x0041;
        /// <summary>
        /// WM_COMMNOTIFY is Obsolete for Win32-Based Applications
        /// </summary>
        [Obsolete]
        public const int WM_COMMNOTIFY = 0x0044;
        /// <summary>
        /// The WM_WINDOWPOSCHANGING message is sent to a window whose size, position, or place in the Z order is about to change as a result of a call to the SetWindowPos function or another window-management function.
        /// </summary>
        public const int WM_WINDOWPOSCHANGING = 0x0046;
        /// <summary>
        /// The WM_WINDOWPOSCHANGED message is sent to a window whose size, position, or place in the Z order has changed as a result of a call to the SetWindowPos function or another window-management function.
        /// </summary>
        public const int WM_WINDOWPOSCHANGED = 0x0047;
        /// <summary>
        /// Notifies applications that the system, typically a battery-powered personal computer, is about to enter a suspended mode.
        /// Use: POWERBROADCAST
        /// </summary>
        [Obsolete]
        public const int WM_POWER = 0x0048;
        /// <summary>
        /// An application sends the WM_COPYDATA message to pass data to another application.
        /// </summary>
        public const int WM_COPYDATA = 0x004A;
        /// <summary>
        /// The WM_CANCELJOURNAL message is posted to an application when a user cancels the application's journaling activities. The message is posted with a NULL window handle.
        /// </summary>
        public const int WM_CANCELJOURNAL = 0x004B;
        /// <summary>
        /// Sent by a common control to its parent window when an event has occurred or the control requires some information.
        /// </summary>
        public const int WM_NOTIFY = 0x004E;
        /// <summary>
        /// The WM_INPUTLANGCHANGEREQUEST message is posted to the window with the focus when the user chooses a new input language, either with the hotkey (specified in the Keyboard control panel application) or from the indicator on the system taskbar. An application can accept the change by passing the message to the DefWindowProc function or reject the change (and prevent it from taking place) by returning immediately.
        /// </summary>
        public const int WM_INPUTLANGCHANGEREQUEST = 0x0050;
        /// <summary>
        /// The WM_INPUTLANGCHANGE message is sent to the topmost affected window after an application's input language has been changed. You should make any application-specific settings and pass the message to the DefWindowProc function, which passes the message to all first-level child windows. These child windows can pass the message to DefWindowProc to have it pass the message to their child windows, and so on.
        /// </summary>
        public const int WM_INPUTLANGCHANGE = 0x0051;
        /// <summary>
        /// Sent to an application that has initiated a training card with Microsoft Windows Help. The message informs the application when the user clicks an authorable button. An application initiates a training card by specifying the HELP_TCARD command in a call to the WinHelp function.
        /// </summary>
        public const int WM_TCARD = 0x0052;
        /// <summary>
        /// Indicates that the user pressed the F1 key. If a menu is active when F1 is pressed, WM_HELP is sent to the window associated with the menu; otherwise, WM_HELP is sent to the window that has the keyboard focus. If no window has the keyboard focus, WM_HELP is sent to the currently active window.
        /// </summary>
        public const int WM_HELP = 0x0053;
        /// <summary>
        /// The WM_USERCHANGED message is sent to all windows after the user has logged on or off. When the user logs on or off, the system updates the user-specific settings. The system sends this message immediately after updating the settings.
        /// </summary>
        public const int WM_USERCHANGED = 0x0054;
        /// <summary>
        /// Determines if a window accepts ANSI or Unicode structures in the WM_NOTIFY notification message. WM_NOTIFYFORMAT messages are sent from a common control to its parent window and from the parent window to the common control.
        /// </summary>
        public const int WM_NOTIFYFORMAT = 0x0055;
        /// <summary>
        /// The WM_CONTEXTMENU message notifies a window that the user clicked the right mouse button (right-clicked) in the window.
        /// </summary>
        public const int WM_CONTEXTMENU = 0x007B;
        /// <summary>
        /// The WM_STYLECHANGING message is sent to a window when the SetWindowLong function is about to change one or more of the window's styles.
        /// </summary>
        public const int WM_STYLECHANGING = 0x007C;
        /// <summary>
        /// The WM_STYLECHANGED message is sent to a window after the SetWindowLong function has changed one or more of the window's styles
        /// </summary>
        public const int WM_STYLECHANGED = 0x007D;
        /// <summary>
        /// The WM_DISPLAYCHANGE message is sent to all windows when the display resolution has changed.
        /// </summary>
        public const int WM_DISPLAYCHANGE = 0x007E;
        /// <summary>
        /// The WM_GETICON message is sent to a window to retrieve a handle to the large or small icon associated with a window. The system displays the large icon in the ALT+TAB dialog, and the small icon in the window caption.
        /// </summary>
        public const int WM_GETICON = 0x007F;
        /// <summary>
        /// An application sends the WM_SETICON message to associate a new large or small icon with a window. The system displays the large icon in the ALT+TAB dialog box, and the small icon in the window caption.
        /// </summary>
        public const int WM_SETICON = 0x0080;
        /// <summary>
        /// The WM_NCCREATE message is sent prior to the WM_CREATE message when a window is first created.
        /// </summary>
        public const int WM_NCCREATE = 0x0081;
        /// <summary>
        /// The WM_NCDESTROY message informs a window that its nonclient area is being destroyed. The DestroyWindow function sends the WM_NCDESTROY message to the window following the WM_DESTROY message. WM_DESTROY is used to free the allocated memory object associated with the window.
        /// The WM_NCDESTROY message is sent after the child windows have been destroyed. In contrast, WM_DESTROY is sent before the child windows are destroyed.
        /// </summary>
        public const int WM_NCDESTROY = 0x0082;
        /// <summary>
        /// The WM_NCCALCSIZE message is sent when the size and position of a window's client area must be calculated. By processing this message, an application can control the content of the window's client area when the size or position of the window changes.
        /// </summary>
        public const int WM_NCCALCSIZE = 0x0083;
        /// <summary>
        /// The WM_NCPAINT message is sent to a window when its frame must be painted.
        /// </summary>
        public const int WM_NCPAINT = 0x0085;
        /// <summary>
        /// The WM_GETDLGCODE message is sent to the window procedure associated with a control. By default, the system handles all keyboard input to the control; the system interprets certain types of keyboard input as dialog box navigation keys. To override this default behavior, the control can respond to the WM_GETDLGCODE message to indicate the types of input it wants to process itself.
        /// </summary>
        public const int WM_GETDLGCODE = 0x0087;
        /// <summary>
        /// The WM_SYNCPAINT message is used to synchronize painting while avoiding linking independent GUI threads.
        /// </summary>
        public const int WM_SYNCPAINT = 0x0088;
        /// <summary>
        /// The WM_NCMOUSEMOVE message is posted to a window when the cursor is moved within the nonclient area of the window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        public const int WM_NCMOUSEMOVE = 0x00A0;
        /// <summary>
        /// The WM_NCLBUTTONDOWN message is posted when the user presses the left mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        public const int WM_NCLBUTTONDOWN = 0x00A1;
        /// <summary>
        /// The WM_NCLBUTTONUP message is posted when the user releases the left mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        public const int WM_NCLBUTTONUP = 0x00A2;
        /// <summary>
        /// The WM_NCLBUTTONDBLCLK message is posted when the user double-clicks the left mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        public const int WM_NCLBUTTONDBLCLK = 0x00A3;
        /// <summary>
        /// The WM_NCRBUTTONDOWN message is posted when the user presses the right mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        public const int WM_NCRBUTTONDOWN = 0x00A4;
        /// <summary>
        /// The WM_NCRBUTTONUP message is posted when the user releases the right mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        public const int WM_NCRBUTTONUP = 0x00A5;
        /// <summary>
        /// The WM_NCRBUTTONDBLCLK message is posted when the user double-clicks the right mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        public const int WM_NCRBUTTONDBLCLK = 0x00A6;
        /// <summary>
        /// The WM_NCMBUTTONDOWN message is posted when the user presses the middle mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        public const int WM_NCMBUTTONDOWN = 0x00A7;
        /// <summary>
        /// The WM_NCMBUTTONUP message is posted when the user releases the middle mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        public const int WM_NCMBUTTONUP = 0x00A8;
        /// <summary>
        /// The WM_NCMBUTTONDBLCLK message is posted when the user double-clicks the middle mouse button while the cursor is within the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        public const int WM_NCMBUTTONDBLCLK = 0x00A9;
        /// <summary>
        /// The WM_NCXBUTTONDOWN message is posted when the user presses the first or second X button while the cursor is in the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        public const int WM_NCXBUTTONDOWN = 0x00AB;
        /// <summary>
        /// The WM_NCXBUTTONUP message is posted when the user releases the first or second X button while the cursor is in the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        public const int WM_NCXBUTTONUP = 0x00AC;
        /// <summary>
        /// The WM_NCXBUTTONDBLCLK message is posted when the user double-clicks the first or second X button while the cursor is in the nonclient area of a window. This message is posted to the window that contains the cursor. If a window has captured the mouse, this message is not posted.
        /// </summary>
        public const int WM_NCXBUTTONDBLCLK = 0x00AD;
        /// <summary>
        /// The WM_INPUT_DEVICE_CHANGE message is sent to the window that registered to receive raw input. A window receives this message through its WindowProc function.
        /// </summary>
        public const int WM_INPUT_DEVICE_CHANGE = 0x00FE;
        /// <summary>
        /// The WM_INPUT message is sent to the window that is getting raw input.
        /// </summary>
        public const int WM_INPUT = 0x00FF;
        /// <summary>
        /// This message filters for keyboard messages.
        /// </summary>
        public const int WM_KEYFIRST = 0x0100;
        /// <summary>
        /// The WM_KEYDOWN message is posted to the window with the keyboard focus when a nonsystem key is pressed. A nonsystem key is a key that is pressed when the ALT key is not pressed.
        /// </summary>
        public const int WM_KEYDOWN = 0x0100;
        /// <summary>
        /// The WM_KEYUP message is posted to the window with the keyboard focus when a nonsystem key is released. A nonsystem key is a key that is pressed when the ALT key is not pressed, or a keyboard key that is pressed when a window has the keyboard focus.
        /// </summary>
        public const int WM_KEYUP = 0x0101;
        /// <summary>
        /// The WM_CHAR message is posted to the window with the keyboard focus when a WM_KEYDOWN message is translated by the TranslateMessage function. The WM_CHAR message contains the character code of the key that was pressed.
        /// </summary>
        public const int WM_CHAR = 0x0102;
        /// <summary>
        /// The WM_DEADCHAR message is posted to the window with the keyboard focus when a WM_KEYUP message is translated by the TranslateMessage function. WM_DEADCHAR specifies a character code generated by a dead key. A dead key is a key that generates a character, such as the umlaut (double-dot), that is combined with another character to form a composite character. For example, the umlaut-O character (Ö) is generated by typing the dead key for the umlaut character, and then typing the O key.
        /// </summary>
        public const int WM_DEADCHAR = 0x0103;
        /// <summary>
        /// The WM_SYSKEYDOWN message is posted to the window with the keyboard focus when the user presses the F10 key (which activates the menu bar) or holds down the ALT key and then presses another key. It also occurs when no window currently has the keyboard focus; in this case, the WM_SYSKEYDOWN message is sent to the active window. The window that receives the message can distinguish between these two contexts by checking the context code in the lParam parameter.
        /// </summary>
        public const int WM_SYSKEYDOWN = 0x0104;
        /// <summary>
        /// The WM_SYSKEYUP message is posted to the window with the keyboard focus when the user releases a key that was pressed while the ALT key was held down. It also occurs when no window currently has the keyboard focus; in this case, the WM_SYSKEYUP message is sent to the active window. The window that receives the message can distinguish between these two contexts by checking the context code in the lParam parameter.
        /// </summary>
        public const int WM_SYSKEYUP = 0x0105;
        /// <summary>
        /// The WM_SYSCHAR message is posted to the window with the keyboard focus when a WM_SYSKEYDOWN message is translated by the TranslateMessage function. It specifies the character code of a system character key — that is, a character key that is pressed while the ALT key is down.
        /// </summary>
        public const int WM_SYSCHAR = 0x0106;
        /// <summary>
        /// The WM_SYSDEADCHAR message is sent to the window with the keyboard focus when a WM_SYSKEYDOWN message is translated by the TranslateMessage function. WM_SYSDEADCHAR specifies the character code of a system dead key — that is, a dead key that is pressed while holding down the ALT key.
        /// </summary>
        public const int WM_SYSDEADCHAR = 0x0107;
        /// <summary>
        /// The WM_UNICHAR message is posted to the window with the keyboard focus when a WM_KEYDOWN message is translated by the TranslateMessage function. The WM_UNICHAR message contains the character code of the key that was pressed.
        /// The WM_UNICHAR message is equivalent to WM_CHAR, but it uses Unicode Transformation Format (UTF)-32, whereas WM_CHAR uses UTF-16. It is designed to send or post Unicode characters to ANSI windows and it can can handle Unicode Supplementary Plane characters.
        /// </summary>
        public const int WM_UNICHAR = 0x0109;
        /// <summary>
        /// This message filters for keyboard messages.
        /// </summary>
        public const int WM_KEYLAST = 0x0109;
        /// <summary>
        /// Sent immediately before the IME generates the composition string as a result of a keystroke. A window receives this message through its WindowProc function.
        /// </summary>
        public const int WM_IME_STARTCOMPOSITION = 0x010D;
        /// <summary>
        /// Sent to an application when the IME ends composition. A window receives this message through its WindowProc function.
        /// </summary>
        public const int WM_IME_ENDCOMPOSITION = 0x010E;
        /// <summary>
        /// Sent to an application when the IME changes composition status as a result of a keystroke. A window receives this message through its WindowProc function.
        /// </summary>
        public const int WM_IME_COMPOSITION = 0x010F;
        public const int WM_IME_KEYLAST = 0x010F;
        /// <summary>
        /// The WM_INITDIALOG message is sent to the dialog box procedure immediately before a dialog box is displayed. Dialog box procedures typically use this message to initialize controls and carry out any other initialization tasks that affect the appearance of the dialog box.
        /// </summary>
        public const int WM_INITDIALOG = 0x0110;
        /// <summary>
        /// The WM_COMMAND message is sent when the user selects a command item from a menu, when a control sends a notification message to its parent window, or when an accelerator keystroke is translated.
        /// </summary>
        public const int WM_COMMAND = 0x0111;
        /// <summary>
        /// A window receives this message when the user chooses a command from the Window menu, clicks the maximize button, minimize button, restore button, close button, or moves the form. You can stop the form from moving by filtering this out.
        /// </summary>
        public const int WM_SYSCOMMAND = 0x0112;
        /// <summary>
        /// The WM_TIMER message is posted to the installing thread's message queue when a timer expires. The message is posted by the GetMessage or PeekMessage function.
        /// </summary>
        public const int WM_TIMER = 0x0113;
        /// <summary>
        /// The WM_HSCROLL message is sent to a window when a scroll event occurs in the window's standard horizontal scroll bar. This message is also sent to the owner of a horizontal scroll bar control when a scroll event occurs in the control.
        /// </summary>
        public const int WM_HSCROLL = 0x0114;
        /// <summary>
        /// The WM_VSCROLL message is sent to a window when a scroll event occurs in the window's standard vertical scroll bar. This message is also sent to the owner of a vertical scroll bar control when a scroll event occurs in the control.
        /// </summary>
        public const int WM_VSCROLL = 0x0115;
        /// <summary>
        /// The WM_INITMENU message is sent when a menu is about to become active. It occurs when the user clicks an item on the menu bar or presses a menu key. This allows the application to modify the menu before it is displayed.
        /// </summary>
        public const int WM_INITMENU = 0x0116;
        /// <summary>
        /// The WM_INITMENUPOPUP message is sent when a drop-down menu or submenu is about to become active. This allows an application to modify the menu before it is displayed, without changing the entire menu.
        /// </summary>
        public const int WM_INITMENUPOPUP = 0x0117;
        /// <summary>
        /// The WM_MENUSELECT message is sent to a menu's owner window when the user selects a menu item.
        /// </summary>
        public const int WM_MENUSELECT = 0x011F;
        /// <summary>
        /// The WM_MENUCHAR message is sent when a menu is active and the user presses a key that does not correspond to any mnemonic or accelerator key. This message is sent to the window that owns the menu.
        /// </summary>
        public const int WM_MENUCHAR = 0x0120;
        /// <summary>
        /// The WM_ENTERIDLE message is sent to the owner window of a modal dialog box or menu that is entering an idle state. A modal dialog box or menu enters an idle state when no messages are waiting in its queue after it has processed one or more previous messages.
        /// </summary>
        public const int WM_ENTERIDLE = 0x0121;
        /// <summary>
        /// The WM_MENURBUTTONUP message is sent when the user releases the right mouse button while the cursor is on a menu item.
        /// </summary>
        public const int WM_MENURBUTTONUP = 0x0122;
        /// <summary>
        /// The WM_MENUDRAG message is sent to the owner of a drag-and-drop menu when the user drags a menu item.
        /// </summary>
        public const int WM_MENUDRAG = 0x0123;
        /// <summary>
        /// The WM_MENUGETOBJECT message is sent to the owner of a drag-and-drop menu when the mouse cursor enters a menu item or moves from the center of the item to the top or bottom of the item.
        /// </summary>
        public const int WM_MENUGETOBJECT = 0x0124;
        /// <summary>
        /// The WM_UNINITMENUPOPUP message is sent when a drop-down menu or submenu has been destroyed.
        /// </summary>
        public const int WM_UNINITMENUPOPUP = 0x0125;
        /// <summary>
        /// The WM_MENUCOMMAND message is sent when the user makes a selection from a menu.
        /// </summary>
        public const int WM_MENUCOMMAND = 0x0126;
        /// <summary>
        /// An application sends the WM_CHANGEUISTATE message to indicate that the user interface (UI) state should be changed.
        /// </summary>
        public const int WM_CHANGEUISTATE = 0x0127;
        /// <summary>
        /// An application sends the WM_UPDATEUISTATE message to change the user interface (UI) state for the specified window and all its child windows.
        /// </summary>
        public const int WM_UPDATEUISTATE = 0x0128;
        /// <summary>
        /// An application sends the WM_QUERYUISTATE message to retrieve the user interface (UI) state for a window.
        /// </summary>
        public const int WM_QUERYUISTATE = 0x0129;
        /// <summary>
        /// The WM_CTLCOLORMSGBOX message is sent to the owner window of a message box before Windows draws the message box. By responding to this message, the owner window can set the text and background colors of the message box by using the given display device context handle.
        /// </summary>
        public const int WM_CTLCOLORMSGBOX = 0x0132;
        /// <summary>
        /// An edit control that is not read-only or disabled sends the WM_CTLCOLOREDIT message to its parent window when the control is about to be drawn. By responding to this message, the parent window can use the specified device context handle to set the text and background colors of the edit control.
        /// </summary>
        public const int WM_CTLCOLOREDIT = 0x0133;
        /// <summary>
        /// Sent to the parent window of a list box before the system draws the list box. By responding to this message, the parent window can set the text and background colors of the list box by using the specified display device context handle.
        /// </summary>
        public const int WM_CTLCOLORLISTBOX = 0x0134;
        /// <summary>
        /// The WM_CTLCOLORBTN message is sent to the parent window of a button before drawing the button. The parent window can change the button's text and background colors. However, only owner-drawn buttons respond to the parent window processing this message.
        /// </summary>
        public const int WM_CTLCOLORBTN = 0x0135;
        /// <summary>
        /// The WM_CTLCOLORDLG message is sent to a dialog box before the system draws the dialog box. By responding to this message, the dialog box can set its text and background colors using the specified display device context handle.
        /// </summary>
        public const int WM_CTLCOLORDLG = 0x0136;
        /// <summary>
        /// The WM_CTLCOLORSCROLLBAR message is sent to the parent window of a scroll bar control when the control is about to be drawn. By responding to this message, the parent window can use the display context handle to set the background color of the scroll bar control.
        /// </summary>
        public const int WM_CTLCOLORSCROLLBAR = 0x0137;
        /// <summary>
        /// A static control, or an edit control that is read-only or disabled, sends the WM_CTLCOLORSTATIC message to its parent window when the control is about to be drawn. By responding to this message, the parent window can use the specified device context handle to set the text and background colors of the static control.
        /// </summary>
        public const int WM_CTLCOLORSTATIC = 0x0138;
        /// <summary>
        /// Use WM_MOUSEFIRST to specify the first mouse message. Use the PeekMessage() Function.
        /// </summary>
        public const int WM_MOUSEFIRST = 0x0200;
        /// <summary>
        /// The WM_MOUSEMOVE message is posted to a window when the cursor moves. If the mouse is not captured, the message is posted to the window that contains the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        public const int WM_MOUSEMOVE = 0x0200;
        /// <summary>
        /// The WM_LBUTTONDOWN message is posted when the user presses the left mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        public const int WM_LBUTTONDOWN = 0x0201;
        /// <summary>
        /// The WM_LBUTTONUP message is posted when the user releases the left mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        public const int WM_LBUTTONUP = 0x0202;
        /// <summary>
        /// The WM_LBUTTONDBLCLK message is posted when the user double-clicks the left mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        public const int WM_LBUTTONDBLCLK = 0x0203;
        /// <summary>
        /// The WM_RBUTTONDOWN message is posted when the user presses the right mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        public const int WM_RBUTTONDOWN = 0x0204;
        /// <summary>
        /// The WM_RBUTTONUP message is posted when the user releases the right mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        public const int WM_RBUTTONUP = 0x0205;
        /// <summary>
        /// The WM_RBUTTONDBLCLK message is posted when the user double-clicks the right mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        public const int WM_RBUTTONDBLCLK = 0x0206;
        /// <summary>
        /// The WM_MBUTTONDOWN message is posted when the user presses the middle mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        public const int WM_MBUTTONDOWN = 0x0207;
        /// <summary>
        /// The WM_MBUTTONUP message is posted when the user releases the middle mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        public const int WM_MBUTTONUP = 0x0208;
        /// <summary>
        /// The WM_MBUTTONDBLCLK message is posted when the user double-clicks the middle mouse button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        public const int WM_MBUTTONDBLCLK = 0x0209;
        /// <summary>
        /// The WM_MOUSEWHEEL message is sent to the focus window when the mouse wheel is rotated. The DefWindowProc function propagates the message to the window's parent. There should be no internal forwarding of the message, since DefWindowProc propagates it up the parent chain until it finds a window that processes it.
        /// </summary>
        public const int WM_MOUSEWHEEL = 0x020A;
        /// <summary>
        /// The WM_XBUTTONDOWN message is posted when the user presses the first or second X button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        public const int WM_XBUTTONDOWN = 0x020B;
        /// <summary>
        /// The WM_XBUTTONUP message is posted when the user releases the first or second X button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        public const int WM_XBUTTONUP = 0x020C;
        /// <summary>
        /// The WM_XBUTTONDBLCLK message is posted when the user double-clicks the first or second X button while the cursor is in the client area of a window. If the mouse is not captured, the message is posted to the window beneath the cursor. Otherwise, the message is posted to the window that has captured the mouse.
        /// </summary>
        public const int WM_XBUTTONDBLCLK = 0x020D;
        /// <summary>
        /// The WM_MOUSEHWHEEL message is sent to the focus window when the mouse's horizontal scroll wheel is tilted or rotated. The DefWindowProc function propagates the message to the window's parent. There should be no internal forwarding of the message, since DefWindowProc propagates it up the parent chain until it finds a window that processes it.
        /// </summary>
        public const int WM_MOUSEHWHEEL = 0x020E;
        /// <summary>
        /// Use WM_MOUSELAST to specify the last mouse message. Used with PeekMessage() Function.
        /// </summary>
        public const int WM_MOUSELAST = 0x020E;
        /// <summary>
        /// The WM_PARENTNOTIFY message is sent to the parent of a child window when the child window is created or destroyed, or when the user clicks a mouse button while the cursor is over the child window. When the child window is being created, the system sends WM_PARENTNOTIFY just before the CreateWindow or CreateWindowEx function that creates the window returns. When the child window is being destroyed, the system sends the message before any processing to destroy the window takes place.
        /// </summary>
        public const int WM_PARENTNOTIFY = 0x0210;
        /// <summary>
        /// The WM_ENTERMENULOOP message informs an application's main window procedure that a menu modal loop has been entered.
        /// </summary>
        public const int WM_ENTERMENULOOP = 0x0211;
        /// <summary>
        /// The WM_EXITMENULOOP message informs an application's main window procedure that a menu modal loop has been exited.
        /// </summary>
        public const int WM_EXITMENULOOP = 0x0212;
        /// <summary>
        /// The WM_NEXTMENU message is sent to an application when the right or left arrow key is used to switch between the menu bar and the system menu.
        /// </summary>
        public const int WM_NEXTMENU = 0x0213;
        /// <summary>
        /// The WM_SIZING message is sent to a window that the user is resizing. By processing this message, an application can monitor the size and position of the drag rectangle and, if needed, change its size or position.
        /// </summary>
        public const int WM_SIZING = 0x0214;
        /// <summary>
        /// The WM_CAPTURECHANGED message is sent to the window that is losing the mouse capture.
        /// </summary>
        public const int WM_CAPTURECHANGED = 0x0215;
        /// <summary>
        /// The WM_MOVING message is sent to a window that the user is moving. By processing this message, an application can monitor the position of the drag rectangle and, if needed, change its position.
        /// </summary>
        public const int WM_MOVING = 0x0216;
        /// <summary>
        /// Notifies applications that a power-management event has occurred.
        /// </summary>
        public const int WM_POWERBROADCAST = 0x0218;
        /// <summary>
        /// Notifies an application of a change to the hardware configuration of a device or the computer.
        /// </summary>
        public const int WM_DEVICECHANGE = 0x0219;
        /// <summary>
        /// An application sends the WM_MDICREATE message to a multiple-document interface (MDI) client window to create an MDI child window.
        /// </summary>
        public const int WM_MDICREATE = 0x0220;
        /// <summary>
        /// An application sends the WM_MDIDESTROY message to a multiple-document interface (MDI) client window to close an MDI child window.
        /// </summary>
        public const int WM_MDIDESTROY = 0x0221;
        /// <summary>
        /// An application sends the WM_MDIACTIVATE message to a multiple-document interface (MDI) client window to instruct the client window to activate a different MDI child window.
        /// </summary>
        public const int WM_MDIACTIVATE = 0x0222;
        /// <summary>
        /// An application sends the WM_MDIRESTORE message to a multiple-document interface (MDI) client window to restore an MDI child window from maximized or minimized size.
        /// </summary>
        public const int WM_MDIRESTORE = 0x0223;
        /// <summary>
        /// An application sends the WM_MDINEXT message to a multiple-document interface (MDI) client window to activate the next or previous child window.
        /// </summary>
        public const int WM_MDINEXT = 0x0224;
        /// <summary>
        /// An application sends the WM_MDIMAXIMIZE message to a multiple-document interface (MDI) client window to maximize an MDI child window. The system resizes the child window to make its client area fill the client window. The system places the child window's window menu icon in the rightmost position of the frame window's menu bar, and places the child window's restore icon in the leftmost position. The system also appends the title bar text of the child window to that of the frame window.
        /// </summary>
        public const int WM_MDIMAXIMIZE = 0x0225;
        /// <summary>
        /// An application sends the WM_MDITILE message to a multiple-document interface (MDI) client window to arrange all of its MDI child windows in a tile format.
        /// </summary>
        public const int WM_MDITILE = 0x0226;
        /// <summary>
        /// An application sends the WM_MDICASCADE message to a multiple-document interface (MDI) client window to arrange all its child windows in a cascade format.
        /// </summary>
        public const int WM_MDICASCADE = 0x0227;
        /// <summary>
        /// An application sends the WM_MDIICONARRANGE message to a multiple-document interface (MDI) client window to arrange all minimized MDI child windows. It does not affect child windows that are not minimized.
        /// </summary>
        public const int WM_MDIICONARRANGE = 0x0228;
        /// <summary>
        /// An application sends the WM_MDIGETACTIVE message to a multiple-document interface (MDI) client window to retrieve the handle to the active MDI child window.
        /// </summary>
        public const int WM_MDIGETACTIVE = 0x0229;
        /// <summary>
        /// An application sends the WM_MDISETMENU message to a multiple-document interface (MDI) client window to replace the entire menu of an MDI frame window, to replace the window menu of the frame window, or both.
        /// </summary>
        public const int WM_MDISETMENU = 0x0230;
        /// <summary>
        /// The WM_ENTERSIZEMOVE message is sent one time to a window after it enters the moving or sizing modal loop. The window enters the moving or sizing modal loop when the user clicks the window's title bar or sizing border, or when the window passes the WM_SYSCOMMAND message to the DefWindowProc function and the wParam parameter of the message specifies the SC_MOVE or SC_SIZE value. The operation is complete when DefWindowProc returns.
        /// The system sends the WM_ENTERSIZEMOVE message regardless of whether the dragging of full windows is enabled.
        /// </summary>
        public const int WM_ENTERSIZEMOVE = 0x0231;
        /// <summary>
        /// The WM_EXITSIZEMOVE message is sent one time to a window, after it has exited the moving or sizing modal loop. The window enters the moving or sizing modal loop when the user clicks the window's title bar or sizing border, or when the window passes the WM_SYSCOMMAND message to the DefWindowProc function and the wParam parameter of the message specifies the SC_MOVE or SC_SIZE value. The operation is complete when DefWindowProc returns.
        /// </summary>
        public const int WM_EXITSIZEMOVE = 0x0232;
        /// <summary>
        /// Sent when the user drops a file on the window of an application that has registered itself as a recipient of dropped files.
        /// </summary>
        public const int WM_DROPFILES = 0x0233;
        /// <summary>
        /// An application sends the WM_MDIREFRESHMENU message to a multiple-document interface (MDI) client window to refresh the window menu of the MDI frame window.
        /// </summary>
        public const int WM_MDIREFRESHMENU = 0x0234;
        /// <summary>
        /// Sent to an application when a window is activated. A window receives this message through its WindowProc function.
        /// </summary>
        public const int WM_IME_SETCONTEXT = 0x0281;
        /// <summary>
        /// Sent to an application to notify it of changes to the IME window. A window receives this message through its WindowProc function.
        /// </summary>
        public const int WM_IME_NOTIFY = 0x0282;
        /// <summary>
        /// Sent by an application to direct the IME window to carry out the requested command. The application uses this message to control the IME window that it has created. To send this message, the application calls the SendMessage function with the following parameters.
        /// </summary>
        public const int WM_IME_CONTROL = 0x0283;
        /// <summary>
        /// Sent to an application when the IME window finds no space to extend the area for the composition window. A window receives this message through its WindowProc function.
        /// </summary>
        public const int WM_IME_COMPOSITIONFULL = 0x0284;
        /// <summary>
        /// Sent to an application when the operating system is about to change the current IME. A window receives this message through its WindowProc function.
        /// </summary>
        public const int WM_IME_SELECT = 0x0285;
        /// <summary>
        /// Sent to an application when the IME gets a character of the conversion result. A window receives this message through its WindowProc function.
        /// </summary>
        public const int WM_IME_CHAR = 0x0286;
        /// <summary>
        /// Sent to an application to provide commands and request information. A window receives this message through its WindowProc function.
        /// </summary>
        public const int WM_IME_REQUEST = 0x0288;
        /// <summary>
        /// Sent to an application by the IME to notify the application of a key press and to keep message order. A window receives this message through its WindowProc function.
        /// </summary>
        public const int WM_IME_KEYDOWN = 0x0290;
        /// <summary>
        /// Sent to an application by the IME to notify the application of a key release and to keep message order. A window receives this message through its WindowProc function.
        /// </summary>
        public const int WM_IME_KEYUP = 0x0291;
        /// <summary>
        /// The WM_MOUSEHOVER message is posted to a window when the cursor hovers over the client area of the window for the period of time specified in a prior call to TrackMouseEvent.
        /// </summary>
        public const int WM_MOUSEHOVER = 0x02A1;
        /// <summary>
        /// The WM_MOUSELEAVE message is posted to a window when the cursor leaves the client area of the window specified in a prior call to TrackMouseEvent.
        /// </summary>
        public const int WM_MOUSELEAVE = 0x02A3;
        /// <summary>
        /// The WM_NCMOUSEHOVER message is posted to a window when the cursor hovers over the nonclient area of the window for the period of time specified in a prior call to TrackMouseEvent.
        /// </summary>
        public const int WM_NCMOUSEHOVER = 0x02A0;
        /// <summary>
        /// The WM_NCMOUSELEAVE message is posted to a window when the cursor leaves the nonclient area of the window specified in a prior call to TrackMouseEvent.
        /// </summary>
        public const int WM_NCMOUSELEAVE = 0x02A2;
        /// <summary>
        /// The WM_WTSSESSION_CHANGE message notifies applications of changes in session state.
        /// </summary>
        public const int WM_WTSSESSION_CHANGE = 0x02B1;
        public const int WM_TABLET_FIRST = 0x02C0;
        public const int WM_TABLET_LAST = 0x02DF;
        /// <summary>
        /// An application sends a WM_CUT message to an edit control or combo box to delete (cut) the current selection, if any, in the edit control and copy the deleted text to the clipboard in CF_TEXT format.
        /// </summary>
        public const int WM_CUT = 0x0300;
        /// <summary>
        /// An application sends the WM_COPY message to an edit control or combo box to copy the current selection to the clipboard in CF_TEXT format.
        /// </summary>
        public const int WM_COPY = 0x0301;
        /// <summary>
        /// An application sends a WM_PASTE message to an edit control or combo box to copy the current content of the clipboard to the edit control at the current caret position. Data is inserted only if the clipboard contains data in CF_TEXT format.
        /// </summary>
        public const int WM_PASTE = 0x0302;
        /// <summary>
        /// An application sends a WM_CLEAR message to an edit control or combo box to delete (clear) the current selection, if any, from the edit control.
        /// </summary>
        public const int WM_CLEAR = 0x0303;
        /// <summary>
        /// An application sends a WM_UNDO message to an edit control to undo the last operation. When this message is sent to an edit control, the previously deleted text is restored or the previously added text is deleted.
        /// </summary>
        public const int WM_UNDO = 0x0304;
        /// <summary>
        /// The WM_RENDERFORMAT message is sent to the clipboard owner if it has delayed rendering a specific clipboard format and if an application has requested data in that format. The clipboard owner must render data in the specified format and place it on the clipboard by calling the SetClipboardData function.
        /// </summary>
        public const int WM_RENDERFORMAT = 0x0305;
        /// <summary>
        /// The WM_RENDERALLFORMATS message is sent to the clipboard owner before it is destroyed, if the clipboard owner has delayed rendering one or more clipboard formats. For the content of the clipboard to remain available to other applications, the clipboard owner must render data in all the formats it is capable of generating, and place the data on the clipboard by calling the SetClipboardData function.
        /// </summary>
        public const int WM_RENDERALLFORMATS = 0x0306;
        /// <summary>
        /// The WM_DESTROYCLIPBOARD message is sent to the clipboard owner when a call to the EmptyClipboard function empties the clipboard.
        /// </summary>
        public const int WM_DESTROYCLIPBOARD = 0x0307;
        /// <summary>
        /// The WM_DRAWCLIPBOARD message is sent to the first window in the clipboard viewer chain when the content of the clipboard changes. This enables a clipboard viewer window to display the new content of the clipboard.
        /// </summary>
        public const int WM_DRAWCLIPBOARD = 0x0308;
        /// <summary>
        /// The WM_PAINTCLIPBOARD message is sent to the clipboard owner by a clipboard viewer window when the clipboard contains data in the CF_OWNERDISPLAY format and the clipboard viewer's client area needs repainting.
        /// </summary>
        public const int WM_PAINTCLIPBOARD = 0x0309;
        /// <summary>
        /// The WM_VSCROLLCLIPBOARD message is sent to the clipboard owner by a clipboard viewer window when the clipboard contains data in the CF_OWNERDISPLAY format and an event occurs in the clipboard viewer's vertical scroll bar. The owner should scroll the clipboard image and update the scroll bar values.
        /// </summary>
        public const int WM_VSCROLLCLIPBOARD = 0x030A;
        /// <summary>
        /// The WM_SIZECLIPBOARD message is sent to the clipboard owner by a clipboard viewer window when the clipboard contains data in the CF_OWNERDISPLAY format and the clipboard viewer's client area has changed size.
        /// </summary>
        public const int WM_SIZECLIPBOARD = 0x030B;
        /// <summary>
        /// The WM_ASKCBFORMATNAME message is sent to the clipboard owner by a clipboard viewer window to request the name of a CF_OWNERDISPLAY clipboard format.
        /// </summary>
        public const int WM_ASKCBFORMATNAME = 0x030C;
        /// <summary>
        /// The WM_CHANGECBCHAIN message is sent to the first window in the clipboard viewer chain when a window is being removed from the chain.
        /// </summary>
        public const int WM_CHANGECBCHAIN = 0x030D;
        /// <summary>
        /// The WM_HSCROLLCLIPBOARD message is sent to the clipboard owner by a clipboard viewer window. This occurs when the clipboard contains data in the CF_OWNERDISPLAY format and an event occurs in the clipboard viewer's horizontal scroll bar. The owner should scroll the clipboard image and update the scroll bar values.
        /// </summary>
        public const int WM_HSCROLLCLIPBOARD = 0x030E;
        /// <summary>
        /// This message informs a window that it is about to receive the keyboard focus, giving the window the opportunity to realize its logical palette when it receives the focus.
        /// </summary>
        public const int WM_QUERYNEWPALETTE = 0x030F;
        /// <summary>
        /// The WM_PALETTEISCHANGING message informs applications that an application is going to realize its logical palette.
        /// </summary>
        public const int WM_PALETTEISCHANGING = 0x0310;
        /// <summary>
        /// This message is sent by the OS to all top-level and overlapped windows after the window with the keyboard focus realizes its logical palette.
        /// This message enables windows that do not have the keyboard focus to realize their logical palettes and update their client areas.
        /// </summary>
        public const int WM_PALETTECHANGED = 0x0311;
        /// <summary>
        /// The WM_HOTKEY message is posted when the user presses a hot key registered by the RegisterHotKey function. The message is placed at the top of the message queue associated with the thread that registered the hot key.
        /// </summary>
        public const int WM_HOTKEY = 0x0312;
        /// <summary>
        /// The WM_PRINT message is sent to a window to request that it draw itself in the specified device context, most commonly in a printer device context.
        /// </summary>
        public const int WM_PRINT = 0x0317;
        /// <summary>
        /// The WM_PRINTCLIENT message is sent to a window to request that it draw its client area in the specified device context, most commonly in a printer device context.
        /// </summary>
        public const int WM_PRINTCLIENT = 0x0318;
        /// <summary>
        /// The WM_APPCOMMAND message notifies a window that the user generated an application command event, for example, by clicking an application command button using the mouse or typing an application command key on the keyboard.
        /// </summary>
        public const int WM_APPCOMMAND = 0x0319;
        /// <summary>
        /// The WM_THEMECHANGED message is broadcast to every window following a theme change event. Examples of theme change events are the activation of a theme, the deactivation of a theme, or a transition from one theme to another.
        /// </summary>
        public const int WM_THEMECHANGED = 0x031A;
        /// <summary>
        /// Sent when the contents of the clipboard have changed.
        /// </summary>
        public const int WM_CLIPBOARDUPDATE = 0x031D;
        /// <summary>
        /// WM_DWMNCRENDERINGCHANGED is called when the non-client area rendering status of a window has changed. Only windows that have set the flag DWM_BLURBEHIND.fTransitionOnMaximized to true will get this message.
        /// </summary>
        public const int WM_DWMNCRENDERINGCHANGED = 0x031F;
        /// <summary>
        /// Sent to all top-level windows when the colorization color has changed.
        /// </summary>
        public const int WM_DWMCOLORIZATIONCOLORCHANGED = 0x0320;
        /// <summary>
        /// WM_DWMWINDOWMAXIMIZEDCHANGE will let you know when a DWM composed window is maximized. You also have to register for this message as well. You'd have other windowd go opaque when this message is sent.
        /// </summary>
        public const int WM_DWMWINDOWMAXIMIZEDCHANGE = 0x0321;
        /// <summary>
        /// Sent to request extended title bar information. A window receives this message through its WindowProc function.
        /// </summary>
        public const int WM_GETTITLEBARINFOEX = 0x033F;
        public const int WM_HANDHELDFIRST = 0x0358;
        public const int WM_HANDHELDLAST = 0x035F;
        public const int WM_AFXFIRST = 0x0360;
        public const int WM_AFXLAST = 0x037F;
        public const int WM_PENWINFIRST = 0x0380;
        public const int WM_PENWINLAST = 0x038F;

        /// <summary>
        /// The WM_APP constant is used by applications to help define private messages, usually of the form WM_APP+X, where X is an integer value.
        /// </summary>
        public const int WM_APP = 0x8000;
        /// <summary>
        /// The WM_USER constant is used by applications to help define private messages for use by private window classes, usually of the form WM_USER+X, where X is an integer value.
        /// </summary>
        public const int WM_USER = 0x0400;
        /// <summary>
        /// An application sends the WM_CPL_LAUNCH message to Windows Control Panel to request that a Control Panel application be started.
        /// </summary>
        public const int WM_CPL_LAUNCH = WM_USER + 0x1000;
        /// <summary>
        /// The WM_CPL_LAUNCHED message is sent when a Control Panel application, started by the WM_CPL_LAUNCH message, has closed. The WM_CPL_LAUNCHED message is sent to the window identified by the wParam parameter of the WM_CPL_LAUNCH message that started the application.
        /// </summary>
        public const int WM_CPL_LAUNCHED = WM_USER + 0x1001;
        /// <summary>
        /// WM_SYSTIMER is a well-known yet still undocumented message. Windows uses WM_SYSTIMER for internal actions like scrolling.
        /// </summary>
        public const int WM_SYSTIMER = 0x118;

        #endregion

        /// <summary>
        /// Windows Messages defined in winuser.h from Windows SDK v6.1. Documentation from MSDN.
        /// </summary>
        [Obsolete("Use const int WM_ instead.",true)]
        public enum WM : uint
        {
        }

        /// <summary>
        /// The WM_SYSCOMMAND wParam parameter values.
        /// Prefixed with SC_ in Win.h.
        /// </summary>
        public enum WMSysCommand : int
        {
            SIZE = 0xF000,
            MOVE = 0xF010,
            MINIMIZE = 0xF020,
            MAXIMIZE = 0xF030,
            NEXTWINDOW = 0xF040,
            PREVWINDOW = 0xF050,
            CLOSE = 0xF060,
            VSCROLL = 0xF070,
            HSCROLL = 0xF080,
            MOUSEMENU = 0xF090,
            KEYMENU = 0xF100,
            ARRANGE = 0xF110,
            RESTORE = 0xF120,
            TASKLIST = 0xF130,
            SCREENSAVE = 0xF140,
            HOTKEY = 0xF150,

            // WINVER >= 0x0400 ==> from Win95 and above.
            DEFAULT = 0xF160,
            MONITORPOWER = 0xF170,
            CONTEXTHELP = 0xF180,
            SEPARATOR = 0xF00F,
            // /WINVER >= 0x0400
        }

        /// <summary>
        /// WM_SIZE wParam parameter values.
        /// </summary>
        public const int SIZE_MAXHIDE = 4;
        /// <summary>
        /// WM_SIZE wParam parameter values.
        /// </summary>
        public const int SIZE_MAXSHOW = 3;
        /// <summary>
        /// WM_SIZE wParam parameter values.
        /// </summary>
        public const int SIZE_MAXIMIZED = 2;
        /// <summary>
        /// WM_SIZE wParam parameter values.
        /// </summary>
        public const int SIZE_MINIMIZED = 1;
        /// <summary>
        /// WM_SIZE wParam parameter values.
        /// </summary>
        public const int SIZE_RESTORED = 0;
    
        /// <summary>
        /// SetWindowPos flags.
        /// </summary>
        public enum SetWindowPosFlags : uint
        {
            /// <summary>
            /// SWP_ASYNCWINDOWPOS - If the calling thread and the thread that owns the window are attached to different input queues,
            /// the system posts the request to the thread that owns the window. This prevents the calling thread from
            /// blocking its execution while other threads process the request.
            /// </summary>
            SynchronousWindowPosition = 0x4000,
            /// <summary>
            /// SWP_DEFERERASE - Prevents generation of the WM_SYNCPAINT message.
            /// </summary>
            DeferErase = 0x2000,
            /// <summary>
            /// SWP_DRAWFRAME - Draws a frame (defined in the window's class description) around the window.
            /// </summary>
            DrawFrame = 0x0020,
            /// <summary>
            /// SWP_FRAMECHANGED - Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message to
            /// the window, even if the window's size is not being changed. If this flag is not specified, WM_NCCALCSIZE
            /// is sent only when the window's size is being changed.
            /// </summary>
            FrameChanged = 0x0020,
            /// <summary>
            /// SWP_HIDEWINDOW - Hides (actually minimizes) the window.
            /// </summary>
            HideWindow = 0x0080,
            /// <summary>
            /// SWP_NOACTIVATE - Does not activate the window. If this flag is not set, the window is activated and moved to the
            /// top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter
            /// parameter).
            /// </summary>
            DoNotActivate = 0x0010,
            /// <summary>
            /// SWP_NOCOPYBITS - Discards the entire contents of the client area. If this flag is not specified, the valid
            /// contents of the client area are saved and copied back into the client area after the window is sized or
            /// repositioned.
            /// </summary>
            DoNotCopyBits = 0x0100,
            /// <summary>
            /// SWP_NOMOVE - Retains the current position (ignores X and Y parameters).
            /// </summary>
            IgnoreMove = 0x0002,
            /// <summary>
            /// SWP_NOOWNERZORDER - Does not change the owner window's position in the Z order.
            /// </summary>
            DoNotChangeOwnerZOrder = 0x0200,
            /// <summary>
            /// SWP_NOREDRAW - Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies to
            /// the client area, the nonclient area (including the title bar and scroll bars), and any part of the parent
            /// window uncovered as a result of the window being moved. When this flag is set, the application must
            /// explicitly invalidate or redraw any parts of the window and parent window that need redrawing.
            /// </summary>
            DoNotRedraw = 0x0008,
            /// <summary>
            /// SWP_NOREPOSITION - Same as the SWP_NOOWNERZORDER flag.
            /// </summary>
            DoNotReposition = 0x0200,
            /// <summary>
            /// SWP_NOSENDCHANGING - Prevents the window from receiving the WM_WINDOWPOSCHANGING message.
            /// </summary>
            DoNotSendChangingEvent = 0x0400,
            /// <summary>
            /// SWP_NOSIZE - Retains the current size (ignores the cx and cy parameters).
            /// </summary>
            IgnoreResize = 0x0001,
            /// <summary>
            /// SWP_NOZORDER - Retains the current Z order (ignores the hWndInsertAfter parameter).
            /// </summary>
            IgnoreZOrder = 0x0004,
            /// <summary>
            /// SWP_SHOWWINDOW - Displays the window.
            /// </summary>
            ShowWindow = 0x0040,
        }


        [StructLayout( LayoutKind.Sequential )]
        public struct WindowPos
        {
            public IntPtr Hwnd;
            public IntPtr HwndInsertAfter;
            public int X;
            public int Y;
            public int Cx;
            public int Cy;
            public SetWindowPosFlags Flags;
        }

        /// <summary>
        /// Special window handles
        /// Prefixed with HWND_ in Win.h.
        /// </summary>
        public enum SpecialWindowHandles
        {
            /// <summary>
            /// Places the window at the bottom of the Z order. 
            /// If the hWnd parameter identifies a topmost window, the window loses its topmost status and is placed at the bottom of all other windows.
            /// </summary>
            TOP = 0,
            /// <summary>
            /// Places the window above all non-topmost windows (that is, behind all topmost windows). 
            /// This flag has no effect if the window is already a non-topmost window.
            /// </summary>
            BOTTOM = 1,
            /// <summary>
            /// Places the window at the top of the Z order.
            /// </summary>
            TOPMOST = -1,
            /// <summary>
            /// Places the window above all non-topmost windows. The window maintains its topmost position even when it is deactivated.
            /// </summary>
            NOTOPMOST = -2
        }
        
        [StructLayout( LayoutKind.Sequential )]
        public struct Margins
        {
            public int LeftWidth;
            public int RightWidth;
            public int TopHeight;
            public int BottomHeight;
        }

        [StructLayout( LayoutKind.Sequential )]
        public struct Size
        {
            public int Cx;
            public int Cy;
        };

        /// <summary>
        /// Defines a Rect structure with its coordinates.
        /// </summary>
        [StructLayout( LayoutKind.Sequential )]
        public struct Rect
        {
            public int Left;
            public int Top;
            public int Right;
            public int Bottom;

            public Rect( int leftTopRightBottom )
            {
                Left = Top = Right = Bottom = leftTopRightBottom;
            }

            public Rect( int leftRight, int topBottom )
            {
                Left = Right = leftRight;
                Top = Bottom = topBottom;
            }

            public bool Contains( Rect r )
            {
                return Left <= r.Left && Top <= r.Top && Right >= r.Right && Bottom >= r.Bottom;
            }

            public int Width { get { return Right - Left; } }
            
            public int Height { get { return Bottom - Top; } }

        }

        /// <summary>
        /// Defines a point structure with its coordinates.
        /// </summary>
        [StructLayout( LayoutKind.Sequential )]
        public struct Point
        {
            /// <summary>
            /// X coordinate of the point.
            /// </summary>
            public int X;
            /// <summary>
            /// Y coordinate of the point.
            /// </summary>
            public int Y;

            public Point( int x, int y )
            {
                X = x;
                Y = y;
            }
        }

        #region GetModuleHandleEx flags
        /// <summary>
        /// The lpModuleName parameter is an address in the module.
        /// </summary>
        public const uint GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS = 0x00000004;

        /// <summary>
        /// The module stays loaded until the process is terminated, no matter how many times FreeLibrary is called.
        /// This option cannot be used with GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT.
        /// </summary>
        public const uint GET_MODULE_HANDLE_EX_FLAG_PIN = 0x00000001;

        /// <summary>
        /// The reference count for the module is not incremented.
        /// This option is equivalent to the behavior of GetModuleHandle (pre Windows XP & 2003 Server).
        /// Do not pass the retrieved module handle to the FreeLibrary function; doing so can cause the DLL to be unmapped prematurely. 
        /// For more information, see .
        /// This option cannot be used with GET_MODULE_HANDLE_EX_FLAG_PIN.
        /// </summary>
        public const uint GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT = 0x00000002;
        #endregion

        /// <summary>
        /// Type of hook. See <see cref="User32Api.SetWindowsHookEx"/>.
        /// </summary>
        public enum HookType : int
        {
            /// <summary>
            /// The WH_JOURNALRECORD hook enables you to monitor and record input events.
            /// Typically, you use this hook to record a sequence of mouse and keyboard events to play back later by using the WH_JOURNALPLAYBACK Hook.
            /// The WH_JOURNALRECORD hook is a global hook — it cannot be used as a thread-specific hook.
            /// </summary>
            WH_JOURNALRECORD = 0,
            /// <summary>
            /// The WH_JOURNALPLAYBACK hook enables an application to insert messages into the system message queue.
            /// You can use this hook to play back a series of mouse and keyboard events recorded earlier by using the WH_JOURNALRECORD Hook.
            /// Regular mouse and keyboard input is disabled as long as a WH_JOURNALPLAYBACK hook is installed.
            /// A WH_JOURNALPLAYBACK hook is a global hook — it cannot be used as a thread-specific hook.
            /// </summary>
            WH_JOURNALPLAYBACK = 1,
            /// <summary>
            /// The WH_KEYBOARD hook enables an application to monitor message traffic for WM_KEYDOWN and WM_KEYUP messages
            /// about to be returned by the GetMessage or PeekMessage function.
            /// You can use the WH_KEYBOARD hook to monitor keyboard input posted to a message queue.
            /// </summary>
            WH_KEYBOARD = 2,
            /// <summary>
            /// The WH_GETMESSAGE hook enables an application to monitor messages about to be returned by the GetMessage or PeekMessage function.
            /// You can use the WH_GETMESSAGE hook to monitor mouse and keyboard input and other messages posted to the message queue.
            /// </summary>
            WH_GETMESSAGE = 3,
            /// <summary>
            /// The WH_CALLWNDPROC and WH_CALLWNDPROCRET hooks enable you to monitor messages sent to window procedures.
            /// The system calls a WH_CALLWNDPROC hook procedure before passing the message to the receiving window procedure,
            /// and calls the WH_CALLWNDPROCRET hook procedure after the window procedure has processed the message.
            /// </summary>
            WH_CALLWNDPROC = 4,
            /// <summary>
            /// The system calls a WH_CBT hook procedure before activating, creating, destroying, minimizing, maximizing, moving, or sizing a window;
            /// before completing a system command; before removing a mouse or keyboard event from the system message queue;
            /// before setting the input focus; or before synchronizing with the system message queue.
            /// The value the hook procedure returns determines whether the system allows or prevents one of these operations.
            /// The WH_CBT hook is intended primarily for computer-based training (CBT) applications.
            /// </summary>
            WH_CBT = 5,
            /// <summary>
            /// The WH_MSGFILTER and WH_SYSMSGFILTER hooks enable you to monitor messages about to be processed by a menu,
            /// scroll bar, message box, or dialog box, and to detect when a different window is about to be activated as a result of the user's
            /// pressing the ALT+TAB or ALT+ESC key combination.
            /// The WH_MSGFILTER hook can only monitor messages passed to a menu, scroll bar,
            /// message box, or dialog box created by the application that installed the hook procedure.
            /// The WH_SYSMSGFILTER hook monitors such messages for all applications.
            /// 
            /// The WH_MSGFILTER and WH_SYSMSGFILTER hooks enable you to perform message filtering during modal loops that is equivalent to the filtering
            /// done in the main message loop. For example, an application often examines a new message in the main loop between the time it
            /// retrieves the message from the queue and the time it dispatches the message,
            /// performing special processing as appropriate. However, during a modal loop, the system retrieves and dispatches messages without allowing
            /// an application the chance to filter the messages in its main message loop.
            /// If an application installs a WH_MSGFILTER or WH_SYSMSGFILTER hook procedure, the system calls the procedure during the modal loop.
            /// </summary>
            WH_SYSMSGFILTER = 6,
            /// <summary>
            /// The WH_MOUSE hook enables you to monitor mouse messages about to be returned by the GetMessage or PeekMessage function.
            /// You can use the WH_
            /// MOUSE hook to monitor mouse input posted to a message queue.
            /// </summary>
            WH_MOUSE = 7,
            /// <summary>
            /// Unsupported on Windows.
            /// </summary>
            WH_HARDWARE = 8,
            /// <summary>
            /// The system calls a WH_DEBUG hook procedure before calling hook procedures associated with any other hook in the system.
            /// You can use this hook to determine whether to allow the system to call hook procedures associated with other types of hooks.
            /// </summary>
            WH_DEBUG = 9,
            /// <summary>
            /// A shell application can use the WH_SHELL hook to receive important notifications.
            /// The system calls a WH_SHELL hook procedure when the shell application is about to be activated and when a top-level window is created or destroyed.
            /// </summary>
            WH_SHELL = 10,
            /// <summary>
            /// The WH_FOREGROUNDIDLE hook enables you to perform low priority tasks during times when its foreground thread is idle.
            /// The system calls a WH_FOREGROUNDIDLE hook procedure when the application's foreground thread is about to become idle.
            /// </summary>
            WH_FOREGROUNDIDLE = 11,
            /// <summary>
            /// The WH_CALLWNDPROC and WH_CALLWNDPROCRET hooks enable you to monitor messages sent to window procedures.
            /// The system calls a WH_CALLWNDPROC hook procedure before passing the message to the receiving window procedure,
            /// and calls the WH_CALLWNDPROCRET hook procedure after the window procedure has processed the message.
            /// </summary>
            WH_CALLWNDPROCRET = 12,
            /// <summary>
            /// The WH_KEYBOARD_LL hook enables you to monitor keyboard input events about to be posted in a thread input queue.
            /// </summary>
            WH_KEYBOARD_LL = 13,
            /// <summary>
            /// The WH_MOUSE_LL hook enables you to monitor mouse input events about to be posted in a thread input queue.
            /// </summary>
            WH_MOUSE_LL = 14
        }

        /// <summary>
        /// Signatures for hooking functions.
        /// </summary>
        /// <param name="code">
        /// The hook code. If nCode is less than zero, the hook procedure must pass the message to the CallNextHookEx 
        /// function without further processing and should return the value returned by CallNextHookEx. The parameter postitive values depends on the <see cref="HookType"/>.</param>
        /// <param name="wParam"></param>
        /// <param name="lParam"></param>
        /// <returns></returns>
        public delegate int HookProc( int code, IntPtr wParam, IntPtr lParam );

        [NativeDll(DefaultDllNameGeneric="user32")]
        public interface User32Api
        {
            /// <summary>
            /// Retrieves the position of the mouse cursor, in screen coordinates.
            /// </summary>
            /// <param name="point">Receives the screen coordinates of the cursor.</param>
            /// <returns>Returns true if successful. To get extended error information, call <see cref="Marshal.GetLastWin32Error"/>.</returns>
            [CK.Interop.DllImport( SetLastError = true )]
            bool GetCursorPos( out Point point );

            /// <summary>
            /// Moves the cursor to the specified screen coordinates. If the new coordinates are not within the screen rectangle set by the most recent ClipCursor function call, 
            /// the system automatically adjusts the coordinates so that the cursor stays within the rectangle. 
            /// </summary>
            /// <param name="x">The new x-coordinate of the cursor, in screen coordinates.</param>
            /// <param name="y">The new y-coordinate of the cursor, in screen coordinates.</param>
            /// <returns>Returns true if successful. To get extended error information, call <see cref="Marshal.GetLastWin32Error"/>.</returns>
            [CK.Interop.DllImport]
            bool SetCursorPos( int x, int y );

            /// <summary>
            /// Retrieves a module handle for the specified module and increments the module's reference count unless
            /// <see cref="GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT"/> is specified.
            /// The module must have been loaded by the calling process.
            /// See http://msdn.microsoft.com/en-us/library/windows/desktop/ms683200%28v=vs.85%29.aspx.
            /// </summary>
            /// <param name="dwFlags">
            /// This parameter can be zero or one or more of the following values. 
            /// If the module's reference count is incremented, the caller must use the FreeLibrary function 
            /// to decrement the reference count when the module handle is no longer needed.
            /// See <see cref="GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS"/>, <see cref="GET_MODULE_HANDLE_EX_FLAG_PIN"/> and <see cref="GET_MODULE_HANDLE_EX_FLAG_UNCHANGED_REFCOUNT"/>.
            /// </param>
            /// <param name="moduleName">
            /// The name of the loaded module (either a .dll or .exe file), or an address in the module (if dwFlags is GET_MODULE_HANDLE_EX_FLAG_FROM_ADDRESS).
            /// For a module name, if the file name extension is omitted, the default library extension .dll is appended. 
            /// The file name string can include a trailing point character (.) to indicate that the module name has no extension. 
            /// The string does not have to specify a path. When specifying a path, be sure to use backslashes (\), not forward slashes (/). 
            /// The name is compared (case independently) to the names of modules currently mapped into the address space of the calling process.
            /// If this parameter is NULL, the function returns a handle to the file used to create the calling process (.exe file).
            /// </param>
            /// <param name="hModule">
            /// A handle to the specified module. If the function fails, this parameter is NULL.
            /// The GetModuleHandleEx function does not retrieve handles for modules that were loaded using the LOAD_LIBRARY_AS_DATAFILE flag. For more information, see LoadLibraryEx.
            /// </param>
            /// <returns>Returns true if successful. To get extended error information, call <see cref="Marshal.GetLastWin32Error"/>.</returns>
            [CK.Interop.DllImport]
            bool GetModuleHandleEx( uint dwFlags, string moduleName, out IntPtr hModule );

            /// <summary>
            /// Retrieves a handle to the foreground window (the window with which the user is currently working).
            /// The system assigns a slightly higher priority to the thread that creates the foreground window than it does to other threads.
            /// </summary>
            /// <returns>
            /// The return value is a handle to the foreground window.
            /// The foreground window can be NULL in certain circumstances, such as when a window is losing activation. 
            /// </returns>
            [CK.Interop.DllImport]
            IntPtr GetForegroundWindow();

            /// <summary>
            /// Return type: BOOL
            /// If the window was brought to the foreground, the return value is nonzero.
            /// If the window was not brought to the foreground, the return value is zero.
            /// </summary>
            /// <param name="hWnd">Handle of the window.</param>
            /// <returns>
            /// If the window was brought to the foreground, the return value is nonzero.
            /// If the window was not brought to the foreground, the return value is zero.
            /// </returns>
            [CK.Interop.DllImport]
            int SetForegroundWindow( IntPtr hWnd );

            /// <summary>
            /// Sends the specified message to a window or windows. 
            /// The SendMessage function calls the window procedure for the specified window and does not return until the window procedure has processed the message.
            /// </summary>
            /// <param name="hWnd">
            /// A handle to the window whose window procedure will receive the message. 
            /// If this parameter is HWND_BROADCAST ((HWND)0xffff), the message is sent to all top-level windows in the system, 
            /// including disabled or invisible unowned windows, overlapped windows, and pop-up windows; but the message is not sent to child windows.
            /// Message sending is subject to UIPI. The thread of a process can send messages only to message queues of threads in processes of lesser or equal integrity level.
            /// </param>
            /// <param name="msg">The message to be sent.</param>
            /// <param name="wParam">Additional message-specific information.</param>
            /// <param name="lParam">Additional message-specific information.</param>
            /// <returns>The return value specifies the result of the message processing; it depends on the message sent.</returns>
            [CK.Interop.DllImport( CharSet = CharSet.Auto )]
            IntPtr SendMessage( IntPtr hWnd, UInt32 msg, IntPtr wParam, IntPtr lParam );

            /// <summary>
            /// Calls the default window procedure to provide default processing for any window messages that an application does not process.
            /// This function ensures that every message is processed. DefWindowProc is called with the same parameters received by the window procedure. 
            /// </summary>
            /// <param name="hWnd">A handle to the window procedure that received the message.</param>
            /// <param name="uMsg">The message.</param>
            /// <param name="wParam">Additional message information. The content of this parameter depends on the value of the Msg parameter.</param>
            /// <param name="lParam">Additional message information. The content of this parameter depends on the value of the Msg parameter.</param>
            /// <returns>The return value is the result of the message processing and depends on the message.</returns>
            [CK.Interop.DllImport]
            IntPtr DefWindowProc( IntPtr hWnd, int uMsg, IntPtr wParam, IntPtr lParam );

            /// <summary>
            /// Retrieves the identifier of the thread that created the specified window and the identifier of the process that created the window.
            /// </summary>
            /// <param name="hwnd">A handle to the window.</param>
            /// <returns>The return value is the identifier of the thread that created the window.</returns>
            /// <remarks>http://msdn.microsoft.com/en-us/library/ms633522%28v=vs.85%29.aspx</remarks>
            [CK.Interop.DllImport( SetLastError = true )]
            uint GetWindowThreadProcessId( IntPtr hWnd, out uint processId );

            /// <summary>
            /// Retrieves the thread identifier of the calling thread.
            /// </summary>
            /// <returns>The return value is the thread identifier of the calling thread.</returns>
            [CK.Interop.DllImport( DllNameGeneric = "kernel32.dll" )]
            uint GetCurrentThreadId();

            /// <summary>
            /// Installs an application-defined hook procedure into a hook chain. You would install a hook procedure to monitor the system for certain types of events. 
            /// These events are associated either with a specific thread or with all threads in the same desktop as the calling thread. 
            /// See http://msdn.microsoft.com/en-us/library/windows/desktop/ms644990%28v=vs.85%29.aspx.
            /// </summary>
            /// <param name="idHook">Type of hook. See <see cref="HookType"/>.</param>
            /// <param name="hookFunction">Hook function.</param>
            /// <param name="hInstance">
            /// A handle to the DLL containing the hook procedure pointed to by the hookFunction parameter.
            /// The hInstance parameter must be set to <see cref="IntPtr.Zero"/> if the threadId parameter specifies a thread created by the current process and if the 
            /// hook procedure is within the code associated with the current process.
            /// </param>
            /// <param name="threadId">
            /// The identifier of the thread with which the hook procedure is to be associated. 
            /// For desktop apps, if this parameter is zero, the hook procedure is associated with all existing threads running 
            /// in the same desktop as the calling thread.
            /// </param>
            /// <returns>
            /// If the function succeeds, the return value is the handle to the hook procedure.
            /// If the function fails, the return value is NULL. To get extended error information, call <see cref="Marshal.GetLastWin32Error"/>.
            /// </returns>
            [CK.Interop.DllImport]
            IntPtr SetWindowsHookEx( HookType idHook, HookProc hookFunction, IntPtr hInstance, uint threadId );

            /// <summary>
            /// A handle to the hook to be removed. This parameter is a hook handle obtained by a previous call to <see cref="SetWindowsHookEx"/>. 
            /// </summary>
            /// <param name="idHook">Hook identifier.</param>
            /// <returns>Returns true if successful. To get extended error information, call <see cref="Marshal.GetLastWin32Error"/>.</returns>
            [CK.Interop.DllImport]
            bool UnhookWindowsHookEx( IntPtr idHook );

            /// <summary>
            /// Passes the hook information to the next hook procedure in the current hook chain. A hook procedure can call this function either before or after processing the hook information.
            /// </summary>
            /// <param name="idHook">This parameter is ignored.</param>
            /// <param name="nCode">The hook code passed to the current hook procedure. The next hook procedure uses this code to determine how to process the hook information.</param>
            /// <param name="wParam">The wParam value passed to the current hook procedure. The meaning of this parameter depends on the type of hook associated with the current hook chain.</param>
            /// <param name="lParam">The lParam value passed to the current hook procedure. The meaning of this parameter depends on the type of hook associated with the current hook chain.</param>
            /// <returns>
            /// This value is returned by the next hook procedure in the chain. The current hook procedure must also return this value. The meaning of the return value depends on the hook type. 
            /// For more information, see the descriptions of the individual hook procedures.</returns>
            [CK.Interop.DllImport]
            int CallNextHookEx( IntPtr idHook, int nCode, IntPtr wParam, IntPtr lParam );

            [CK.Interop.DllImport( EntryPoint32 = "SetWindowLong", EntryPoint64 = "SetWindowLongPtr", ExactSpelling = false )]
            IntPtr SetWindowLong( IntPtr hWnd, WindowLongIndex index, uint dwNewLong );

            [CK.Interop.DllImport( EntryPoint32 = "GetWindowLong", EntryPoint64 = "GetWindowLongPtr", ExactSpelling = false )]
            IntPtr GetWindowLong( IntPtr hWnd, WindowLongIndex index );

            [CK.Interop.DllImport( SetLastError = true )]
            bool SetWindowPos( IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags flags );

            [CK.Interop.DllImport]
            bool GetClientRect( IntPtr hwnd, out Rect r );

            [CK.Interop.DllImport]
            bool GetWindowRect( IntPtr hwnd, out Rect r );

            [CK.Interop.DllImport( SetLastError = true )]
            bool RegisterHotKey( IntPtr hWnd, int id, uint fsModifiers, uint vk );
            [CK.Interop.DllImport( SetLastError = true )]
            bool UnregisterHotKey( IntPtr hWnd, int id );
        }

    }

}
