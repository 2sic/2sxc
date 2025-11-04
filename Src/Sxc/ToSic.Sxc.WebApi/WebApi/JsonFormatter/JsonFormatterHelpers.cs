using System.Text.Json;

namespace ToSic.Sxc.WebApi;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class JsonFormatterHelpers
{
    /// <summary>
    /// Get the casing configuration from JsonSerializerOptions.
    /// Analyzes both PropertyNamingPolicy and DictionaryKeyPolicy to determine the casing.
    /// </summary>
    /// <param name="options">The JsonSerializerOptions to analyze</param>
    /// <returns>The detected Casing configuration</returns>
    internal static Casing GetCasing(JsonSerializerOptions options)
    {
        if (options == null)
            return Casing.Unspecified;

        var isCamelCase = options.PropertyNamingPolicy == JsonNamingPolicy.CamelCase;
        var isDictionaryCamelCase = options.DictionaryKeyPolicy == JsonNamingPolicy.CamelCase;

        // If both are camelCase, return the simple Camel flag
        if (isCamelCase && isDictionaryCamelCase)
            return Casing.Camel;

        // If both preserve original casing
        if (!isCamelCase && !isDictionaryCamelCase)
            return Casing.Preserve;

        // Mixed scenarios - use granular flags
        var result = Casing.Unspecified;

        if (isCamelCase)
            result |= Casing.Camel;
        else
            result |= Casing.Preserve;

        if (isDictionaryCamelCase)
            result |= Casing.DictionaryCamel;
        else
            result |= Casing.DictionaryPreserve;

        return result;
    }


    public static void SetCasing(Casing casing, JsonSerializerOptions jsonSerializerOptions)
    {
        // this preserves casing (old behavior for 2sxc Apis)
        if (casing == Casing.Unspecified) return;

        var objectPreserve = casing.HasFlag(Casing.Preserve);
        jsonSerializerOptions.PropertyNamingPolicy = objectPreserve ? null : JsonNamingPolicy.CamelCase;

        var dicPreserve = (objectPreserve && !casing.HasFlag(Casing.DictionaryCamel))
                          || casing.HasFlag(Casing.DictionaryPreserve);
        jsonSerializerOptions.DictionaryKeyPolicy = dicPreserve ? null : JsonNamingPolicy.CamelCase;
    }
}