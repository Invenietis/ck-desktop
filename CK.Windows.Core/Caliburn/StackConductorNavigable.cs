using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Windows.Input;
using CK.Core;

namespace Caliburn.Micro
{
    /// <summary>
    /// A Conductor that is a collection of views. enable going back to the stacked views.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class NavigableConductor<T> : StackConductor<T>
        where T : class, IWizardNavigable
    {
        /// <summary>
        /// Ctor
        /// </summary>
        public NavigableConductor()
            : base()
        {
        }

        /// <summary>
        /// Gets the Next item of the currently active item
        /// </summary>
        public T Next
        {
            get { return (T)ActiveItem.Next; }
        }

        /// <summary>
        /// Activates the previous view. 
        /// </summary>
        public new void GoBack()
        {
            if( ActiveItem.CheckCanGoBack() && ActiveItem.OnBeforeGoBack() )
            {
                if( CanGoBack() && OnBeforeGoBack() )
                {
                    if( Previous.OnActivating() )
                    {
                        ActivateItem( Previous );
                        ActiveItem.OnActivated();
                    }
                }
            }
        }

        /// <summary>
        /// Activates the next view
        /// </summary>
        public void GoFurther()
        {
            if( ActiveItem.CheckCanGoFurther() && ActiveItem.OnBeforeNext() )
            {
                if( CanGoFuther() && OnBeforeGoFuther() )
                {
                    if( Next.OnActivating() )
                    {
                        ActivateItem( Next );
                        ActiveItem.OnActivated();
                        return;
                    }
                }
            }
        }

        /// <summary>
        /// When overridden in a inherited class, tests whether we should actually go to the previous view.
        /// Can be used to do things before going back.
        /// </summary>
        /// <returns></returns>
        public override bool OnBeforeGoBack()
        {
            return true;
        }

        /// <summary>
        /// When overridden in a inherited class, tests whether we should actually go to the next view.
        /// Can be used to do things before going next.
        /// </summary>
        /// <returns></returns>
        public bool OnBeforeGoFuther()
        {
            return true;
        }

        /// <summary>
        /// Test if the Next view exists
        /// </summary>
        /// <returns></returns>
        public bool CanGoFuther()
        {
            return Next != null;
        }

        /// <summary>
        /// Goes directly back to the root page, popping all the stack of pages until the first one.
        /// </summary>
        public new void GoBackToRoot()
        {
            while( CanGoBack() )
            {
                Pop();
            }

            if( Stack.Count == 1 )
            {
                if( Stack[0].OnActivating() )
                {
                    ActivateItem( Stack[0] );
                    Stack[0].OnActivated();
                }
            }
        }

        class GoBackCmd : ICommand
        {
            NavigableConductor<T> _c;

            public GoBackCmd( NavigableConductor<T> c )
            {
                _c = c;
            }

            public bool CanExecute( object parameter )
            {
                return _c.CanGoBack() && _c.ActiveItem.CheckCanGoBack();
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
                if( cmdRef != null && (cmd = cmdRef.Target) != null ) cmd.RaiseCanExecuteChanged();
            }

            static public GoBackCmd Ensure( NavigableConductor<T> holder, ref WeakReference<GoBackCmd> cmdRef )
            {
                GoBackCmd cmd;
                if( cmdRef == null ) cmdRef = cmd = new GoBackCmd( holder );
                else if( (cmd = cmdRef.Target) == null ) cmdRef.Target = cmd = new GoBackCmd( holder );
                return cmd;
            }
        }

        WeakReference<GoBackCmd> _goBackCommand;

        public new ICommand GoBackCommand { get { return GoBackCmd.Ensure( this, ref _goBackCommand ); } }

        class GoFurtherCmd : ICommand
        {
            NavigableConductor<T> _c;

            public GoFurtherCmd( NavigableConductor<T> c )
            {
                _c = c;
            }

            public bool CanExecute( object parameter )
            {
                return _c.CanGoFuther() && _c.ActiveItem.CheckCanGoFurther();
            }

            public void Execute( object parameter )
            {
                _c.GoFurther();
            }

            public event EventHandler CanExecuteChanged;

            public void RaiseCanExecuteChanged()
            {
                var h = CanExecuteChanged;
                if( h != null ) h( this, EventArgs.Empty );
            }

            static public void RaiseCanExecuteChanged( WeakReference<GoFurtherCmd> cmdRef )
            {
                GoFurtherCmd cmd;
                if( cmdRef != null && (cmd = cmdRef.Target) != null ) cmd.RaiseCanExecuteChanged();
            }

            static public GoFurtherCmd Ensure( NavigableConductor<T> holder, ref WeakReference<GoFurtherCmd> cmdRef )
            {
                GoFurtherCmd cmd;
                if( cmdRef == null ) cmdRef = cmd = new GoFurtherCmd( holder );
                else if( (cmd = cmdRef.Target) == null ) cmdRef.Target = cmd = new GoFurtherCmd( holder );
                return cmd;
            }
        }

        WeakReference<GoFurtherCmd> _goFurtherCommand;

        public ICommand GoFurtherCommand { get { return GoFurtherCmd.Ensure( this, ref _goFurtherCommand ); } }

    }
}
