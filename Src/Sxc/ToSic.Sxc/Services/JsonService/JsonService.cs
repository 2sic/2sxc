using System.Text.Json;
using ToSic.Lib.Documentation;
using ToSic.Lib.Services;
using ToSic.Sxc.Data;
using static ToSic.Eav.Parameters;
using static ToSic.Eav.Serialization.JsonOptions;

namespace ToSic.Sxc.Services
{
    [PrivateApi("Hide implementation")]
    internal class JsonService: ServiceBase, IJsonService
    {
        public JsonService(): base("Sxc.JsnSvc")
        {

        }

        /// <inheritdoc />
        public T To<T>(string json) 
            => JsonSerializer.Deserialize<T>(json, SafeJsonForHtmlAttributes);

        /// <inheritdoc />
        public object ToObject(string json)
            => JsonSerializer.Deserialize<object>(json, SafeJsonForHtmlAttributes);

        /// <inheritdoc />
        public ITyped ToTyped(string json, string noParamOrder = Protector, string fallback = default) 
            => DynamicJacket.AsDynamicJacket(json, fallback, Log);

        /// <inheritdoc />
        public string ToJson(object item)
            => JsonSerializer.Serialize(item, SafeJsonForHtmlAttributes);

        /// <inheritdoc />
        public string ToJson(object item, int indentation)
            => JsonSerializer.Serialize(item, SafeJsonForHtmlAttributes);
    }
}
