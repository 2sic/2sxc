using System.Net.Http.Formatting;
using ToSic.Eav.Serialization;

namespace ToSic.Sxc.Dnn.WebApi.Internal.HttpJson;

internal class JsonFormatters
{
    public static SystemTextJsonMediaTypeFormatter SystemTextJsonMediaTypeFormatter =>
        _systemTextJsonMediaTypeFormatter ??= new()
        {
            JsonSerializerOptions = JsonOptions.UnsafeJsonWithoutEncodingHtml
        };
    private static SystemTextJsonMediaTypeFormatter _systemTextJsonMediaTypeFormatter;
}