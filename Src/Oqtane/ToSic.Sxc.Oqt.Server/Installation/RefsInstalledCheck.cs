using System;
using System.IO;
using System.Reflection;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Server.Installation;

internal static class RefsInstalledCheck
{
    private static bool? _refsCheckedAndError;

    internal static OqtViewResultsDto InstallationErrorResult;

    const string MicrosoftAspNetCoreDll = "Microsoft.AspNetCore.dll";

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
            if (Environment.Version.Major >= 5 & Environment.Version.Major < 6) // missing refs for Oqtane 2.0.0 on .NET5.0
                errorMessage = "<strong>Warning:</strong> The \"refs\" folder is missing. Please ensure that <strong><a href=\"https://github.com/2sic/oqtane-razor-refs/releases/download/1.0.0/net5.0-refs.zip\">net5.0-refs.zip</a></strong> is unzipped as explained in the recipe <a href=\"https://azing.org/2sxc/r/P0_y3L4a\" target=\"new\">Install refs for Oqtane 2.0.0 on .NET5.0</a>.";
            else if (Environment.Version.Major >= 6) // missing refs for Oqtane 3.0.0 on .NET6.0
                errorMessage = "<strong>Warning:</strong> The \"refs\" folder is missing. Please ensure that <strong><a href=\"https://github.com/2sic/oqtane-razor-refs/releases/download/1.0.0/net6.0-refs.zip\">net6.0-refs.zip</a></strong> is unzipped as explained in the recipe <a href=\"https://azing.org/2sxc/r/nQjqJ_Wr\" target=\"new\">Install refs for Oqtane 3.0.0 on .NET6.0</a>.";
            else
                errorMessage = "<strong>Warning:</strong> The \"refs\" folder is missing. Please ensure that <strong>Razor.Compiler.Dependencies.zip</strong> is unzipped as explained in the installation recipe <a href=\"https://azing.org/2sxc/r/fOG3aByY\" target=\"new\">https://azing.org/2sxc/r/fOG3aByY</a>.";
        }
        else
        {
            // When we have refs, we still need to check is it right version. It is possible that Oqtane was upgraded to newer version of run-time and that current refs are outdated.

            // Microsoft.AspNetCore.dll will be used as significant reference assembly to check 'refs' version.
            var referenceAssemblyPath = Path.Combine(refsPath, MicrosoftAspNetCoreDll);

            _refsCheckedAndError = !File.Exists(referenceAssemblyPath);
            if (_refsCheckedAndError.Value) // Microsoft.AspNetCore.dll is missing from 'refs' folder
                errorMessage = "<strong>Warning:</strong> The \"refs\" have missing Reference assemblies. Please ensure that all dlls from <strong>refs.zip</a></strong> are unzipped directly in 'refs' folder. You can get refs.zip from <a href=\"https://github.com/2sic/oqtane-razor-refs/releases\" target =\"new\">oqtane-razor-refs</a>.";
            else
            {
                // Is the 'refs' version older than current .NET run-time version?
                var referenceAssemblyName = AssemblyName.GetAssemblyName(referenceAssemblyPath);
                _refsCheckedAndError = referenceAssemblyName.Version?.Major < Environment.Version.Major;
                if (_refsCheckedAndError.Value)
                    errorMessage = $"<strong>Warning:</strong> The Reference assemblies version {referenceAssemblyName.Version} in \"refs\" folder is older than current .NET run-time version {Environment.Version}. Please delete local \"refs\" folder and ensure that right <strong><a href=\"https://github.com/2sic/oqtane-razor-refs/releases/download/1.0.0/net6.0-refs.zip\">net6.0-refs.zip</a></strong> is unzipped as explained in the recipe <a href=\"https://azing.org/2sxc/r/nQjqJ_Wr\" target=\"new\">Install refs for Oqtane 3.0.0 on .NET6.0</a>.";
            }
        }

        // if we have any error, set InstallationErrorResult
        if (_refsCheckedAndError.Value)
        {
            InstallationErrorResult = new()
            {
                ErrorMessage = errorMessage,
            };
        }

        oqtViewResultsDto = InstallationErrorResult;

        return _refsCheckedAndError.Value;
    }
}