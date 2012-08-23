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

        //Icon configuration
        public CustomMsgBoxIcon Icon { get; set; }

        #region Checkbox configuration
        
        public bool IsCheckboxSelected { get; set; }
        public string CheckBoxLabel { get; set; }
        public bool ShowCheckBox { get; set; }
        
        #endregion

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
        public ModalViewModel( string title, string description, bool showCheckBox, string checkBoxLabel, CustomMsgBoxIcon messageBoxIcon )
        {
            Icon = messageBoxIcon;
            ModalResult = App.ModalResult.Cancel;
            CheckBoxLabel = checkBoxLabel;
            ShowCheckBox = showCheckBox;
            Description = description;
            Title = title;
            Buttons = new List<ModalButton>();
        }

        /// <summary>
        /// Constructor of the ViewModel used by the WPF <see cref="CustomMsgBox"/>.
        /// </summary>
        /// <param name="title">Title of the modal</param>
        /// <param name="description">Description of the modal</param>
        /// <param name="showCheckBox">Whether or not we should display a checkbox</param>
        /// <param name="checkBoxLabel">The label of the checkbox</param>
        public ModalViewModel( string title, string description, bool showCheckBox, string checkBoxLabel )
            : this( title, description, showCheckBox, checkBoxLabel, CustomMsgBoxIcon.Information )
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
