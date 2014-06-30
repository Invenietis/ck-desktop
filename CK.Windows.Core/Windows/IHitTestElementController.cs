using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace CK.Windows.Core
{
    public interface IHitTestElementController
    {
        /// <summary>
        /// Allows changing the current HTCode.
        /// </summary>
        /// <param name="p">The point used for HitTest</param>
        /// <param name="HTCode">The current HTCode</param>
        /// <param name="hitObject">The element currently hit</param>
        /// <returns></returns>
        int GetHitTestResult( Point p, int HTCode, DependencyObject hitObject );
    }
}
