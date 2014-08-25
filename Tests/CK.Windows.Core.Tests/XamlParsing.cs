#region LGPL License
/*----------------------------------------------------------------------------
* This file (Tests\CK.Windows.Core.Tests\XamlParsing.cs) is part of CiviKey. 
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
using System.Linq;
using System.Text;
using NUnit.Framework;
using System.Xaml.Schema;
using System.Xaml;
using System.Xml;
using System.IO;
using System.Windows;
using System.ComponentModel;

namespace TestXaml.TestNamespace
{

    public class OneObject
    {
        public int Field;
    }
    
    public class OneObjectGen<T>
    {
        public T Field;
    }

    public interface IConfigItemProperty<T> : INotifyPropertyChanged
    {
        T Value { get; set; }
    }

}

namespace CK.Windows.Core
{

    [TestFixture]
    public class XamlParsing
    {
        [Test]
        [STAThread]
        public void XamlParseSimple()
        {
            var s = @"
<Window 
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:sys=""clr-namespace:System;assembly=mscorlib""
    xmlns:test=""clr-namespace:TestXaml.TestNamespace;assembly=CK.Windows.Core.Tests""
    xmlns:ck=""clr-namespace:CK.Windows;assembly=CK.Windows.Core"" >

    <Window.Resources>
        <x:Array x:Key=""Simple.Int.Array"" Type=""{x:Type sys:Int32}"">
        </x:Array>
        <x:Array x:Key=""Simple.Obj.Array"" Type=""{x:Type test:OneObject}"">
        </x:Array>
    </Window.Resources>

</Window>";

            var ctx = new XamlSchemaContext();
            using( var text = new StringReader( s ) )
            using( var reader = new XmlTextReader( text ) )
            {
                var w = (Window)System.Windows.Markup.XamlReader.Load( reader );
                Assert.That( w.Resources["Simple.Obj.Array"], Is.InstanceOf<TestXaml.TestNamespace.OneObject[]>() );
            }
        }

        [Test]
        [STAThread]
        public void XamlParseGenericType()
        {
            var s = @"
<Window 
    xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""
    xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""
    xmlns:sys=""clr-namespace:System;assembly=mscorlib""
    xmlns:test=""clr-namespace:TestXaml.TestNamespace;assembly=CK.Windows.Core.Tests""
    xmlns:ck=""clr-namespace:CK.Windows;assembly=CK.Windows.Core"" >

    <Window.Resources>

        <x:Array x:Key=""Sample"" Type=""{x:Type test:OneObjectGen(sys:Int32)}""></x:Array>
        <x:Array x:Key=""Sample1"" Type=""{x:Type test:IConfigItemProperty(sys:Int32)}""></x:Array>
        <x:Array x:Key=""Sample2"" Type=""{x:Type test:IConfigItemProperty(sys:Boolean)}""></x:Array>

        <x:Array x:Key=""CKSample"" Type=""{ck:Type test:OneObjectGen(sys:Int32)}""></x:Array>
        <x:Array x:Key=""CKSample1"" Type=""{ck:Type test:IConfigItemProperty(sys:Int32)}""></x:Array>
        <x:Array x:Key=""CKSample2"" Type=""{ck:Type test:IConfigItemProperty(sys:Boolean)}""></x:Array>

    </Window.Resources>

</Window>";

            var ctx = new XamlSchemaContext();
            using( var text = new StringReader( s ) )
            using( var reader = new XmlTextReader( text ) )
            {
                var w = (Window)System.Windows.Markup.XamlReader.Load( reader );

                Assert.That( w.Resources["Sample"], Is.InstanceOf<TestXaml.TestNamespace.OneObjectGen<int>[]>(), "It seems to work! But not in VS2010..." );
                Assert.That( w.Resources["Sample1"], Is.InstanceOf<TestXaml.TestNamespace.IConfigItemProperty<int>[]>(), "It seems to work! But not in VS2010..." );
                Assert.That( w.Resources["Sample2"], Is.InstanceOf<TestXaml.TestNamespace.IConfigItemProperty<bool>[]>(), "It seems to work! But not in VS2010..." );

                Assert.That( w.Resources["CKSample"], Is.InstanceOf<TestXaml.TestNamespace.OneObjectGen<int>[]>(), "CK.Windows.TypeExtension is just the same as standard TypeExtension... But it works." );
                Assert.That( w.Resources["CKSample1"], Is.InstanceOf<TestXaml.TestNamespace.IConfigItemProperty<int>[]>(), "CK.Windows.TypeExtension is just the same as standard TypeExtension... But it works." );
                Assert.That( w.Resources["CKSample2"], Is.InstanceOf<TestXaml.TestNamespace.IConfigItemProperty<bool>[]>(), "CK.Windows.TypeExtension is just the same as standard TypeExtension... But it works." );
            }

        }

    }

}
