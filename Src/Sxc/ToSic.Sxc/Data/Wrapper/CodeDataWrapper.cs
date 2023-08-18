using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Text.Json;
using System.Text.Json.Nodes;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Data.Typed;
using ToSic.Sxc.Services;
using static ToSic.Eav.Serialization.JsonOptions;
using static ToSic.Sxc.Data.Wrapper.WrapperConstants;

namespace ToSic.Sxc.Data.Wrapper
{
    [PrivateApi]
    public class CodeDataWrapper: ServiceBase
    {
        public readonly LazySvc<CodeDataFactory> Cdf;

        public CodeDataWrapper(LazySvc<ConvertForCodeService> forCodeConverter, LazySvc<CodeDataFactory> cdf): base("Sxc.DWrpFk")
        {
            ConnectServices(
                _forCodeConverter = forCodeConverter,
                Cdf = cdf
            );
        }

        internal ConvertForCodeService ConvertForCode => _forCodeConverter.Value;
        private readonly LazySvc<ConvertForCodeService> _forCodeConverter;

        internal DynamicJacketBase Json2Jacket(string json, string fallback = default)
            => IfJsonTryConvertToJacket(AsJsonNode(json, fallback ?? EmptyJson)).Jacket;
        internal ITyped Json2Typed(string json, string fallback = default)
            => IfJsonTryConvertToJacket(AsJsonNode(json, fallback ?? EmptyJson)).Jacket.Typed;

        internal WrapDictionaryDynamic<TKey, TValue> FromDictionary<TKey, TValue>(IDictionary<TKey, TValue> original)
        {
            return new WrapDictionaryDynamic<TKey, TValue>(original, this);
        }

        public WrapObjectDynamic FromObject(object data, WrapperSettings settings)
        {
            var preWrap = new PreWrapObject(data, settings, this);
            return new WrapObjectDynamic(preWrap, this);
        }

        public ITyped TypedFromObject(object data, WrapperSettings settings)
        {
            var preWrap = new PreWrapObject(data, settings, this);
            return new WrapObjectTyped(preWrap, this);
        }
        public ITypedItem TypedItemFromObject(object data, WrapperSettings settings, ILazyLike<CodeDataFactory> cdf = default)
        {
            var preWrap = new PreWrapObject(data, settings, this);
            return new WrapObjectTypedItem(preWrap, this, cdf ?? Cdf);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="wrapNonAnon">if true and the contents isn't already a dynamic object, it will also wrap real objects; otherwise only anonymous</param>
        /// <returns></returns>
        [PrivateApi]
        internal object WrapIfPossible(object data, bool wrapNonAnon, WrapperSettings settings)
        {
            // If null or simple value, use that
            if (data is null) return null;

            if (data is string || data.GetType().IsValueType) return data;

            // Guids & DateTimes are objects, but very simple, and should be returned for normal use
            if (data is Guid || data is DateTime) return data;

            // Check if the result is a JSON object which should support navigation again
            var result = IfJsonGetValueOrJacket(data);

            // Check if the original or result already supports navigation... - which is the case if it's a DynamicJacket now
            switch (result)
            {
                case IPropertyLookup _:
                case ISxcDynamicObject _:
                case DynamicObject _:
                    return result;
            }

            // if (result is IDictionary dicResult) return DynamicReadDictionary(dicResult);

            // Simple arrays don't benefit from re-wrapping. 
            // If the calling code needs to do something with them, it should iterate it and then rewrap with AsDynamic
            if (result.GetType().IsArray) return result;

            // Otherwise it's a complex object, which should be re-wrapped for navigation
            var wrap = wrapNonAnon || data.IsAnonymous();
            return wrap
                ? settings.WrapToDynamic 
                    ? FromObject(data, settings) as object
                    : TypedFromObject(data, settings)
                : data;
        }

        private static JsonNode AsJsonNode(string json, string fallback = EmptyJson)
        {
            if (!string.IsNullOrWhiteSpace(json))
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

        private (DynamicJacketBase Jacket, bool Ok, JsonValueKind ValueKind) IfJsonTryConvertToJacket(object original)
        {
            var l = Log.Fn<(DynamicJacketBase Jacket, bool Ok, JsonValueKind ValueKind)>();
            if (!(original is JsonNode jsonNode))
                return l.Return((null, false, JsonValueKind.Undefined), "not json node");

            switch (jsonNode)
            {
                case JsonArray jArray:
                    return l.Return((new DynamicJacketList(jArray, this), true, JsonValueKind.Array), "array");
                case JsonObject jResult: // it's another complex object, so return another wrapped reader
                    return l.Return((new DynamicJacket(jResult, this), true, JsonValueKind.Object), "obj");
                case JsonValue jValue: // it's a simple value - so we want to return the underlying real value
                    {
                        var je = jValue.GetValue<JsonElement>();
                        switch (je.ValueKind)
                        {
                            case JsonValueKind.Object:
                                return l.Return((new DynamicJacket(JsonObject.Create(je), this), true, JsonValueKind.Object), "val obj");
                            case JsonValueKind.Array:
                                return l.Return((new DynamicJacketList(JsonArray.Create(je), this), true, JsonValueKind.Array), "val array");
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

            var maybeJacket = IfJsonTryConvertToJacket(original);
            if (maybeJacket.Ok) return maybeJacket.Jacket;

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
