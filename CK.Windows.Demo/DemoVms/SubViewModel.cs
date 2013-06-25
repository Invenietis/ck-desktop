#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Demo\DemoVms\SubViewModel.cs) is part of CiviKey. 
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
using CK.Windows.Config;
using System.Windows;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace CK.Windows.Demo
{
    internal class SubViewModel : ConfigPage
    {
        SpecificViewModel _specificVM;
        public SpecificViewModel SpecificVM { get { return _specificVM ?? ( _specificVM = new SpecificViewModel( ConfigManager ) ); } }

        ConfigCurrentItemTestsViewModel _profilesVM;
        public ConfigCurrentItemTestsViewModel ProfilesVM { get { return _profilesVM ?? ( _profilesVM = new ConfigCurrentItemTestsViewModel( ConfigManager ) ); } }


        public bool BoolProperty { get; set; }

        public SubViewModel( AppViewModel app, ConfigManager configManager )
            : base( configManager )
        {
            DisplayName = "Sub view";

            this.AddProperty( "Bool property", this, ( o ) => o.BoolProperty );

            this.AddLink( SpecificVM );

            this.AddLink( ProfilesVM );

            var group = this.AddGroup();

            PropertyChangedMockupClass mockup = new PropertyChangedMockupClass();
            group.AddProperty( mockup, m => m.InjectedCallerName );
            group.AddProperty( mockup, m => m.Value );

            group.AddAction( "Change values", () =>
            {
                mockup.InjectedCallerName = "CallerMemberNameAttribute works !";
                mockup.Value = "Classical process works";
            } );


        }
    }

    public class PropertyChangedMockupClass : INotifyPropertyChanged
    {
        public PropertyChangedMockupClass()
        {
            InjectedCallerName = "Click the button below";
            Value = "Click the button below";
        }

        string _injectedCallerName;
        public string InjectedCallerName
        {
            get { return _injectedCallerName; }
            set
            {
                _injectedCallerName = value;
                OnPropertyChanged();
            }
        }

        string _value;
        public string Value { get { return _value; } set { _value = value; OnPropertyChanged( "Value" ); } }

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged( [CallerMemberName] string propertyName = "" )
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if( handler != null )
            {
                var e = new PropertyChangedEventArgs( propertyName );
                handler( this, e );
            }
        }
    }
}

namespace System.Runtime.CompilerServices
{
    [AttributeUsage( AttributeTargets.Parameter, AllowMultiple = false, Inherited = true )]
    public sealed class CallerMemberNameAttribute : Attribute
    {
    }
}
