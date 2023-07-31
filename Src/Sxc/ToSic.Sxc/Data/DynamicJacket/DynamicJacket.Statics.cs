//using System.Text.Json;
//using System.Text.Json.Nodes;
//using ToSic.Lib.Documentation;
//using ToSic.Lib.Logging;
//using static ToSic.Eav.Serialization.JsonOptions;

//namespace ToSic.Sxc.Data
//{
//    public partial class DynamicJacket
//    {
//        [PrivateApi]
//        public const string EmptyJson = "{}";
//        [PrivateApi]
//        private const char JObjStart = '{';
//        [PrivateApi]
//        private const char JArrayStart = '[';
//        [PrivateApi]
//        private const string JsonErrorCode = "error";

//        [PrivateApi]
//        internal static DynamicJacketBase AsDynamicJacket(string json, string fallback = default, ILog log = default)
//            => TryToConvertToJacket(AsJsonNode(json, fallback ?? EmptyJson), log).Jacket;

//        [PrivateApi]
//        private static JsonNode AsJsonNode(string json, string fallback = EmptyJson)
//        {
//            if (!string.IsNullOrWhiteSpace(json))
//                try
//                {
//                    // find first possible opening character
//                    var firstCharPos = json.IndexOfAny(new[] { JObjStart, JArrayStart });
//                    if (firstCharPos > -1)
//                    {
//                        var firstChar = json[firstCharPos];
//                        switch (firstChar)
//                        {
//                            case JObjStart:
//                                return JsonNode.Parse(json, JsonNodeDefaultOptions, JsonDocumentDefaultOptions)?.AsObject();
//                            case JArrayStart:
//                                return JsonNode.Parse(json, JsonNodeDefaultOptions, JsonDocumentDefaultOptions)?.AsArray();
//                        }
//                    }
//                }
//                catch
//                {
//                    if (fallback == JsonErrorCode) throw;
//                }

//            // fallback
//            return fallback == null
//                ? null
//                : JsonNode.Parse(fallback, JsonNodeDefaultOptions, JsonDocumentDefaultOptions);
//        }

//        internal static (DynamicJacketBase Jacket, bool Ok, JsonValueKind ValueKind) TryToConvertToJacket(object original, ILog log = default)
//        {
//            var l = log.Fn<(DynamicJacketBase Jacket, bool Ok, JsonValueKind ValueKind)>();
//            if (!(original is JsonNode jsonNode))
//                return l.Return((null, false, JsonValueKind.Undefined), "not json node");

//            switch (jsonNode)
//            {
//                case JsonArray jArray:
//                    return l.Return((new DynamicJacketList(jArray), true, JsonValueKind.Array), "array");
//                case JsonObject jResult: // it's another complex object, so return another wrapped reader
//                    return l.Return((new DynamicJacket(jResult), true, JsonValueKind.Object), "obj");
//                case JsonValue jValue: // it's a simple value - so we want to return the underlying real value
//                    {
//                        var je = jValue.GetValue<JsonElement>();
//                        switch (je.ValueKind)
//                        {
//                            case JsonValueKind.Object:
//                                return l.Return((new DynamicJacket(JsonObject.Create(je)), true, JsonValueKind.Object), "val obj");
//                            case JsonValueKind.Array:
//                                return l.Return((new DynamicJacketList(JsonArray.Create(je)), true, JsonValueKind.Array), "val array");
//                            default:
//                                return l.Return((null, false, JsonValueKind.Undefined), "val not handled");
//                        }
//                    }
//                default: // it's something else, let's just return that
//                    return l.Return((null, false, JsonValueKind.Undefined), $"{nameof(jsonNode)} not handled");
//            }
//        }

//        /// <summary>
//        /// Takes a result of a object query and ensures it will be treated as a DynamicJacket as well.
//        /// So if it's a simple value, it's returned as a value, otherwise it's wrapped into a DynamicJacket again.
//        /// </summary>
//        /// <param name="original"></param>
//        /// <returns></returns>
//        [PrivateApi]
//        internal static object WrapIfJObjectUnwrapIfJValue(object original)
//        {
//            if (!(original is JsonNode jsonNode)) return original;

//            var maybeJacket = DynamicJacketFactory.TryToConvertToJacket(original, null);
//            if (maybeJacket.Ok) return maybeJacket.Jacket;

//            switch (jsonNode)
//            {
//                //case JsonArray jArray:
//                //    return new DynamicJacketList(jArray);
//                //case JsonObject jResult: // it's another complex object, so return another wrapped reader
//                //    return new DynamicJacket(jResult);
//                case JsonValue jValue: // it's a simple value - so we want to return the underlying real value
//                    {
//                        var je = jValue.GetValue<JsonElement>();
//                        switch (je.ValueKind)
//                        {
//                            case JsonValueKind.True:
//                                return true;
//                            case JsonValueKind.False:
//                                return false;
//                            case JsonValueKind.Number when je.TryGetInt32(out var intValue):
//                                return intValue;
//                            case JsonValueKind.Number when je.TryGetInt64(out var longValue):
//                                return longValue;
//                            case JsonValueKind.Number:
//                                return je.GetDouble();
//                            case JsonValueKind.String when je.TryGetDateTime(out var dateTime):
//                                return dateTime;
//                            case JsonValueKind.String:
//                                return je.GetString();
//                            case JsonValueKind.Null:
//                            case JsonValueKind.Undefined:
//                                return null;
//                            //case JsonValueKind.Object:
//                            //    return new DynamicJacket(JsonObject.Create(je));
//                            //case JsonValueKind.Array:
//                            //    return new DynamicJacketList(JsonArray.Create(je));
//                            default:
//                                return jValue.AsValue();
//                        }
//                    }
//                default: // it's something else, let's just return that
//                    return original;
//            }
//        }

//    }
//}
