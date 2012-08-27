#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Config\ConfigItemExtensions.cs) is part of CiviKey. 
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using System.Reflection;
using System.ComponentModel;
using CK.Reflection;
using System.Linq.Expressions;
using System.Diagnostics;
using System.Collections.Specialized;
using System.Collections;

namespace CK.Windows.Config
{
    public static class ConfigItemExtensions
    {
        public static ConfigGroup AddGroup( this IConfigItemContainer c )
        {
            ConfigGroup g = new ConfigGroup( c.ConfigManager );
            c.Items.Add( g );
            return g;
        }

        public static ConfigItemLink AddLink( this IConfigItemContainer c, ConfigPage page )
        {
            ConfigItemLink l = new ConfigItemLink( c.ConfigManager, page, null );
            c.Items.Add( l );
            return l;
        }

        public static ConfigItemProperty<T> AddProperty<T, THolder>( this IConfigItemContainer c, THolder o, Expression<Func<THolder, T>> prop )
        {
            ConfigItemProperty<T> p = new ConfigItemProperty<T>( c.ConfigManager, o, ReflectionHelper.GetPropertyInfo( prop ) );
            c.Items.Add( p );
            return p;
        }

        public static ConfigItemProperty<T> AddProperty<T, THolder>( this IConfigItemContainer c, string displayName, THolder o, Expression<Func<THolder, T>> prop )
        {
            return AddProperty<T, THolder>( c, displayName, null, o, prop );
        }

        public static ConfigItemProperty<T> AddProperty<T, THolder>( this IConfigItemContainer c, string displayName, string description, THolder o, Expression<Func<THolder, T>> prop )
        {
            ConfigItemProperty<T> p = new ConfigItemProperty<T>( c.ConfigManager, o, ReflectionHelper.GetPropertyInfo( o, prop ) );
            p.DisplayName = displayName;
            p.Description = description;
            c.Items.Add( p );
            return p;
        }

        public static ConfigActivableSection AddActivableSection<THolder>( this IConfigItemContainer c, string displayName, string description, THolder o, Expression<Func<THolder, bool>> prop, INotifyPropertyChanged propertyMonitor )
        {
            ConfigActivableSection s = new ConfigActivableSection( c.ConfigManager, o, ReflectionHelper.GetPropertyInfo( prop ), propertyMonitor );
            s.DisplayName = displayName;
            s.Description = description;
            c.Items.Add( s );
            return s;
        }

        public static ConfigItemAction AddAction( this IConfigItemContainer c, string displayName, System.Action action )
        {
            return AddAction( c, displayName, null, action );
        }

        public static ConfigItemAction AddAction( this IConfigItemContainer c, string displayName, string description, System.Action action )
        {
            ConfigItemAction a = new ConfigItemAction( c.ConfigManager, new SimpleCommand( action ) ) { DisplayName = displayName, Description = description };
            c.Items.Add( a );
            return a;
        }

        /// <summary>       
        /// Adds a combobox bound to an ObservableCollection and a property showing the currently selected element from the collection
        /// </summary>
        /// <typeparam name="T">Type of one element of the collection</typeparam>
        /// <typeparam name="THolder">Type of the holder of the selected element property</typeparam>
        /// <param name="c"></param>
        /// <param name="displayName">Label of the collection</param>
        /// <param name="description">Description of the control</param>
        /// <param name="o">instance of the holder of the selected element property</param>
        /// <param name="prop">Expression that returns the selected element property</param>
        /// <param name="valueCollection">Function that returns the ObservableCollection</param>
        /// <param name="ensureCurrentNotNull">set to true if you want the collection to set the current element if it is null and there are elements in the valueCollection</param>
        /// <param name="noCurrentDisplayString">Displayed string when the current element is null</param>
        /// <returns></returns>
        public static ConfigItemCurrent<T> AddCurrentItem<T, THolder>( this IConfigItemContainer c, string displayName, string description, THolder o, Expression<Func<THolder, T>> prop, Func<THolder,object> valueCollection, bool ensureCurrentNotNull, string noCurrentDisplayString )
        {
            ConfigItemCurrent<T> a = new ConfigItemCurrent<T>( c.ConfigManager, o, ReflectionHelper.GetPropertyInfo( o, prop ), () => valueCollection( o ), ensureCurrentNotNull, noCurrentDisplayString ) { DisplayName = displayName, Description = description };
            c.Items.Add( a );
            return a;
        }

    }

}
