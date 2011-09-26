using System;
using System.ComponentModel;

namespace CK.Windows.Config
{
    public interface IConfigItemProperty<T> : IConfigItem, INotifyPropertyChanged
    {
        T Value { get; set; }
    }
}
