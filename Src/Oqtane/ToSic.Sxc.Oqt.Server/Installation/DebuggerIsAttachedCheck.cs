using System.Diagnostics;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Server.Installation
{
    // TODO: STV - WIP enhance development expirience in VS2022 + Oqtane 3.0 Source Code DEV ENV  
    internal static class DebuggerIsAttachedCheck
    {
      
        private static bool? _debuggerIsAttachedCheckedAndError;

        internal static OqtViewResultsDto InstallationErrorResult;

        internal static bool WarnIfDebuggerIsAttached(out OqtViewResultsDto oqtViewResultsDto)
        {
            // Don't repeat if already checked
            if(_debuggerIsAttachedCheckedAndError.HasValue)
            {
                oqtViewResultsDto = InstallationErrorResult;
                return _debuggerIsAttachedCheckedAndError.Value;
            }
            
            // First time, run all checks
            var errorMessage = string.Empty;

            // Check if debugger is attached.
            _debuggerIsAttachedCheckedAndError = Debugger.IsAttached; 
            if (_debuggerIsAttachedCheckedAndError.Value)
            {
                errorMessage = "<strong>Warning:</strong> Debugger is attached. There is issue with HotReload that prevents 2sxc app templates to be installed. " +
                    "Please <a href=\"https://azing.org/2sxc/r/YUm957D-\" target=\"_new\">Disable Hot Reload in Visual Studio 2022</a>. " +
                    "You can find more info in <a href=\"https://azing.org/2sxc/r/VJEjD7bN\" target=\"_new\">Install 2sxc module in Oqtane Framework 3.0.0 source code with Visual Studio 2022</a>.";

                InstallationErrorResult = new OqtViewResultsDto
                {
                    ErrorMessage = errorMessage,
                };
            }

            oqtViewResultsDto = InstallationErrorResult;
            
            return !string.IsNullOrEmpty(errorMessage);
        }
    }
}
