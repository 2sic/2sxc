using System.Text.Json;

namespace ToSic.Sxc.WebApi;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class JsonFormatterHelpers
{
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