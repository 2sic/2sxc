using System.Net.Http.Formatting;
using System.Text.Json;

namespace ToSic.Sxc.Dnn.WebApi.Sys.HttpJson;
internal class JsonFormatterCasingHelpersForDnn
{
    internal static Casing ExtractCasingFromFormatters(List<SystemTextJsonMediaTypeFormatter> formatters)
    {
        if (formatters is not { Count: > 0 })
            return Casing.Unspecified;

        foreach (var formatter in formatters)
        {
            var options = formatter.JsonSerializerOptions;
            if (options?.PropertyNamingPolicy != null)
                return GetCasing(options);
        }

        return Casing.Unspecified;
    }

    /// <summary>
    /// Get the casing configuration from JsonSerializerOptions.
    /// Analyzes both PropertyNamingPolicy and DictionaryKeyPolicy to determine the casing.
    /// </summary>
    /// <param name="options">The JsonSerializerOptions to analyze</param>
    /// <returns>The detected Casing configuration</returns>
    internal static Casing GetCasing(JsonSerializerOptions options)
    {
        if (options == null! /* paranoid */)
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

}
