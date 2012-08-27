#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Config\ConfigActivableSection.cs) is part of CiviKey. 
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
    public class ConfigActivableSection : ConfigItemProperty<bool>, IConfigActivableSection
    {
        public ConfigActivableSection( ConfigManager configManager, ValueProperty<bool> prop, INotifyPropertyChanged monitor )
            : base( configManager, prop, monitor )
        {
            Items = new ConfigItemCollection( this );
        }

        public ConfigActivableSection( ConfigManager configManager, object o, PropertyInfo p )
            : this( configManager, new ValueProperty<bool>( o, p ), o as INotifyPropertyChanged )
        {
        }

        public ConfigActivableSection( ConfigManager configManager, object o, PropertyInfo p, INotifyPropertyChanged monitor )
            : this( configManager, new ValueProperty<bool>( o, p ), monitor )
        {
        }

        public IObservableCollection<object> Items { get; private set; }
    }
}
