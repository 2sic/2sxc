using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Data.Typed;
using static ToSic.Eav.Parameters;
using static ToSic.Eav.Serialization.JsonOptions;
using static ToSic.Sxc.Data.Wrapper.WrapperConstants;

namespace ToSic.Sxc.Data.Wrapper
{
    public class CodeJsonWrapper: ServiceBase
    {
        private readonly Generator<WrapObjectTyped> _wrapTypeGenerator;
        public WrapperSettings Settings { get; private set; }

        public CodeJsonWrapper(Generator<WrapObjectTyped> wrapTypeGenerator) : base($"{Constants.SxcLogName}.CdJsWr")
        {
            ConnectServices(
                _wrapTypeGenerator = wrapTypeGenerator
            );
        }

        public CodeJsonWrapper Setup(WrapperSettings settings)
        {
            Settings = settings;
            return this;
        }

        internal DynamicJacketBase Json2Jacket(string json, string noParamOrder = Protector, string fallback = default)
        {
            Protect(noParamOrder, nameof(fallback));
            return IfJsonTryConvertToJacket(AsJsonNode(json, fallback)).Final;
        }

        public ITyped Json2Typed(string json, string noParamOrder = Protector, string fallback = default)
        {
            Protect(noParamOrder, nameof(fallback));
            return IfJsonTryConvertToTyped(AsJsonNode(json, fallback)).Final;
        }


        private static JsonNode AsJsonNode(string json, string fallback = EmptyJson)
        {
            if (json.HasValue())
                try
                {
                    // find first possible opening character
                    var firstCharPos = json.IndexOfAny(new[] { JObjStart, JArrayStart });
                    if (firstCharPos > -1)
                    {
                        var firstChar = json[firstCharPos];
                        switch (firstChar)
                        {
                            case JObjStart:
                                return JsonNode.Parse(json, JsonNodeDefaultOptions, JsonDocumentDefaultOptions)?.AsObject();
                            case JArrayStart:
                                return JsonNode.Parse(json, JsonNodeDefaultOptions, JsonDocumentDefaultOptions)?.AsArray();
                        }
                    }
                }
                catch
                {
                    if (fallback == JsonErrorCode) throw;
                }

            // fallback
            return fallback == null
                ? null
                : JsonNode.Parse(fallback, JsonNodeDefaultOptions, JsonDocumentDefaultOptions);
        }

        private (object Final, bool Ok, JsonValueKind ValueKind) IfJsonTryConvertToWrapper(object original)
        {
            return Settings.WrapToDynamic
                ? ((object Final, bool Ok, JsonValueKind ValueKind))IfJsonTryConvertToJacket(original)
                : IfJsonTryConvertToTyped(original);
        }
        private (DynamicJacketBase Final, bool Ok, JsonValueKind ValueKind) IfJsonTryConvertToJacket(object original)
        {
            return IfJsonTryConvertToJacket<DynamicJacketBase>(original, CreateDynJacketObject, CreateDynJacketList);
        }
        private (ITyped Final, bool Ok, JsonValueKind ValueKind) IfJsonTryConvertToTyped(object original)
        {
            return IfJsonTryConvertToJacket<ITyped>(original, CreateTypedObject, CreateTypedList);
        }

        internal DynamicJacket CreateDynJacketObject(JsonObject jsonObject)
        {
            var preWrap = new PreWrapJsonObject(this, jsonObject, Settings);
            return new DynamicJacket(this, preWrap);
        }

        private DynamicJacketList CreateDynJacketList(JsonArray jsonArray)
        {
            var preWrap = new PreWrapJsonArray(this, jsonArray, Settings);
            return new DynamicJacketList(this, preWrap);
        }
        private WrapObjectTyped CreateTypedList(JsonArray jsonArray)
        {
            var preWrap = new PreWrapJsonArray(this, jsonArray, Settings);
            return _wrapTypeGenerator.New().Setup(preWrap);
        }
        private WrapObjectTyped CreateTypedObject(JsonObject jsonObject)
        {
            var preWrap = new PreWrapJsonObject(this, jsonObject, Settings);
            return _wrapTypeGenerator.New().Setup(preWrap);
        }

        private (TResult Final, bool Ok, JsonValueKind ValueKind)
            IfJsonTryConvertToJacket<TResult>(object original, Func<JsonObject, TResult> toObj, Func<JsonArray, TResult> toArr) where TResult : class
        {
            ILogCall<(TResult Final, bool Ok, JsonValueKind ValueKind)> l = Log.Fn<(TResult Jacket, bool Ok, JsonValueKind ValueKind)>();
            if (!(original is JsonNode jsonNode))
                return l.Return((null, false, JsonValueKind.Undefined), "not json node");

            switch (jsonNode)
            {
                case JsonArray jArray:
                    return l.Return((toArr(jArray), true, JsonValueKind.Array), "array");
                case JsonObject jResult: // it's another complex object, so return another wrapped reader
                    return l.Return((toObj(jResult), true, JsonValueKind.Object), "obj");
                case JsonValue jValue: // it's a simple value - so we want to return the underlying real value
                    {
                        var je = jValue.GetValue<JsonElement>();
                        switch (je.ValueKind)
                        {
                            case JsonValueKind.Object:
                                return l.Return((toObj(JsonObject.Create(je)), true, JsonValueKind.Object), "val obj");
                            case JsonValueKind.Array:
                                return l.Return((toArr(JsonArray.Create(je)), true, JsonValueKind.Array), "val array");
                            default:
                                return l.Return((null, false, JsonValueKind.Undefined), "val not handled");
                        }
                    }
                default: // it's something else, let's just return that
                    return l.Return((null, false, JsonValueKind.Undefined), $"{nameof(jsonNode)} not handled");
            }
        }

        /// <summary>
        /// Takes a JSON Node and if it's just a value, return that.
        /// If it's a complex object, place it in a jacket again for dynamic code to be able to navigate it. 
        /// So if it's a simple value, it's returned as a value, otherwise it's wrapped into a DynamicJacket again.
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        [PrivateApi]
        internal object IfJsonGetValueOrJacket(object original)
        {
            if (!(original is JsonNode jsonNode)) return original;

            var maybeJacket = IfJsonTryConvertToWrapper(original);
            if (maybeJacket.Ok) return maybeJacket.Final;

            switch (jsonNode)
            {
                case JsonValue jValue: // it's a simple value - so we want to return the underlying real value
                    {
                        var je = jValue.GetValue<JsonElement>();
                        switch (je.ValueKind)
                        {
                            case JsonValueKind.True:
                                return true;
                            case JsonValueKind.False:
                                return false;
                            case JsonValueKind.Number when je.TryGetInt32(out var intValue):
                                return intValue;
                            case JsonValueKind.Number when je.TryGetInt64(out var longValue):
                                return longValue;
                            case JsonValueKind.Number:
                                return je.GetDouble();
                            case JsonValueKind.String when je.TryGetDateTime(out var dateTime):
                                return dateTime;
                            case JsonValueKind.String:
                                return je.GetString();
                            case JsonValueKind.Null:
                            case JsonValueKind.Undefined:
                                return null;
                            //case JsonValueKind.Object:
                            //    return new DynamicJacket(JsonObject.Create(je));
                            //case JsonValueKind.Array:
                            //    return new DynamicJacketList(JsonArray.Create(je));
                            default:
                                return jValue.AsValue();
                        }
                    }
                default: // it's something else, let's just return that
                    return original;
            }
        }


    }
}
