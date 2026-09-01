using System.Text.Json.Nodes;
using ToSic.Eav.Serialization.Sys.Json;

namespace ToSic.Sxc.Oqt.Server.Installation;

internal static class HotReloadEnabledCheck
{
    private static bool? _hotReloadEnabledCheckedAndError;

    private const string errorMessage = "Warning: You must run Oqtane without Hot-Reload to install Apps. See https://go.2sxc.org/oqt-hr";

    internal static void Check()
    {
        // Don't repeat if already checked
        if (!_hotReloadEnabledCheckedAndError.HasValue)
        {
            // Check if Hot Reload is Enabled.
            _hotReloadEnabledCheckedAndError = IsHotReloadActive();
            if (_hotReloadEnabledCheckedAndError.Value)
                AddHotReloadPropertyWhenIsMissing(Path.Combine(Directory.GetCurrentDirectory(), "Properties", "launchSettings.json"));
        }

        if (_hotReloadEnabledCheckedAndError.Value)
            throw new(errorMessage);
    }

    /// <summary>
    /// True when the runtime will accept hot-reload deltas.
    /// The CLR only loads assemblies in modifiable state when DOTNET_MODIFIABLE_ASSEMBLIES=debug,
    /// which VS and dotnet-watch set only while Hot Reload is on.
    /// Do NOT test for Microsoft.AspNetCore.Watch.BrowserRefresh - as of VS2026 / .NET 10 that module
    /// is also loaded when just browser-refresh is active and Hot Reload is off (false positive).
    /// </summary>
    private static bool IsHotReloadActive()
        => string.Equals(Environment.GetEnvironmentVariable("DOTNET_MODIFIABLE_ASSEMBLIES"), "debug", StringComparison.OrdinalIgnoreCase);

    /// <summary>
    /// Set "hotReloadEnabled": false on every launch profile, so the next start is clean.
    /// Returns true if the file was changed.
    /// </summary>
    /// <remarks>
    /// In the past we set "hotReloadEnabled": true, for most common case with Oqtane source code in Visual Studio in Debug with HotReload on Windows, hosted in IISExpress.
    /// HotReload become core dotnet feature, working across many dev environments with Kestrel and 2sxc supports Linux and macOS platforms requiring to disable HotReload
    /// for Oqtane to work properly. So now we set "hotReloadEnabled": false to avoid issues with HotReload on all platforms.
    /// </remarks>
    internal static bool AddHotReloadPropertyWhenIsMissing(string launchSettingsFile)
    {
        if (!File.Exists(launchSettingsFile)) return false;
        try
        {
            var launchSettings = JsonNode.Parse(File.ReadAllText(launchSettingsFile), JsonOptions.JsonNodeDefaultOptions, JsonOptions.JsonDocumentDefaultOptions);
            var profiles = launchSettings?["profiles"]?.AsObject();
            if (profiles is null) return false;

            var changed = false;
            foreach (var profile in profiles.Select(p => p.Value as JsonObject).Where(p => p is not null))
            {
                // if hot reload exists, leave it alone (do not automatically change user intended configuration)
                if (profile!["hotReloadEnabled"] is JsonValue existing /*&& existing.TryGetValue<bool>(out var enabled) && !enabled*/)
                    continue;
                profile["hotReloadEnabled"] = false; // change to 'false' (default is 'true'), so the next start is clean and will not have hot reload enabled
                changed = true;
            }

            if (changed)
                File.WriteAllText(launchSettingsFile, launchSettings!.ToJsonString(JsonOptions.UnsafeJsonWithoutEncodingHtml));

            return changed;
        }
        catch
        {
            return false;
        }
    }
}
