using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using System.Windows.Interop;
using System;
using System.Windows;

namespace CK.Windows.Helper
{
    public class ScreenHelper
    {
        public static bool IsInScreen( Rectangle rect )
        {
            return Screen.AllScreens.Any( ( s ) => s.WorkingArea.Contains( rect ) );
        }

        public static bool IsInScreen( System.Drawing.Point point )
        {
            return Screen.AllScreens.Any( ( s ) => s.WorkingArea.Contains( point ) );
        }

        public static bool IsInScreen( Window window )
        {
            return Screen.AllScreens.Any( ( s ) => s.WorkingArea.Contains( new Rectangle( (int)window.Top, (int)window.Left, (int)window.Width, (int)window.Left ) ) );
        }

        public static System.Drawing.Point GetCenterOfParentScreen( Rectangle rect )
        {
            Screen parent = Screen.FromRectangle( rect );
            return new System.Drawing.Point( parent.WorkingArea.Width / 2, parent.WorkingArea.Height / 2 );
        }

        public static Rectangle GetPrimaryScreenSize()
        {
            return Screen.PrimaryScreen.Bounds;
        }

    }
}
