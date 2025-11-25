using System.Text.Json;
using ToSic.Eav.Apps.Sys.FileSystemState;

namespace ToSic.Sxc.Backend.App;

[ShowApiWhenReleased(ShowApiMode.Never)]
public static class ExtensionManifestSerializer
{
    private static readonly JsonElement JsonNullElement = JsonDocument.Parse("null").RootElement.Clone();

    private static readonly JsonSerializerOptions DefaultOptions = new()
    {
        WriteIndented = true
    };

    private static JsonElement Sanitize(JsonElement el) => el.ValueKind == JsonValueKind.Undefined ? JsonNullElement : el;

    /// <summary>
    /// Serialize manifest ensuring undefined JsonElements become null to prevent System.Text.Json exceptions.
    /// </summary>
    public static string Serialize(ExtensionManifest manifest, JsonSerializerOptions? options = null)
    {
        var sanitized = manifest with
        {
            DataBundles = Sanitize(manifest.DataBundles),
            InputTypeAssets = Sanitize(manifest.InputTypeAssets),
            Releases = Sanitize(manifest.Releases),
        };
        return JsonSerializer.Serialize(sanitized, options ?? DefaultOptions);
    }

    public static string Serialize(JsonElement manifest, JsonSerializerOptions? options = null)
        => JsonSerializer.Serialize(manifest, options ?? DefaultOptions);
}
