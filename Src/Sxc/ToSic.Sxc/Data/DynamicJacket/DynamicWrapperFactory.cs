using System.Text.Json.Nodes;
using System.Text.Json;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using System.Dynamic;
using System;
using System.Collections.Generic;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Data.Typed;
using ToSic.Sxc.Services;
using static ToSic.Eav.Serialization.JsonOptions;

namespace ToSic.Sxc.Data
{
    [PrivateApi]
    public class DynamicWrapperFactory: ServiceBase
    {

        [PrivateApi]
        public const string EmptyJson = "{}";
        [PrivateApi]
        private const char JObjStart = '{';
        [PrivateApi]
        private const char JArrayStart = '[';
        [PrivateApi]
        private const string JsonErrorCode = "error";

        public DynamicWrapperFactory(LazySvc<ConvertForCodeService> forCodeConverter): base("Sxc.DWrpFk")
        {
            ConnectServices(
                _forCodeConverter = forCodeConverter
            );
        }

        internal ConvertForCodeService ConvertForCode => _forCodeConverter.Value;
        private readonly LazySvc<ConvertForCodeService> _forCodeConverter;

        internal DynamicJacketBase FromJson(string json, string fallback = default)
            => TryToConvertToJacket(AsJsonNode(json, fallback ?? EmptyJson)).Jacket;

        internal DynamicReadDictionary<TKey, TValue> FromDictionary<TKey, TValue>(IDictionary<TKey, TValue> original)
            => new DynamicReadDictionary<TKey, TValue>(original, this);

        public DynamicReadObject FromObject(object data, bool wrapChildren, bool wrapRealChildren)
            => new DynamicReadObject(data, wrapChildren, wrapRealChildren, this);

        public ITyped TypedFromObject(object data, bool wrapChildren, bool wrapRealChildren)
        {
            var dyn = FromObject(data, wrapChildren, wrapRealChildren);
            return new TypedObjectWrapper(dyn.Analyzer, this);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="wrapRealObjects">if true and the contents isn't already a dynamic object, it will also wrap real objects; otherwise only anonymous</param>
        /// <param name="wrapChildren"></param>
        /// <param name="wrapRealChildren"></param>
        /// <returns></returns>
        [PrivateApi]
        internal object WrapIfPossible(object value, bool wrapRealObjects, bool wrapChildren, bool wrapRealChildren, bool wrapIntoTyped = false)
        {
            // If null or simple value, use that
            if (value is null) return null;

            if (value is string || value.GetType().IsValueType) return value;

            // Guids & DateTimes are objects, but very simple, and should be returned for normal use
            if (value is Guid || value is DateTime) return value;

            // Check if the result is a JSON object which should support navigation again
            var result = WrapIfJObjectUnwrapIfJValue(value);

            // Check if the result already supports navigation... - which is the case if it's a DynamicJacket now
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
            var wrap = wrapRealObjects || value.IsAnonymous();
            return wrap ? FromObject(value, wrapChildren, wrapRealChildren) : value;
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

        private (DynamicJacketBase Jacket, bool Ok, JsonValueKind ValueKind) TryToConvertToJacket(object original)
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
        /// Takes a result of a object query and ensures it will be treated as a DynamicJacket as well.
        /// So if it's a simple value, it's returned as a value, otherwise it's wrapped into a DynamicJacket again.
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        [PrivateApi]
        internal object WrapIfJObjectUnwrapIfJValue(object original)
        {
            if (!(original is JsonNode jsonNode)) return original;

            var maybeJacket = TryToConvertToJacket(original);
            if (maybeJacket.Ok) return maybeJacket.Jacket;

            switch (jsonNode)
            {
                //case JsonArray jArray:
                //    return new DynamicJacketList(jArray);
                //case JsonObject jResult: // it's another complex object, so return another wrapped reader
                //    return new DynamicJacket(jResult);
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
