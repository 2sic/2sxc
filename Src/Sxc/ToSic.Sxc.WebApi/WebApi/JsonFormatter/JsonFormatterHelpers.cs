using System.Text.Json;

namespace ToSic.Sxc.WebApi;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal static class JsonFormatterHelpers
{

    public static void SetCasing(this JsonSerializerOptions jsonSerializerOptions, Casing casing)
    {
        // this preserves casing (old behavior for 2sxc Apis)
        if (casing == Casing.Unspecified)
            return;

        var objectPreserve = casing.HasFlag(Casing.Preserve);
        jsonSerializerOptions.PropertyNamingPolicy = objectPreserve ? null : JsonNamingPolicy.CamelCase;

        var dicPreserve = (objectPreserve && !casing.HasFlag(Casing.DictionaryCamel))
                          || casing.HasFlag(Casing.DictionaryPreserve);
        jsonSerializerOptions.DictionaryKeyPolicy = dicPreserve ? null : JsonNamingPolicy.CamelCase;
    }
}