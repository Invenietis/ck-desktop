using CK.Windows.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Shapes;
using CK.Windows.Core;

namespace CK.Windows
{
    /// <summary>
    /// WPF Window that has the Aero "glossy-effect".
    /// </summary>
    public class CKWindow : Window
    {
        WindowInteropHelper _interopHelper;

        /// <summary>
        /// Defautl constructor
        /// </summary>
        public CKWindow()
            : base()
        {
            _interopHelper = new WindowInteropHelper( this );
        }

        /// <summary>
        /// Gets the Win32 window handle of this <see cref="Window"/> object.
        /// Available once <see cref="Window.SourceInitialized"/> has been raised.
        /// </summary>
        public IntPtr Hwnd { get; private set; }

        public new bool ShowInTaskbar
        {
            get { return base.ShowInTaskbar; }
            set
            {
                if( base.ShowInTaskbar != value )
                {
                    base.ShowInTaskbar = value;
                    SetToolWindowFlag( value );
                }
            }
        }

        internal void SetToolWindowFlag( bool set )
        {
            if( set )
            {
                Win.Functions.SetWindowLong(
                            Hwnd,
                            Win.WindowLongIndex.GWL_EXSTYLE,
                            (uint)Win.Functions.GetWindowLong( Hwnd, Win.WindowLongIndex.GWL_EXSTYLE ) | Win.WS_EX_TOOLWINDOW );
            }
            else
            {
                Win.Functions.SetWindowLong(
                            Hwnd,
                            Win.WindowLongIndex.GWL_EXSTYLE,
                            (uint)Win.Functions.GetWindowLong( Hwnd, Win.WindowLongIndex.GWL_EXSTYLE ) & ~Win.WS_EX_TOOLWINDOW );
            }
        }

        /// <summary>
        /// Virtual method, can be overriden in a derived class to handle the call to Hide on this CKWindow.
        /// </summary>
        public new virtual void Hide()
        {
            base.Hide();
        }

        IntPtr WndProcWinDefault( IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled )
        {
            switch( msg )
            {
                case Win.WM_NCHITTEST: //Handling the HitTest in order to move & resize the window without having any NC Area
                    {
                        int hit = Win.Functions.DefWindowProc( Hwnd, msg, wParam, lParam ).ToInt32();
                        if( hit == Win.HTCLIENT )
                        {
                            CKNCHitTest( PointFromLParam( lParam ), ref hit );
                        }
                        handled = true;
                        return new IntPtr( hit );
                    }
            }
            return IntPtr.Zero;
        }

        Point PointFromLParam( IntPtr lParam )
        {
            return new Point( (short)(lParam.ToInt32() & 0x0000FFFF), (short)((lParam.ToInt32() & 0xFFFF0000) >> 16) );
        }

        List<DependencyObject> hitResultsList = new List<DependencyObject>();
        void CKNCHitTest( Point p, ref int htCode )
        {
            IHitTestElementController controllingElement;
            var point = PointFromScreen( p );
            hitResultsList.Clear();
            VisualTreeHelper.HitTest( this, HitTestFilter, d => HitTestResult( d ), new PointHitTestParameters( point ) );
            DependencyObject result = hitResultsList.FirstOrDefault();
            if( result != null )
            {
                if( IsDraggableVisual( result ) ) //Checking if the currently hit element is a draggable one.
                {
                    htCode = Win.HTCAPTION;
                }
                else if( ResizeMode == System.Windows.ResizeMode.CanResizeWithGrip )
                {
                    if( TreeHelper.FindParentInVisualTree( result, d => d is System.Windows.Controls.Primitives.ResizeGrip ) != null )
                    {
                        if( FlowDirection == FlowDirection.RightToLeft )
                            htCode = Win.HTBOTTOMLEFT;
                        else htCode = Win.HTBOTTOMRIGHT;
                    }
                }
                else if( EnableHitTestElementController( result, p, htCode, out controllingElement ) ) //Checking whether this window is configured to handle HitTests.
                {
                    if( controllingElement != null )
                    {
                        htCode = controllingElement.GetHitTestResult( p, htCode, result );
                    }
                }
            }
            else
            {
                // Nothing was hit. Assume the extended frame.
                htCode = Win.HTCAPTION;
            }
        }

        protected override System.Windows.Media.HitTestResult HitTestCore( System.Windows.Media.PointHitTestParameters hitTestParameters )
        {
            return new PointHitTestResult( this, hitTestParameters.HitPoint );
        }

        // Return the result of the hit test to the callback.
        HitTestResultBehavior HitTestResult( HitTestResult result )
        {
            // Add the hit test result to the list that will be processed after the enumeration.
            hitResultsList.Add( result.VisualHit );

            // Set the behavior to return visuals at all z-order levels.
            return HitTestResultBehavior.Continue;
        }

        static int i = 0;
        HitTestFilterBehavior HitTestFilter( DependencyObject d )
        {
            return ((Visibility)d.GetValue( FrameworkElement.VisibilityProperty ) != Visibility.Visible)
                ? HitTestFilterBehavior.ContinueSkipSelfAndChildren
                : HitTestFilterBehavior.Continue;
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

        /// <summary>
        /// By default, the HitTest doesn't test if the hit visualElement implements <see cref="IHitTestElementController"/>.
        /// By overriding this method, we can enable the IHitTestElementController feature to override the current htCode.
        /// </summary>
        /// <param name="visualElement">The element that is hit by the HitTest</param>
        /// <param name="p">The point to test</param>
        /// <param name="currentHTCode">The current htCode</param>
        /// <param name="specialElement">
        /// Returns the <see cref="IHitTestElementController"/> element on which GetHitTestResult should be called.
        /// </param>
        /// <returns></returns>
        protected virtual bool EnableHitTestElementController( DependencyObject visualElement, Point p, int currentHTCode, out IHitTestElementController specialElement )
        {
            specialElement = null;
            return false;
        }

        #region OnXXX

        /// <summary>
        /// When the window is initialized, gets the current window's Handle.
        /// Then calls WPF Window OnSourceInitialized.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnSourceInitialized( EventArgs e )
        {
            Hwnd = _interopHelper.Handle;
            HwndSource hSource = HwndSource.FromHwnd( Hwnd );
            hSource.AddHook( WndProcWinDefault );

            base.OnSourceInitialized( e );

            if( !ShowInTaskbar )
            {
                SetToolWindowFlag( true );
            }
        }

        #endregion
    }
}
