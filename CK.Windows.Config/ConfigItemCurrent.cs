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

        public ConfigItemCurrent( ConfigManager configManager, object o, PropertyInfo current, Func<object> valueCollection, bool ensureCurrentNotNull, string noCurrentDisplayString )
            : this( configManager, new ValueProperty<T>( o, current ), valueCollection, o as INotifyPropertyChanged, ensureCurrentNotNull, noCurrentDisplayString )
        {
        }

        public ConfigItemCurrent( ConfigManager configManager, ValueProperty<T> current, Func<object> valueCollection, INotifyPropertyChanged monitorCurrent, bool ensureCurrentNotNull, string noCurrentDisplayString )
            : base( configManager )
        {
            _current = current;
            if ( ( _monitorCurrent = monitorCurrent ) != null )
            {
                _monitorCurrent.PropertyChanged += new PropertyChangedEventHandler( _monitorCurrent_PropertyChanged );
            }
            _sourceValues = valueCollection;
            _ensureCurrentNotNull = ensureCurrentNotNull;
            _noCurrentDisplayString = noCurrentDisplayString;
        }

        void _monitorCurrent_PropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            if ( e.PropertyName == _current.PropertyInfo.Name )
            {
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
                    _values.CurrentChanged += _values_CurrentChanged;
                    _values.CollectionChanged += new NotifyCollectionChangedEventHandler( _values_CollectionChanged );
                }
                return _values; 
            }
        }

        void _values_CollectionChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            //if current should be auto-set (_ensureCurrentNotNull == true),
            //that the current is null and that there is at least one element in the collectionView,
            //set the first element as current.
            if ( _ensureCurrentNotNull && _current.Get() == null && Values.SourceCollection.OfType<object>().ElementAtOrDefault( 0 ) != null )
                _current.Set( (T)Values.SourceCollection.OfType<object>().ElementAtOrDefault( 0 ) );

            this.Refresh();
        }

        public void ValuesRefresh( object o, EventArgs e )
        {
            Values.Refresh();
        }

        void _values_CurrentChanged( object sender, EventArgs e )
        {            
            _current.Set( (T)_values.CurrentItem );

            //When current is not auto-set and current is not null and there is only one element in the collectionView, (which means that the only element of the collection IS the current)
            //then the combobox isn't necesary anymore. Trigger PropertyChanged on ShowMultiple & ShowOne to have the combo replaced by a textblock
            if ( !_ensureCurrentNotNull && _current.Get() != null && !IsMoreThanOne)
            {
                NotifyOfPropertyChange( "ShowMultiple" );
                NotifyOfPropertyChange( "ShowOne" );
            }
        }
    }
}
