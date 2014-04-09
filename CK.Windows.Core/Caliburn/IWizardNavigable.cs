using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace Caliburn.Micro
{
    public interface IWizardNavigable
    {
        /// <summary>
        /// Raised before activated the IWizardNavigable. Can be canceled.
        /// </summary>
        event CancelEventHandler Activating;

        /// <summary>
        /// Raised when the IWizardNavigable is the current active Wizard page.
        /// </summary>
        event EventHandler<WizardNavigableEventArgs> Activated;

        /// <summary>
        /// The next wizard page, which will be activate when the user clicks on next.
        /// </summary>
        IWizardNavigable Next { get; }

        /// <summary>
        /// Gets whether the user can go on the next Wizard page
        /// </summary>
        bool CanGoFurther { get; }

        /// <summary>
        /// Gets whether the user can go on the previous Wizard page
        /// </summary>
        bool CanGoBack { get; }

        /// <summary>
        /// Is called before going to the next step of the wizard.
        /// Can be overridden to implement the test corresponding to the situation.
        /// </summary>
        /// <returns>True if the user can be shown the next step</returns>
        bool CheckCanGoFurther();

        /// <summary>
        /// Is called before going to the previous step of the wizard.
        /// Can be overridden to implement the test corresponding to the situation.
        /// </summary>
        /// <returns>True if the user can go back to the previous step</returns>
        bool CheckCanGoBack();

        /// <summary>
        /// Is called right before going ot the next step, but after checking CanGoFurther.
        /// Can be overridden to do something before changing the ActiveItem.
        /// Return false to cancel going ot the next WizardPage
        /// </summary>
        /// <returns>true if the manager should go on changing the view, false otherwise</returns>
        bool OnBeforeNext();

        /// <summary>
        /// Is called right before going ot the previous step, but after checking CanGoBack.
        /// Can be overridden to do something before changing the ActiveItem.
        /// Return false to cancel going ot the previous WizardPage 
        /// </summary>
        /// <returns>true if the manager should go on changing the view, false otherwise</returns>
        bool OnBeforeGoBack();

        /// <summary>
        /// Is called before ActivateItem function.
        /// </summary>
        /// <returns>Return false, if the Activating event has been canceled.</returns>
        bool OnActivating();

        /// <summary>
        /// Is called after ActivateItem function.
        /// </summary>
        void OnActivated();
    }
}
