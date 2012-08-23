#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Config\ConfigItemCollection.cs) is part of CiviKey. 
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
    public class ConfigItemCollection : BindableCollection<object>
    {
        ConfigItem _holder;

        public ConfigItemCollection( ConfigItem holder )
        {
            _holder = holder;
            _holder.PropertyChanged += OnHolderPropertyChanged;
        }

        void OnHolderPropertyChanged( object o, PropertyChangedEventArgs e )
        {
            if( e.PropertyName == "Enabled" )
            {
                foreach( var c in Items.OfType<ConfigItem>() ) c.Enabled = _holder.Enabled;
            }
        }

        protected override void OnCollectionChanged( NotifyCollectionChangedEventArgs e )
        {
            IEnumerable source = null;
            if( e.Action == NotifyCollectionChangedAction.Add || e.Action == NotifyCollectionChangedAction.Replace ) source = e.NewItems;
            else if( e.Action == NotifyCollectionChangedAction.Reset ) source = Items;
            if( source != null ) foreach( var c in source.OfType<ConfigItem>() ) c.Enabled = _holder.Enabled;
            base.OnCollectionChanged( e );
        }
    }
}
