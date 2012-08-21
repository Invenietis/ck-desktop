using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;

namespace CK.Windows.App
{
    /// <summary>
    /// Result of a Modal
    /// </summary>
    public enum ModalResult
    {
        Ok,
        Cancel,
        Retry,
        Yes,
        No
    }

    /// <summary>
    /// A button show on the modal
    /// When clicked on, sets its holder's ModalResult to the <see cref="ModalResult"> InnerValue </see> of the button
    /// </summary>
    public class ModalButton
    {
        public ModalViewModel Holder { get; private set; }
        public ModalResult InnerValue { get; private set; }
        public string Label { get; private set; }
        public Action Method { get; private set; }

        private void InnerMethod()
        {
            if( Method != null )
                Method.Invoke();

            Holder.ModalResult = InnerValue;
            Holder.CloseModal();
        }

        VMCommand _buttonClickCommand;
        public VMCommand ButtonClickCommand
        {
            get
            {
                if( _buttonClickCommand == null ) _buttonClickCommand = new VMCommand( InnerMethod );
                return _buttonClickCommand;
            }
        }

        public ModalButton( ModalViewModel holder, string label, Action method, ModalResult innerValue )
        {
            InnerValue = innerValue;
            Holder = holder;
            Method = method;
            Label = label;
        }
    }

    /// <summary>
    /// The ViewModel of the WPF CustomMsgBox
    /// Default ModalResult is Cancel, in case the user has clicked on the close button of the modal
    /// </summary>
    public class ModalViewModel
    {
        //The list of buttons to add to the modal
        public IList<ModalButton> Buttons { get; set; }

        //Checkbox configuration
        public bool IsCheckboxSelected { get; set; }
        public string CheckBoxLabel { get; set; }
        public bool ShowCheckBox { get; set; }

        //Result of the modal
        public ModalResult ModalResult { get; set; }

        public Window Holder { private get; set; }
        public string Title { get; private set; }
        public string Description { get; private set; }

        /// <summary>
        /// Constructor of the ViewModel used by the WPF <see cref="CustomMsgBox"/>.
        /// </summary>
        /// <param name="title">Title of the modal</param>
        /// <param name="description">Description of the modal</param>
        /// <param name="showCheckBox">Whether or not we should display a checkbox</param>
        /// <param name="checkBoxLabel">The label of the checkbox</param>
        public ModalViewModel( string title, string description, bool showCheckBox, string checkBoxLabel )
        {
            ModalResult = App.ModalResult.Cancel;
            CheckBoxLabel = checkBoxLabel;
            ShowCheckBox = showCheckBox;
            Description = description;
            Title = title;
            Buttons = new List<ModalButton>();
        }

        /// <summary>
        /// Constructor of the ViewModel used by the WPF <see cref="CustomMsgBox"/>.
        /// This constructor default behavior is not to show the checkbox.
        /// Use another constructor to configure the checkbox directly.
        /// </summary>
        /// <param name="title">Title of the modal</param>
        /// <param name="description">Description of the modal</param>
        public ModalViewModel( string title, string description )
            : this( title, description, false, "" )
        {
        }

        public void CloseModal()
        {
            if( Holder != null )
                Holder.Close();
        }
    }

    /// <summary>
    /// A command whose sole purpose is to 
    /// relay its functionality to other
    /// objects by invoking delegates. The
    /// default return value for the CanExecute
    /// method is 'true'.
    /// From http://mvvmfoundation.codeplex.com/ open source project.
    /// </summary>
    public class VMCommand : ICommand
    {
        #region Constructors

        /// <summary>
        /// Creates a new command that can always execute.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        public VMCommand( Action execute )
            : this( execute, null )
        {
        }

        /// <summary>
        /// Creates a new command.
        /// </summary>
        /// <param name="execute">The execution logic.</param>
        /// <param name="canExecute">The execution status logic.</param>
        public VMCommand( Action execute, Func<bool> canExecute )
        {
            if( execute == null )
                throw new ArgumentNullException( "execute" );

            _execute = execute;
            _canExecute = canExecute;
        }

        #endregion // Constructors

        #region ICommand Members

        [DebuggerStepThrough]
        public bool CanExecute( object parameter )
        {
            return _canExecute == null ? true : _canExecute();
        }

        public event EventHandler CanExecuteChanged
        {
            add
            {
                if( _canExecute != null )
                    CommandManager.RequerySuggested += value;
            }
            remove
            {
                if( _canExecute != null )
                    CommandManager.RequerySuggested -= value;
            }
        }

        public void Execute( object parameter )
        {
            _execute();
        }

        #endregion // ICommand Members

        #region Fields

        readonly Action _execute;
        readonly Func<bool> _canExecute;

        #endregion // Fields
    }
}
