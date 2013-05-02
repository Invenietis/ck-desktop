using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CK.Windows.Demo.DemoVms
{
    class PseudoSendStringService
    {

        public static void SendString( string text )
        {
            Console.WriteLine( "Sending string: {0}", text );
            System.Windows.Forms.SendKeys.SendWait( text );
        }

    }
}
