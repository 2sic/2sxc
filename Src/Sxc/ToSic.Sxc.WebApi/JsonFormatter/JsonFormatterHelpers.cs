using System.Text.Json;

namespace ToSic.Sxc.WebApi
{
    public class JsonFormatterHelpers
    {
        public static void SetCasing(Casing casing, JsonSerializerOptions jsonSerializerOptions)
        {
            if (casing == Casing.Unspecified) return;

            var objectPreserve = casing.HasFlag(Casing.Preserve);
            jsonSerializerOptions.PropertyNamingPolicy = objectPreserve ? null : JsonNamingPolicy.CamelCase;

            var dicPreserve = (objectPreserve && !casing.HasFlag(Casing.DictionaryCamel))
                              || casing.HasFlag(Casing.DictionaryPreserve);
            jsonSerializerOptions.DictionaryKeyPolicy = dicPreserve ? null : JsonNamingPolicy.CamelCase;
        }
    }
}
