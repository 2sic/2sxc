using ToSic.Eav.Serialization;
using System.Text.Json;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Services
{
    [PrivateApi("Hide implementation")]
    public class JsonService: IJsonService
    {
        /// <inheritdoc />
        public T To<T>(string json) => JsonSerializer.Deserialize<T>(json, JsonOptions.SafeJsonForHtmlAttributes);

        /// <inheritdoc />
        public object ToObject(string json) => JsonSerializer.Deserialize<object>(json, JsonOptions.SafeJsonForHtmlAttributes);

        /// <inheritdoc />
        public string ToJson(object item) => JsonSerializer.Serialize(item, JsonOptions.SafeJsonForHtmlAttributes);

        /// <inheritdoc />
        public string ToJson(object item, int indentation) => JsonSerializer.Serialize(item, JsonOptions.SafeJsonForHtmlAttributes);
    }
}
