#region LGPL License
/*----------------------------------------------------------------------------
* This file (CK.Windows.App\MsgBox\ModalViewModel.cs) is part of CiviKey. 
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
using System.Diagnostics;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows;

namespace CK.Windows.App
{
    /// <summary>
    /// Result of a Modal. 
    /// This result is to be used after closing the Modal.
    /// It only is a localization-free value to assign to a <see cref="CK.Windows.App.ModalButton"/>
    /// </summary>
    public enum ModalResult
    {
        /// <summary>
        /// Ok
        /// </summary>
        Ok,
        /// <summary>
        /// Cancel
        /// </summary>
        Cancel,
        /// <summary>
        /// Retry
        /// </summary>
        Retry,
        /// <summary>
        /// Yes
        /// </summary>
        Yes,
        /// <summary>
        /// No
        /// </summary>
        No
    }

    /// <summary>
    /// A button show on the modal
    /// When clicked on, sets its holder's ModalResult to the <see cref="ModalResult"> InnerValue </see> of the button
    /// </summary>
    public class ModalButton
    {
        /// <summary>
        /// Gets the <see cref="CK.Windows.App.ModalViewModel"/> which contains this button.
        /// </summary>
        public ModalViewModel Holder { get; private set; }

        /// <summary>
        /// Gets the value returned to the <see cref="CK.Windows.App.ModalViewModel"/> when the user chooses this button
        /// </summary>
        public ModalResult InnerValue { get; private set; }

        /// <summary>
        /// Gets what will be written on the button
        /// </summary>
        public string Label { get; private set; }

        /// <summary>
        /// The action when the user clicks on the button - can be null
        /// Clicking this button will close the dialog and return its value to the <see cref="CK.Windows.App.ModalViewModel"/> anyway.
        /// </summary>
        public Action Method { get; private set; }

        private void InnerMethod()
        {
            if( Method != null )
                Method.Invoke();

            Holder.ModalResult = InnerValue;
            Holder.CloseModal();
        }

        VMCommand _buttonClickCommand;

        /// <summary>
        /// Gets the Command that sets the <see cref="CK.Windows.App.ModalViewModel"/> to the InnerValue of this button, launches the Method if there is any and then closes the modal
        /// </summary>
        public VMCommand ButtonClickCommand
        {
            get
            {
                if( _buttonClickCommand == null ) _buttonClickCommand = new VMCommand( InnerMethod );
                return _buttonClickCommand;
            }
        }

        /// <summary>
        /// Constructor of a Button that should be added to a <see cref="CK.Windows.App.ModalViewModel"/> to specifiy the button to show on the Modal window
        /// </summary>
        /// <param name="holder">The <see cref="CK.Windows.App.ModalViewModel"/> which contains this button</param>
        /// <param name="label">What will be written on the button</param>
        /// <param name="method">The action when the user clicks on the button - can be null</param>
        /// <param name="innerValue">The value returned to the <see cref="CK.Windows.App.ModalViewModel"/> when the user chooses this button</param>
        public ModalButton( ModalViewModel holder, string label, Action method, ModalResult innerValue )
        {
            InnerValue = innerValue;
            Holder = holder;
            Method = method;
            Label = label;
        }

        /// <summary>
        /// Constructor of a Button that should be added to a <see cref="CK.Windows.App.ModalViewModel"/> to specifiy the button to show on the Modal window
        /// </summary>
        /// <param name="holder">The <see cref="CK.Windows.App.ModalViewModel"/> which contains this button</param>
        /// <param name="label">What will be written on the button</param>
        /// <param name="innerValue">The value returned to the <see cref="CK.Windows.App.ModalViewModel"/> when the user chooses this button</param>
        public ModalButton( ModalViewModel holder, string label, ModalResult innerValue )
            :this(holder, label, null, innerValue)
        {
        }
    }

    /// <summary>
    /// The ViewModel of the WPF CustomMsgBox
    /// Default ModalResult is Cancel, in case the user has clicked on the close button of the modal
    /// </summary>
    public class ModalViewModel
    {
        /// <summary>
        /// The list of buttons to add to the modal
        /// </summary>
        public IList<ModalButton> Buttons { get; set; }

        /// <summary>
        /// The icon shown on the modal
        /// </summary>
        public CustomMsgBoxIcon Icon { get; set; }

        #region Checkbox configuration

        /// <summary>
        /// Gets or sets whether the checkbox of the modal is checked
        /// </summary>
        public bool IsCheckboxChecked { get; set; }

        /// <summary>
        /// Gets the label of the checkbox displayed in the modal
        /// </summary>
        public string CheckBoxLabel { get; private set; }

        /// <summary>
        /// Gets or sets whether the checkbox is to be displayed in the modal
        /// </summary>
        public bool ShowCheckBox { get; set; }

        #endregion

        /// <summary>
        /// Gets or sets the index of the button that has focus at launch.
        /// If not set, the first button of the list will be set as default.
        /// </summary>
        public int FocusedButtonIndex { get; set; }

        /// <summary>
        /// Result of the modal, returns the innerValue of the clicked button (returns <see cref="CK.Windows.App.ModalResult.Cancel"/> if the close button has been pressed)
        /// </summary>
        public ModalResult ModalResult { get; set; }

        /// <summary>
        /// Gets or sets the Window holding this viewmodel.
        /// </summary>
        internal Window Holder { private get; set; }

        /// <summary>
        /// Gets the title of the modal
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// Gets the description of the modal
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// Constructor of the ViewModel used by the WPF <see cref="CustomMsgBox"/>.
        /// </summary>
        /// <param name="title">Title of the modal</param>
        /// <param name="description">Description of the modal</param>
        /// <param name="showCheckBox">Whether or not we should display a checkbox</param>
        /// <param name="checkBoxLabel">The label of the checkbox</param>
        /// <param name="messageBoxIcon">the icon of the message box, from the <see cref="CustomMsgBoxIcon"/> enum</param>
        /// <param name="focusedButtonIndex">Index of the button that will have keyboard focus</param>int focusedButtonIndex, 
        public ModalViewModel( string title, string description, bool showCheckBox, string checkBoxLabel, CustomMsgBoxIcon messageBoxIcon, int focusedButtonIndex )
        {
            FocusedButtonIndex = focusedButtonIndex;
            ModalResult = App.ModalResult.Cancel;
            Buttons = new List<ModalButton>();
            CheckBoxLabel = checkBoxLabel;
            ShowCheckBox = showCheckBox;
            Description = description;
            Icon = messageBoxIcon;
            Title = title;
        }

        /// <summary>
        /// Constructor of the ViewModel used by the WPF <see cref="CustomMsgBox"/>.
        /// </summary>
        /// <param name="title">Title of the modal</param>
        /// <param name="description">Description of the modal</param>
        /// <param name="showCheckBox">Whether or not we should display a checkbox</param>
        /// <param name="checkBoxLabel">The label of the checkbox</param>
        /// <param name="messageBoxIcon">the icon of the message box, from the <see cref="CustomMsgBoxIcon"/> enum</param>
        public ModalViewModel( string title, string description, bool showCheckBox, string checkBoxLabel, CustomMsgBoxIcon messageBoxIcon )
            : this( title, description, showCheckBox, checkBoxLabel, messageBoxIcon, 0 )
        {
        }

        /// <summary>
        /// Constructor of the ViewModel used by the WPF <see cref="CustomMsgBox"/>.
        /// </summary>
        /// <param name="title">Title of the modal</param>
        /// <param name="description">Description of the modal</param>
        /// <param name="showCheckBox">Whether or not we should display a checkbox</param>
        /// <param name="checkBoxLabel">The label of the checkbox</param>
        /// <param name="focusedButtonIndex">Index of the button that will have keyboard focus</param>
        public ModalViewModel( string title, string description, bool showCheckBox, string checkBoxLabel, int focusedButtonIndex )
            : this( title, description, showCheckBox, checkBoxLabel, CustomMsgBoxIcon.Information, focusedButtonIndex )
        {
        }

        /// <summary>
        /// Constructor of the ViewModel used by the WPF <see cref="CustomMsgBox"/>.
        /// </summary>
        /// <param name="title">Title of the modal</param>
        /// <param name="description">Description of the modal</param>
        /// <param name="showCheckBox">Whether or not we should display a checkbox</param>
        /// <param name="checkBoxLabel">The label of the checkbox</param>
        public ModalViewModel( string title, string description, bool showCheckBox, string checkBoxLabel )
            : this( title, description, showCheckBox, checkBoxLabel, CustomMsgBoxIcon.Information, 0 )
        {
        }

        /// <summary>
        /// Constructor of the ViewModel used by the WPF <see cref="CustomMsgBox"/>.
        /// </summary>
        /// <param name="title">Title of the modal</param>
        /// <param name="description">Description of the modal</param>
        /// <param name="messageBoxIcon">the icon of the message box, from the <see cref="CustomMsgBoxIcon"/> enum</param>
        /// /// <param name="focusedButtonIndex">Index of the button that will have keyboard focus</param>
        public ModalViewModel( string title, string description, CustomMsgBoxIcon messageBoxIcon, int focusedButtonIndex )
            : this( title, description, false, "", messageBoxIcon, focusedButtonIndex )
        {
        }

        /// <summary>
        /// Constructor of the ViewModel used by the WPF <see cref="CustomMsgBox"/>.
        /// </summary>
        /// <param name="title">Title of the modal</param>
        /// <param name="description">Description of the modal</param>
        /// <param name="messageBoxIcon">the icon of the message box, from the <see cref="CustomMsgBoxIcon"/> enum</param>
        public ModalViewModel( string title, string description, CustomMsgBoxIcon messageBoxIcon )
            : this( title, description, false, "", messageBoxIcon, 0 )
        {
        }

        /// <summary>
        /// Constructor of the ViewModel used by the WPF <see cref="CustomMsgBox"/>.
        /// This constructor default behavior is not to show the checkbox.
        /// Use another constructor to configure the checkbox directly.
        /// </summary>
        /// <param name="title">Title of the modal</param>
        /// <param name="description">Description of the modal</param>
        /// <param name="focusedButtonIndex">Index of the button that will have keyboard focus</param>
        public ModalViewModel( string title, string description, int focusedButtonIndex )
            : this( title, description, false, "", focusedButtonIndex )
        {
        }

        /// <summary>
        /// Constructor of the ViewModel used by the WPF <see cref="CustomMsgBox"/>.
        /// This constructor default behavior is not to show the checkbox.
        /// Use another constructor to configure the checkbox directly.
        /// </summary>
        /// <param name="title">Title of the modal</param>
        /// <param name="description">Description of the modal</param>
        public ModalViewModel( string title, string description )
            : this( title, description, false, "", 0 )
        {
        }

        /// <summary>
        /// Closes the window set has holder of this viewmodel
        /// </summary>
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
