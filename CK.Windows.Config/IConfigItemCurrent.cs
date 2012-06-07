using System;
using System.ComponentModel;

namespace CK.Windows.Config
{

    public interface IConfigItemCurrent<T>
    {
        ICollectionView Values { get; }
    }

}
