using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
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

        public ITyped JsonToTyped(string json, string noParamOrder = Protector, string fallback = default)
        {
            Protect(noParamOrder, nameof(fallback));
            if (!json.HasValue()) return null;
            ThrowIfNotExpected(json, false);
            var node = AsJsonNode(json, fallback);
            var result = IfJsonTryConvertToJacket<ITyped>(node, CreateTypedObject, array => null);
            return result.Final;
        }

        public IEnumerable<ITyped> JsonToTypedList(string json, string noParamOrder = Protector, string fallback = default)
        {
            Protect(noParamOrder, nameof(fallback));
            if (!json.HasValue()) return null;
            ThrowIfNotExpected(json, true);
            var node = AsJsonNode(json, fallback);
            var result = IfJsonTryConvertToJacket(node,
                obj => null as IEnumerable<ITyped>,
                array => array.Select((j, index) => j is JsonObject jo
                        ? CreateTypedObject(jo)
                        : throw new ArgumentException(
                            $"Tried to create array of objects but array seems to contain simple values or something else. '{j}', index: {index}"))
                    .ToList()
            );
            return result.Final;
        }

        public void ThrowIfNotExpected(string json, bool expectArray, [CallerMemberName] string cName = default)
        {
            var (isComplex, isArray) = AnalyzeJson(json);
            if (!isComplex)
                throw new ArgumentException(
                    @"Wrapping Json only works for complex objects. This value is either null, empty or a value type.",
                    nameof(json));

            // If Array-state and expectations match, it's ok
            if (isArray == expectArray) return;

            // Throw if IsArray and it wasn't expected
            if (isArray)
                throw new ArgumentException(@"Expected an object but got an array. For arrays you should use ToTypedList(...)", nameof(json));

            // Not array, but apparently expected
            throw new ArgumentException(@"Expected an array but got an object. For objects you should use ToTyped(...)",
                nameof(json));
        }


        private static JsonNode AsJsonNode(string json, string fallback = EmptyJson)
        {
            if (!json.HasValue()) return fallback == null ? null : StandardParse(fallback);

            try
            {
                var (isComplex, isArray) = AnalyzeJson(json);
                if (isComplex)
                {
                    var node = StandardParse(json);
                    if (node != null)
                        return node;
                        // return isArray ? node.AsArray() as JsonNode : node.AsObject();
                }
                // 2023-08-23 2dm - this is fairly complex, I believe the conversions are not necessary
                //var firstCharPos = json.IndexOfAny(new[] { JObjStart, JArrayStart });
                //if (firstCharPos > -1)
                //{
                //    var firstChar = json[firstCharPos];
                //    switch (firstChar)
                //    {
                //        case JObjStart: return StandardParse(json)?.AsObject();
                //        case JArrayStart: return StandardParse(json)?.AsArray();
                //    }
                //}
            }
            catch
            {
                if (fallback == JsonErrorCode) throw;
            }

            // fallback
            return fallback == null ? null : StandardParse(fallback);
        }

        /// <summary>
        /// Find out if a string is a complex object (obj/array) and if it's an array.
        /// </summary>
        /// <param name="json"></param>
        /// <returns></returns>
        public static (bool IsComplex, bool IsArray) AnalyzeJson(string json)
        {
            // find first possible opening character
            var firstCharPos = json.IndexOfAny(new[] { JObjStart, JArrayStart });
            return firstCharPos <= -1 
                ? (false, false) 
                : (true, json[firstCharPos] == JArrayStart);
        }

        private static JsonNode StandardParse(string json) =>
            JsonNode.Parse(json, JsonNodeDefaultOptions, JsonDocumentDefaultOptions);


        private (object Final, bool Ok, JsonValueKind ValueKind) IfJsonTryConvertToWrapper(object original) =>
            Settings.WrapToDynamic
                ? ((object Final, bool Ok, JsonValueKind ValueKind))IfJsonTryConvertToJacket(original)
                : IfJsonTryConvertToTyped(original);

        private (DynamicJacketBase Final, bool Ok, JsonValueKind ValueKind) IfJsonTryConvertToJacket(object original) =>
            IfJsonTryConvertToJacket<DynamicJacketBase>(original, CreateDynJacketObject, CreateDynJacketList);

        private (ITyped Final, bool Ok, JsonValueKind ValueKind) IfJsonTryConvertToTyped(object original) =>
            IfJsonTryConvertToJacket<ITyped>(original, CreateTypedObject, CreateTypedList);

        internal DynamicJacket CreateDynJacketObject(JsonObject jsonObject)
        {
            var preWrap = new PreWrapJsonObject(this, jsonObject);
            return new DynamicJacket(this, preWrap);
        }
        private DynamicJacketList CreateDynJacketList(JsonArray jsonArray)
        {
            var preWrap = new PreWrapJsonArray(this, jsonArray);
            return new DynamicJacketList(this, preWrap);
        }
        private WrapObjectTyped CreateTypedList(JsonArray jsonArray)
        {
            var preWrap = new PreWrapJsonArray(this, jsonArray);
            return _wrapTypeGenerator.New().Setup(preWrap);
        }
        private WrapObjectTyped CreateTypedObject(JsonObject jsonObject)
        {
            var preWrap = new PreWrapJsonObject(this, jsonObject);
            return _wrapTypeGenerator.New().Setup(preWrap);
        }

        private (TResult Final, bool Ok, JsonValueKind ValueKind)
            IfJsonTryConvertToJacket<TResult>(object original, Func<JsonObject, TResult> toObj, Func<JsonArray, TResult> toArr) where TResult : class
        {
            var l = Log.Fn<(TResult Jacket, bool Ok, JsonValueKind ValueKind)>();
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
