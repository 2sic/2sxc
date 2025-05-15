using System.Net.Http.Formatting;
using ToSic.Eav.Serialization;

namespace ToSic.Sxc.Dnn.WebApi.Internal.HttpJson;

internal class JsonFormatters
{
    public static SystemTextJsonMediaTypeFormatter SystemTextJsonMediaTypeFormatter =>
        field ??= new()
        {
            JsonSerializerOptions = JsonOptions.UnsafeJsonWithoutEncodingHtml
        };
}