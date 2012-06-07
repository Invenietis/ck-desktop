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
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Windows;

namespace CK.Windows.Config
{
    public class ConfigItemCurrent<T> : ConfigItem, IConfigItemCurrent<T>
    {        
        ValueProperty<T> _current;
        INotifyPropertyChanged _monitorCurrent;
        Func<object> _sourceValues;
        ICollectionView _values;
        bool _ensureCurrentNotNull;
        string _noCurrentDisplayString;

        /// <summary>
        /// Initalizes a Wrapper that adds a "selected element" notion to an Observable collection set as sourceValue
        /// </summary>
        /// <param name="configManager"></param>
        /// <param name="o">The object holding the selected element property</param>
        /// <param name="current">PropertyInfo of the selected element property</param>
        /// <param name="valueCollection">Func that returns the ObservableCollection listing the values</param>
        /// <param name="ensureCurrentNotNull">set to true if you want the collection to set the current element if it is null and there are elements in the valueCollection</param>
        /// <param name="noCurrentDisplayString">Displayed string when the current element is null</param>
        public ConfigItemCurrent( ConfigManager configManager, object o, PropertyInfo current, Func<object> valueCollection, bool ensureCurrentNotNull, string noCurrentDisplayString )
            : this( configManager, new ValueProperty<T>( o, current ), valueCollection, o as INotifyPropertyChanged, ensureCurrentNotNull, noCurrentDisplayString )
        {
        }

        /// <summary>
        /// Initalizes a Wrapper that adds a "selected element" notion to an Observable collection set as sourceValue
        /// </summary>
        /// <param name="configManager"></param>
        /// <param name="current">ValueProperty of the selected element property</param>
        /// <param name="valueCollection">Func that returns the ObservableCollection listing the values</param>
        /// <param name="monitorCurrent">Instance of the selected element property's holder, must implement INotifyPropertChanged</param>
        /// <param name="ensureCurrentNotNull">set to true if you want the collection to set the current element if it is null and there are elements in the valueCollection</param>
        /// <param name="noCurrentDisplayString">Displayed string when the current element is null</param>
        public ConfigItemCurrent( ConfigManager configManager, ValueProperty<T> current, Func<object> valueCollection, INotifyPropertyChanged monitorCurrent, bool ensureCurrentNotNull, string noCurrentDisplayString )
            : base( configManager )
        {
            _current = current;
            if ( ( _monitorCurrent = monitorCurrent ) != null )
            {
                _monitorCurrent.PropertyChanged += new PropertyChangedEventHandler( OnHolderPropertyChanged );
            }
            _sourceValues = valueCollection;
            _ensureCurrentNotNull = ensureCurrentNotNull;
            _noCurrentDisplayString = noCurrentDisplayString;
        }

        void OnHolderPropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            if ( e.PropertyName == _current.PropertyInfo.Name )
            {
                //When the Holder's selected element property changes, refresh the collectionView's current element
                _values.MoveCurrentTo( _current.Get() );
            }
        }        

        public Visibility ShowMultiple { get { return IsMoreThanOne ? Visibility.Visible : Visibility.Collapsed; } }

        public Visibility ShowOne { get { return IsMoreThanOne ? Visibility.Collapsed : Visibility.Visible; } }

        public bool IsMoreThanOne
        {
            //if the current should not be auto-set (_ensureCurrentNotNull == false) 
            //and that there is only one item in the collectionView, we should display a combobox, 
            //so that the user can select it as current
            get { return (Values.SourceCollection.OfType<object>().ElementAtOrDefault( 1 ) != null 
                || (!_ensureCurrentNotNull && _current.Get() == null && Values.SourceCollection.OfType<object>().ElementAtOrDefault( 0 ) != null)); } 
        }

        public ICollectionView Values
        {
            get
            {
                if( _values == null )
                {
                    _values = CollectionViewSource.GetDefaultView( _sourceValues() );
                    _values.MoveCurrentTo( _current.Get() );

                    //Triggered when the user selects a new item in the combobox or when Values.Refresh() is called
                    _values.CurrentChanged += (s,e) => OnCurrentChanged();

                    //Triggerd by the underlying ObservableCollection or when Values.Refresh() is called
                    _values.CollectionChanged += (s,e) => OnCollectionChanged();
                }
                return _values; 
            }
        }

        void OnCollectionChanged()
        {
            //if current should be auto-set (_ensureCurrentNotNull == true),
            //that the current is null and that there is at least one element in the collectionView,
            //set the first element as current.
            if ( _ensureCurrentNotNull && _current.Get() == null && Values.SourceCollection.OfType<object>().ElementAtOrDefault( 0 ) != null )
                _current.Set( (T)Values.SourceCollection.OfType<object>().ElementAtOrDefault( 0 ) );

            this.Refresh();
        }

        void OnCurrentChanged()
        {
            _current.Set( (T)_values.CurrentItem );

            //When current is not auto-set and current is not null and there is only one element in the collectionView, (which means that the only element of the collection IS the current)
            //then the combobox isn't necessary anymore. Trigger PropertyChanged on ShowMultiple & ShowOne to have the combo replaced by a textblock
            if ( !_ensureCurrentNotNull && _current.Get() != null && !IsMoreThanOne )
            {
                NotifyOfPropertyChange( "ShowMultiple" );
                NotifyOfPropertyChange( "ShowOne" );
            }
        }

        //Should only be useful when the model does not implement INotifyPropertyChanged
        public void RefreshCurrent( object o, EventArgs e )
        {
            Values.MoveCurrentTo( _current.Get() );  
        }

        //Should be only be useful when the model does not implement INotifyPropertyChanged
        public void RefreshValues( object o, EventArgs e )
        {             
            Values.Refresh();         
        }
    }
}
