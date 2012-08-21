using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Caliburn.Micro;
using CK.Windows.Config;
using System.Windows;
using CK.Windows.App;

namespace CK.Windows.Demo
{
    internal class RootViewModel : ConfigPage
    {
        SubViewModel _subvm;
        public bool IsActive { get; set; }


        public RootViewModel( ConfigManager configManager )
            : base( configManager )
        {
            DisplayName = "Root view";

            this.AddAction( "Show a popup", () => MessageBox.Show( "Pow!" ) );
            this.AddAction( "Show another popup", () => MessageBox.Show( "Another Pow!" ) );
            this.AddAction( "Show a custom modal popup", ShowCustomMessageBox );
            this.AddLink( _subvm ?? ( _subvm = new SubViewModel( configManager ) ) );
        }

        /// <summary>
        /// Shows how to use the WPF CustomMsgBox
        /// </summary>
        internal void ShowCustomMessageBox()
        {
            ModalViewModel modalDataContext = new ModalViewModel( "Mise à jour disponible", "Une mise à jour est disponible, voulez-vous l'installer ? \r\n Mon message est super long, il faut que la textbox sur wrap correctemenet pour eviter de donner une modale trop longue", false, "Ne plus me le rappeler" );

            IList<ModalButton> dic = new List<ModalButton>();
            dic.Add( new ModalButton( modalDataContext, "OK", null, ModalResult.Ok ) );
            dic.Add( new ModalButton( modalDataContext, "Cancel", () => Console.Out.WriteLine( "Testing Cancel" ), ModalResult.Cancel ) );
            modalDataContext.Buttons = dic;

            CustomMsgBox b = new CustomMsgBox( ref modalDataContext );
            b.ShowDialog();

            Console.Out.WriteLine( String.Format( "resultat : {0}", modalDataContext.ModalResult + " \r\n checkbox checked : " + modalDataContext.IsCheckboxSelected ) );
        }
    }
}
