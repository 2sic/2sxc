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
using static ToSic.Sxc.Data.Wrapper.JsonProcessingHelpers;

namespace ToSic.Sxc.Data.Wrapper
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class CodeJsonWrapper: ServiceBase
    {
        #region Constructor / Setup

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

        #endregion


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
            var result = IfJsonTryConvertTo<ITyped>(node, CreateTypedObject, array => null);
            return result.Final;
        }

        public IEnumerable<ITyped> JsonToTypedList(string json, string noParamOrder = Protector, string fallback = default)
        {
            Protect(noParamOrder, nameof(fallback));
            if (!json.HasValue()) return null;
            ThrowIfNotExpected(json, true);
            var node = AsJsonNode(json, fallback);
            var result = IfJsonTryConvertTo(node, obj => null, arr => JsonArrayToTypedList(arr, true));
            return result.Final;
        }

        private IEnumerable<ITyped> JsonArrayToTypedList(JsonArray array, bool errorIfNotPossible)
        {
            if (!errorIfNotPossible && array.Any(jItem => !(jItem is JsonObject)))
                return null;

            return array.Select((j, index) => j is JsonObject jo
                    ? CreateTypedObject(jo)
                    : throw new ArgumentException(
                        $"Tried to create array of objects but array seems to contain simple values or something else. '{j}', index: {index}"))
                .ToList();
        }

        private void ThrowIfNotExpected(string json, bool expectArray, [CallerMemberName] string cName = default)
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
                throw new ArgumentException($@"Expected an object but got an array. For arrays you should use ToTypedList(...), not {cName}", nameof(json));

            // Not array, but apparently expected
            throw new ArgumentException($@"Expected an array but got an object. For objects you should use ToTyped(...), not {cName}",
                nameof(json));
        }




        private (DynamicJacketBase Final, bool Ok, JsonValueKind ValueKind) IfJsonTryConvertToJacket(object original) =>
            IfJsonTryConvertTo<DynamicJacketBase>(original, CreateDynJacketObject, CreateDynJacketList);


        internal DynamicJacket CreateDynJacketObject(JsonObject jsonObject) => 
            new DynamicJacket(this, new PreWrapJsonObject(this, jsonObject));

        private DynamicJacketList CreateDynJacketList(JsonArray jsonArray) => 
            new DynamicJacketList(this, new PreWrapJsonArray(this, jsonArray));

        //private WrapObjectTyped CreateTypedList(JsonArray jsonArray) => 
        //    _wrapTypeGenerator.New().Setup(new PreWrapJsonArray(this, jsonArray));

        private WrapObjectTyped CreateTypedObject(JsonObject jsonObject) => 
            _wrapTypeGenerator.New().Setup(new PreWrapJsonObject(this, jsonObject));

        private (TResult Final, bool Ok, JsonValueKind ValueKind)
            IfJsonTryConvertTo<TResult>(object original, Func<JsonObject, TResult> toObj, Func<JsonArray, TResult> toArr) where TResult : class
        {
            var l = Log.Fn<(TResult Jacket, bool Ok, JsonValueKind ValueKind)>();
            if (!(original is JsonNode jsonNode))
                return l.Return((null, false, JsonValueKind.Undefined), "not json node");

            // 2023-08-24 2dm simplified this by preprocessing. Leave the code in till end of September 2023 
            var (node, repackaged) = NeutralizeValueToObjectOrArray(jsonNode);
            jsonNode = node ?? jsonNode;
            var logPrefix = repackaged ? "val " : "";

            switch (jsonNode)
            {
                case JsonArray jArray:
                    return l.Return((toArr(jArray), true, JsonValueKind.Array), $"{logPrefix}array");
                case JsonObject jResult: // it's another complex object, so return another wrapped reader
                    return l.Return((toObj(jResult), true, JsonValueKind.Object), $"{logPrefix}obj");
                // 2023-08-24 2dm simplified this by preprocessing. Leave the code in till end of September 2023 
                //case JsonValue jValue: // it's a simple value - so we want to return the underlying real value
                //    {
                //        // TODO: 2dm must discuss w/STV - is this code even reachable?
                //        // My guess is that all these cases are handled before
                //        var je = jValue.GetValue<JsonElement>();
                //        switch (je.ValueKind)
                //        {
                //            case JsonValueKind.Array:
                //                return l.Return((toArr(JsonArray.Create(je)), true, JsonValueKind.Array), "val array");
                //            case JsonValueKind.Object:
                //                return l.Return((toObj(JsonObject.Create(je)), true, JsonValueKind.Object), "val obj");
                //            default:
                //                return l.Return((null, false, JsonValueKind.Undefined), "val not handled");
                //        }
                //    }
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

            if (!Settings.WrapToDynamic)
            {
                var wrapTyped = IfJsonTryConvertTo<object>(original,
                        CreateTypedObject,
                        jArr => JsonArrayToTypedList(jArr, errorIfNotPossible: false));
                if (wrapTyped.Ok && wrapTyped.Final != null) return wrapTyped.Final;

                // New case in Typed only, as it won't wrap arrays which are not complex object
                if (jsonNode is JsonArray jsonArray)
                    return jsonArray.Select(JsonValueGetContents).ToList();
            }
            else
            {
                var (wrapResult, wrapOk, _) = IfJsonTryConvertToJacket(original);
                if (wrapOk) return wrapResult;
            };



            // If not a value-type, assume we can't process it
            if (!(jsonNode is JsonValue jValue)) return original;

            return JsonValueGetContents(jValue);
        }
    }
}
