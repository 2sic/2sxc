using System.IO.Compression;
using System.Text.Json;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.WebApi.Tests.Extensions;

/// <summary>
/// Helper methods for ExportExtension tests
/// </summary>
internal static class ExportExtensionTestHelpers
{
    /// <summary>
    /// Extract and parse json file from ZIP
    /// </summary>
    public static Dictionary<string, object> GetJsonFileFromZip(ZipArchive zip, string jsonFile)
    {
        var jsonEntry = zip.Entries.FirstOrDefault(e => e.FullName.EndsWith(jsonFile, StringComparison.OrdinalIgnoreCase))
                        ?? throw new InvalidOperationException($"{jsonFile} not found in zip archive");
        using var jsonStream = jsonEntry.Open();
        var dict = JsonSerializer.Deserialize<Dictionary<string, object>>(jsonStream);
        return dict ?? throw new InvalidOperationException($"Failed to deserialize '{jsonEntry.FullName}'");
    }

    /// <summary>
    /// Get a JsonElement value as JsonElement
    /// </summary>
    public static JsonElement GetElement(this Dictionary<string, object> dict, string key)
        => (JsonElement)dict[key];

    /// <summary>
    /// Get a JsonElement value as boolean
    /// </summary>
    public static bool GetBool(this Dictionary<string, object> dict, string key)
            => GetElement(dict, key).GetBoolean();

    /// <summary>
    /// Get a JsonElement value as string
    /// </summary>
    public static string? GetString(this Dictionary<string, object> dict, string key)
        => GetElement(dict, key).GetString();
}
