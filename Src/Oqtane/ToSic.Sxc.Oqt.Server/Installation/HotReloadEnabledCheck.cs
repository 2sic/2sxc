using System.Diagnostics;
using System.IO;
using System.Text.Json.Nodes;
using ToSic.Eav.Serialization;

namespace ToSic.Sxc.Oqt.Server.Installation;

internal static class HotReloadEnabledCheck
{
    private static bool? _hotReloadEnabledCheckedAndError;

    private static string errorMessage = "Warning: You must run Oqtane without Hot-Reload to install Apps. See https://go.2sxc.org/oqt-hr";

    internal static void Check()
    {
        // Don't repeat if already checked
        if (!_hotReloadEnabledCheckedAndError.HasValue)
        {
            // Check if Hot Reload is Enabled.
            // When HotReloadEnabled is not false, special modules are loaded, so we try to find them.
            _hotReloadEnabledCheckedAndError = IsModuleLoaded("Microsoft.AspNetCore.Watch.BrowserRefresh.dll");
            if (_hotReloadEnabledCheckedAndError.Value) 
                AddHotReloadProperty();
        }

        if (_hotReloadEnabledCheckedAndError.Value)
            throw new(errorMessage);
         
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

            var launchSettings = JsonNode.Parse(launchSettingsJson, JsonOptions.JsonNodeDefaultOptions, JsonOptions.JsonDocumentDefaultOptions);
            var profiles = launchSettings?["profiles"]?.AsObject();
            var iisExpress = profiles?["IIS Express"]?.AsObject();

            // json configuration is wrong
            if (iisExpress is null) return false;

            // if hotReloadEnabled property exists do nothing
            if (iisExpress?["hotReloadEnabled"]?.AsValue() is not null) return false;

            // add hotReloadEnabled: true
            iisExpress?.Add("hotReloadEnabled", true);

            File.WriteAllText(launchSettingsFile, launchSettings?.ToJsonString(JsonOptions.UnsafeJsonWithoutEncodingHtml));

            return true;
        }
        catch
        {
            return false;
        }           
    }
}