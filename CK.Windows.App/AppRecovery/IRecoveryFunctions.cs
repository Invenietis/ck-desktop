using System;
using System.Runtime.InteropServices;
using CK.Interop;

namespace CK.Windows.App
{
    /// <summary>
    /// Native mapping for <see cref="AppRecoveryManager"/>.
    /// </summary>
    [NativeDll( DefaultDllNameGeneric = "kernel32.dll" )]
    public interface IRecoveryFunctions
    {
        [CK.Interop.DllImport( CharSet = CharSet.Auto )]
        uint RegisterApplicationRestart( string pwsCommandLine, AppRecoveryManager.RestartFlags dwFlags );

        [CK.Interop.DllImport]
        uint RegisterApplicationRecoveryCallback( IntPtr pRecoveryCallback, IntPtr pvParameter, int dwPingInterval, int dwFlags );

        [CK.Interop.DllImport]
        uint ApplicationRecoveryInProgress( out bool pbCancelled );

        [CK.Interop.DllImport]
        uint ApplicationRecoveryFinished( bool bSuccess );
    }
}
