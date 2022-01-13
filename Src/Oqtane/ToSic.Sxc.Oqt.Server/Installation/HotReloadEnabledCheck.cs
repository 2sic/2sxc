using Newtonsoft.Json.Linq;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ToSic.Sxc.Oqt.Shared.Models;

namespace ToSic.Sxc.Oqt.Server.Installation
{
    internal static class HotReloadEnabledCheck
    {
      
        private static bool? _hotReloadEnabledCheckedAndError;

        internal static OqtViewResultsDto InstallationErrorResult;

        internal static bool WarnIfHotReloadIsEnabled(out OqtViewResultsDto oqtViewResultsDto)
        {
            // Don't repeat if already checked
            if(_hotReloadEnabledCheckedAndError.HasValue)
            {
                oqtViewResultsDto = InstallationErrorResult;
                return _hotReloadEnabledCheckedAndError.Value;
            }
            
            // First time, run all checks
            var errorMessage = string.Empty;

            // Check if Hot Reload is Enabled.
            // When HotReloadEnabled is not false, special modules are loaded, so we try to find them.
            _hotReloadEnabledCheckedAndError = IsModuleLoaded("Microsoft.AspNetCore.Watch.BrowserRefresh.dll") || IsModuleLoaded("Microsoft.WebTools.BrowserLink.Net.dll");
            if (_hotReloadEnabledCheckedAndError.Value)
            {
                var hasHotReloadDisabled = DisableHotReload();

                errorMessage = "<strong>Warning:</strong> Hot Reload is enabled. There is issue with HotReload automatic browser refreshing that prevents 2sxc app templates to be installed. " +
                    (hasHotReloadDisabled 
                        ? "2sxc module just disabled HotReload in 'IIS Express' profile in 'Oqtane.Server\\Properties\\launchSettings.json'. "
                        : "Please check that property 'hotReloadEnabled':false exists in 'IIS Express' profile in 'Oqtane.Server\\Properties\\launchSettings.json'. ") +
                    "Ensure that 'IIS Express' profile is selected and run Oqtane.Server project again. " +
                    "More info: <a href=\"https://azing.org/2sxc/r/YUm957D-\" target=\"_new\">Disable Hot Reload for IIS Express profile of Oqtane.Server project in Visual Studio 2022</a>, " +
                    "<a href=\"https://azing.org/2sxc/r/VJEjD7bN\" target=\"_new\">Install 2sxc module in Oqtane Framework source code with Visual Studio 2022</a>.";

                InstallationErrorResult = new OqtViewResultsDto
                {
                    ErrorMessage = errorMessage,
                };
            }

            oqtViewResultsDto = InstallationErrorResult;
            
            return !string.IsNullOrEmpty(errorMessage);
        }

        private static bool IsModuleLoaded(string moduleName)
        {
            return Process.GetCurrentProcess().Modules.OfType<ProcessModule>().Any(m => m.ModuleName.Contains(moduleName));
        }

        private static bool DisableHotReload()
        {
            var launchSettingsFile = Path.Combine(Directory.GetCurrentDirectory(), $"Properties\\launchSettings.json");
            if (!File.Exists(launchSettingsFile)) return false;
            try
            {
                var launchSettingsJson = File.ReadAllText(launchSettingsFile);

                var launchSettings = JObject.Parse(launchSettingsJson);
                var profiles = (JObject)launchSettings["profiles"];
                var IISExpress = (JObject)profiles["IIS Express"];

                // if hotReloadEnabled property exists do nothing
                if (IISExpress.ContainsKey("hotReloadEnabled")) return false;

                // add hotReloadEnabled: true
                IISExpress.Property("environmentVariables").AddAfterSelf(new JProperty("hotReloadEnabled", false));

                File.WriteAllText(launchSettingsFile, launchSettings.ToString());

                return true;
            }
            catch
            {
                return false;
            }           
        }
    }
}
