#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Core\Helpers\RegionHelper.cs) is part of CiviKey. 
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
using System.Text;
using CK.Core;

namespace BasicCommandHandlers
{
    /// <summary>
    /// This class enbales creating a zone from <see cref="Rectangle"/> objects. We can then test whether a point is the zone.
    /// It also contains two extension methods that enables knocing whether a point is in a rectangle.
    /// </summary>
    public class RegionHelper
    {
        Region _region;
        List<Rectangle> _includedRectangles;
        List<Rectangle> _excludedRectangles;

        /// <summary>
        /// Rectangles contained in the <see cref="Region"/>.
        /// </summary>
        public IReadOnlyList<Rectangle> IncludedRectangles
        {
            get { return _includedRectangles.ToReadOnlyList(); }
        }

        /// <summary>
        /// Rectangles excluded from the <see cref="Region"/>.
        /// </summary>
        public IReadOnlyList<Rectangle> ExcludedRectangles
        {
            get { return _excludedRectangles.ToReadOnlyList(); }
        }

        /// <summary>
        /// Default Constructor
        /// </summary>
        public RegionHelper()
        {
            _includedRectangles = new List<Rectangle>();
            _excludedRectangles = new List<Rectangle>();
        }

        /// <summary>
        /// Calls the default constructor and adds the rectangle.
        /// </summary>
        /// <param name="rectangle">Rectangle to add</param>
        public RegionHelper( Rectangle rectangle )
            : this()
        {
            Add( rectangle );
        }

        /// <summary>
        /// Calls the default constructor and adds rectangles
        /// </summary>
        /// <param name="rectangles">Rectangles to add</param>
        public RegionHelper( IEnumerable<Rectangle> rectangles )
            : this()
        {
            Add( rectangles );
        }

        /// <summary>
        /// Adds a new rectangle into the zone
        /// </summary>
        /// <param name="rectangle">
        /// Rectangle to add, if the rectangle exists in ExcludedRectangles, it is removed. 
        /// Otherwise, the rectangle is added in IncludedRectangles.
        /// </param>
        public void Add( Rectangle rectangle )
        {
            if( _region == null )
            {
                _region = new Region( rectangle );
                _includedRectangles.Add( rectangle );
            }
            else
            {
                _region.Union( rectangle );
                if( !_excludedRectangles.Remove( rectangle ) ) _includedRectangles.Add( rectangle );
            }
        }

        /// <summary>
        /// Adds new rectangles into the zone
        /// </summary>
        /// <param name="rectangles">
        /// Rectangles to add, if rectangles exist in ExcludedRectangles, they are removed. 
        /// Otherwise, the rectangle are added in IncludedRectangles.
        /// </param>
        public void Add( IEnumerable<Rectangle> rectangles )
        {
            foreach( var r in rectangles ) Add( r );
        }

        /// <summary>
        /// Removes a rectangle from the zone. The rectangle does not have to have been added before. 
        /// </summary>
        /// <param name="rectangle">
        /// Rectangle to remove, if the rectangle exists in IncludedRectangles, it is removed. 
        /// Otherwise, the rectangle is added in ExcludedRectangles
        /// </param>
        public void Remove( Rectangle rectangle )
        {
            if( _region != null )
            {
                _region.Exclude( rectangle );
                //if the rectangle doesn't exist in _includedRectangles, it is added in _excludedRectangles
                if( !_includedRectangles.Remove( rectangle ) ) _excludedRectangles.Add( rectangle );
            }
        }

        /// <summary>
        /// Removes a rectangle from the zone. The rectangle does not have to have been added before. 
        /// </summary>
        /// <param name="rectangle">
        /// Rectangles to remove, if rectangles exist in IncludedRectangles, they are removed. 
        /// Otherwise, rectangles are added in ExcludedRectangles
        /// </param>
        public void Remove( IEnumerable<Rectangle> rectangles )
        {
            foreach( var r in rectangles ) Remove( r );
        }

        /// <summary>
        /// Check if the Region contains the point
        /// </summary>
        /// <param name="point">Point to check</param>
        /// <returns>Return true, if the point is contain in the Region. Otherwise, false</returns>
        public bool Contains( Point point )
        {
            return _region != null && _region.IsVisible( point );
        }

        /// <summary>
        /// Check if the Region contains the Rectangle
        /// </summary>
        /// <param name="point">Rectangle to check</param>
        /// <returns>Return true, if a part of the rectangle is contain in the Region. Otherwise, false</returns>
        public bool Contains( Rectangle rectangle )
        {
            return _region != null && _region.IsVisible( rectangle );
        }

        /// <summary>
        /// Check if the point is contained in the X min and X max bounds of the Region.
        /// </summary>
        /// <param name="point">Point to test</param>
        /// <returns>Return true, if the point is contained in the X min and X max bounds of the Region. Otherwise, return false.</returns>
        public bool ContainedInXBounds( Point point )
        {
            return _includedRectangles.Any( r => r.Left < point.X && r.Right >= point.X );
        }

        /// <summary>
        /// Check if the point is contained in the X min and X max bounds of the rectangleToTest.
        /// </summary>
        /// <param name="point">Point to test</param>
        /// <param name="rectangleToTest">Rectangle to test, if the point is included in</param>
        /// <returns>Return true, if the point is contain in X min and X max bounds of the rectangleToTest. Otherwise, return false.</returns>
        public static bool ContainedInXBounds( Point point, Rectangle rectangleToTest )
        {
            return rectangleToTest.Left <= point.X && rectangleToTest.Right > point.X;
        }

        /// <summary>
        /// Check if the point is contained in the Y min and Y max bounds of the Region.
        /// </summary>
        /// <param name="point">Point to test</param>
        /// <returns>Return true, if the point is contain in X min and X max bounds of the Region. Otherwise, return false.</returns>
        public bool ContainedInYBounds( Point point )
        {
            return _includedRectangles.Any( r => r.Top <= point.Y && r.Bottom > point.Y );
        }

        /// <summary>
        /// Check if the point is contained in the Y min and Y max bounds of the rectangleToTest.
        /// </summary>
        /// <param name="point">Point to test</param>
        /// <param name="rectangleToTest">Rectangle to test, if the point is included in</param>
        /// <returns>Return true, if the point is contain in X min and X max bounds of the rectangleToTest. Otherwise, return false.</returns>
        public static bool ContainedInYBounds( Point point, Rectangle rectangleToTest )
        {
            return rectangleToTest.Top <= point.Y && rectangleToTest.Bottom > point.Y;
        }

        /// <summary>
        /// Gets the minimum value of Y in the region for a given X value.
        /// </summary>
        /// <param name="x">X value</param>
        /// <returns>Return Y value, and 0 if X isn't contain in the region</returns>
        public int GetMinYPosition( int x )
        {
            return GetMinPosition( r => r.Left < x && r.Right >= x, r => r.Top );
        }

        /// <summary>
        /// Gets the minimum value of X in the region for a given Y value.
        /// </summary>
        /// <param name="y">Y value</param>
        /// <returns>Return X value, and 0 if Y isn't contain in the region</returns>
        public int GetMinXPosition( int y )
        {
            return GetMinPosition( r => r.Top <= y && r.Bottom > y, r => r.Left );
        }

        /// <summary>
        /// Gets the maximal value of Y in the region for a given X value.
        /// </summary>
        /// <param name="x">X value</param>
        /// <returns>Return Y value, and maximal X value in the region if X isn't contain in the region</returns>
        public int GetMaxYPosition( int x )
        {
            return GetMaxPosition( r => r.Left < x && r.Right >= x, r => r.Bottom );
        }

        /// <summary>
        /// Gets the maximal value of X in the region for a given Y value.
        /// </summary>
        /// <param name="y">Y value</param>
        /// <returns>Return X value, and maximal X value in the region if Y isn't contain in the region</returns>
        public int GetMaxXPosition( int y )
        {
            return GetMaxPosition( r => r.Top <= y && r.Bottom > y, r => r.Right );
        }

        #region Helper Methods

        int GetMinPosition( Func<Rectangle, bool> condition, Func<Rectangle, int> selector )
        {
            IEnumerable<Rectangle> e = _includedRectangles.Where( condition );
            if( e.Any() ) return e.Min( selector );
            return _includedRectangles.Count == 0 ? 0 : _includedRectangles.Min( selector );
        }

        int GetMaxPosition( Func<Rectangle, bool> condition, Func<Rectangle, int> selector )
        {
            IEnumerable<Rectangle> e = _includedRectangles.Where( condition );
            if( e.Any() ) return e.Max( selector );
            return _includedRectangles.Count == 0 ? 0 : _includedRectangles.Max( selector );
        }

        #endregion Helper Methods
    }
}
