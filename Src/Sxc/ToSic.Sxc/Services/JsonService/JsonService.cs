using System.Text.Json;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Services;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Wrapper;
using static ToSic.Eav.Parameters;
using static ToSic.Eav.Serialization.JsonOptions;

namespace ToSic.Sxc.Services
{
    [PrivateApi("Hide implementation")]
    internal class JsonService: ServiceBase, IJsonService
    {
        private readonly LazySvc<CodeDataWrapper> _dynJacketFactory;

        public JsonService(LazySvc<CodeDataWrapper> dynJacketFactory): base("Sxc.JsnSvc")
        {
            ConnectServices(
                _dynJacketFactory = dynJacketFactory
            );
        }

        /// <inheritdoc />
        public T To<T>(string json) 
            => JsonSerializer.Deserialize<T>(json, SafeJsonForHtmlAttributes);

        /// <inheritdoc />
        public object ToObject(string json)
            => JsonSerializer.Deserialize<object>(json, SafeJsonForHtmlAttributes);

        /// <inheritdoc />
        public ITyped ToTyped(string json, string noParamOrder = Protector, string fallback = default) 
            => _dynJacketFactory.Value.Json2Jacket(json, fallback).Typed;

        /// <inheritdoc />
        public string ToJson(object item)
            => JsonSerializer.Serialize(item, SafeJsonForHtmlAttributes);

        /// <inheritdoc />
        public string ToJson(object item, int indentation)
            => JsonSerializer.Serialize(item, SafeJsonForHtmlAttributes);
    }
}
