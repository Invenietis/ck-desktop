#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.Core\Caliburn\StackConductor.cs) is part of CiviKey. 
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

using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Windows.Input;
using System;
using CK.Core;

namespace Caliburn.Micro
{
    /// <summary>
    /// A Conductor that is a collection of views. enable going back to the stacked views.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class StackConductor<T> : Conductor<T>.Collection.OneActive
        where T : class
    {
        List<T> _stack;

        /// <summary>
        /// Ctor
        /// </summary>
        public StackConductor()
        {
            _stack = new List<T>();
            Items.CollectionChanged += ( o, e ) =>
            {
                if( e.Action == NotifyCollectionChangedAction.Remove || e.Action == NotifyCollectionChangedAction.Reset )
                {
                    _stack.RemoveAll( p => !Items.Contains( p ) );
                }
                else if( e.Action == NotifyCollectionChangedAction.Replace )
                {
                    for( int i = 0; i < e.OldItems.Count; ++i )
                    {
                        var p = Items[i];
                        int indexInStack = _stack.IndexOf( p );
                        if( indexInStack >= 0 ) _stack[i] = (T)e.NewItems[i];
                    }
                }
            };
        }

        /// <summary>
        /// Gets the Previous item in the view stack
        /// </summary>
        public T Previous
        {
            get { return _stack.Count > 1 ? _stack[_stack.Count - 2] : null; }
        }

        /// <summary>
        /// Activates the previous view
        /// </summary>
        public void GoBack()
        {
            if( CanGoBack() && OnBeforeGoBack() ) ActivateItem( Previous );
        }

        /// <summary>
        /// When overridden in a inherited class, tests whether we should actually go to the previous view.
        /// Can be used to do things before going back.
        /// </summary>
        /// <returns></returns>
        public virtual bool OnBeforeGoBack()
        {
            return true;
        }

        /// <summary>
        /// Gets whether there is a "previous view", to which going back is possible
        /// </summary>
        /// <returns></returns>
        public bool CanGoBack()
        {
            return _stack.Count > 1;
        }

        /// <summary>
        /// Goes directly back to the root page, popping all the stack of pages until the first one.
        /// </summary>
        public void GoBackToRoot()
        {
            while( CanGoBack() )
            {
                Pop();
            }

            if( _stack.Count == 1 )
                ActivateItem( _stack[0] );
        }

        class GoBackCmd : ICommand
        {
            StackConductor<T> _c;

            public GoBackCmd( StackConductor<T> c )
            {
                _c = c;
            }

            public bool CanExecute( object parameter )
            {
                return _c.CanGoBack();
            }

            public void Execute( object parameter )
            {
                _c.GoBack();
            }

            public event EventHandler CanExecuteChanged;

            public void RaiseCanExecuteChanged()
            {
                var h = CanExecuteChanged;
                if( h != null ) h( this, EventArgs.Empty );
            }

            static public void RaiseCanExecuteChanged( WeakReference<GoBackCmd> cmdRef )
            {
                GoBackCmd cmd;
                if( cmdRef != null && ( cmd = cmdRef.Target ) != null ) cmd.RaiseCanExecuteChanged();
            }

            static public GoBackCmd Ensure( StackConductor<T> holder, ref WeakReference<GoBackCmd> cmdRef )
            {
                GoBackCmd cmd;
                if( cmdRef == null ) cmdRef = cmd = new GoBackCmd( holder );
                else if( ( cmd = cmdRef.Target ) == null ) cmdRef.Target = cmd = new GoBackCmd( holder );
                return cmd;
            }

        }

        WeakReference<GoBackCmd> _goBackCommand;

        public ICommand GoBackCommand { get { return GoBackCmd.Ensure( this, ref _goBackCommand ); } }

        protected override T DetermineNextItemToActivate( IList<T> list, int lastIndex )
        {
            // If the item is the one on top of the stack, we
            // use the stack. Otherwise, we fall back to the default "by index" version.
            if( _stack.Count > 0
                && lastIndex >= 0 && lastIndex < list.Count
                && list[lastIndex] == _stack[_stack.Count - 1] )
            {
                return _stack[_stack.Count - 2];
            }
            return base.DetermineNextItemToActivate( list, lastIndex );
        }

        protected override void OnActivationProcessed( T item, bool success )
        {
            base.OnActivationProcessed( item, success );
            if( success )
            {
                // If we are the first page: push it.
                if( _stack.Count == 0 ) Push( item );
                else
                {
                    // If we are activating the same item twice, just 
                    // forget it.
                    if( _stack[_stack.Count - 1] != item )
                    {
                        // If we have at least 2 items in the stack, we check 
                        // that we are not activating the previous one and if it
                        // is the case, we pop the stack instead of pushing the new item.
                        // This implements... the "back" and secures stupid gotos 
                        // to the previous page (ie. "Goto previous" is the same as "Back").
                        if( _stack.Count > 1 && _stack[_stack.Count - 2] == item ) Pop();
                        else Push( item );
                    }
                }
            }
        }

        private void Push( T item )
        {
            Debug.Assert( _stack.Count == 0 || _stack[_stack.Count - 1] != item );
            _stack.Add( item );
            NotifyOfPropertyChange( "Previous" );
            if( _stack.Count == 2 ) GoBackCmd.RaiseCanExecuteChanged( _goBackCommand );
        }

        private void Pop()
        {
            Debug.Assert( _stack.Count > 1, "We never pop the root page." );
            _stack.RemoveAt( _stack.Count - 1 );
            NotifyOfPropertyChange( "Previous" );
            if( _stack.Count == 1 ) GoBackCmd.RaiseCanExecuteChanged( _goBackCommand );
        }
    }

}
