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
