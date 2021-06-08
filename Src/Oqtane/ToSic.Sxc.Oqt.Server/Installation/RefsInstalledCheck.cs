using System;
using System.IO;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Server.Installation
{
    internal static class RefsInstalledCheck
    {
        private static bool? _refsCheckedAndError;

        internal static OqtViewResultsDto InstallationErrorResult;

        internal static bool WarnIfRefsAreNotInstalled(out OqtViewResultsDto oqtViewResultsDto)
        {
            // Don't repeat if already checked
            if(_refsCheckedAndError.HasValue)
            {
                oqtViewResultsDto = InstallationErrorResult;
                return _refsCheckedAndError.Value;
            }
            
            // First time, run all checks
            var errorMessage = string.Empty;

            // Check for "refs" folder.
            // https://github.com/oqtane/oqtane.framework/issues/1272
            var dllLocation = AppContext.BaseDirectory;
            var refsPath = Path.Combine(dllLocation, "refs");
            _refsCheckedAndError = !Directory.Exists(refsPath);
            if (_refsCheckedAndError.Value)
            {
                errorMessage = "<strong>Warning:</strong> The \"refs\" folder is missing. Please ensure that <strong>Razor.Compiler.Dependencies.zip</strong> is unzipped as explained in the installation recipe <a href=\"https://azing.org/2sxc/r/fOG3aByY\" target=\"new\">https://azing.org/2sxc/r/fOG3aByY</a>.";
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
