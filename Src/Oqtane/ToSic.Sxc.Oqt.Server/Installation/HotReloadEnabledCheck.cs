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

        private static string errorMessage = "Warning: You must run Oqtane without Hot-Reload to install Apps. See https://r.2sxc.org/oqt-hr";

        internal static OqtViewResultsDto InstallationErrorResult;

        internal static void Check()
        {
            // Don't repeat if already checked
            if (_hotReloadEnabledCheckedAndError.HasValue)
                if (_hotReloadEnabledCheckedAndError.Value)
                    ThrowError();
                else
                    return;

            // Check if Hot Reload is Enabled.
            // When HotReloadEnabled is not false, special modules are loaded, so we try to find them.
            _hotReloadEnabledCheckedAndError = IsModuleLoaded("Microsoft.AspNetCore.Watch.BrowserRefresh.dll") || IsModuleLoaded("Microsoft.WebTools.BrowserLink.Net.dll");
            if (_hotReloadEnabledCheckedAndError.Value)
            {
                AddHotReloadProperty();
                ThrowError();
            }          
        }

        private static bool IsModuleLoaded(string moduleName)
        {
            return Process.GetCurrentProcess().Modules.OfType<ProcessModule>().Any(m => m.ModuleName.Contains(moduleName));
        }

        private static bool AddHotReloadProperty()
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
                IISExpress.Property("environmentVariables").AddAfterSelf(new JProperty("hotReloadEnabled", true));

                File.WriteAllText(launchSettingsFile, launchSettings.ToString());

                return true;
            }
            catch
            {
                return false;
            }           
        }

        private static void ThrowError()
        {
            throw new System.Exception(errorMessage);
        }
    }
}
