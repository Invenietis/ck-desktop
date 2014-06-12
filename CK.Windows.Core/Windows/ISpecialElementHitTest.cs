using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace CK.Windows.Core
{
    public interface ISpecialElementHitTest
    {
        /// <summary>
        /// Allows to change the current HTCode.
        /// This method is called when the window where is the element returns true with <see cref="CKWindow.EnableSpecialElementHitTest"/>
        /// </summary>
        /// <param name="p">The point used for HitTest</param>
        /// <param name="HTCode">The current HTCode</param>
        /// <param name="hitObject">The current element hit by the current point</param>
        /// <returns></returns>
        int GetHitTestResult( Point p, int HTCode, DependencyObject hitObject );
    }
}
