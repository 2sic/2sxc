using System.Text.Json.Nodes;
using ToSic.Eav.Documentation;
using ToSic.Eav.Serialization;

namespace ToSic.Sxc.Data
{
    public partial class DynamicJacket
    {
        [PrivateApi]
        public const string EmptyJson = "{}";
        [PrivateApi]
        private const char JObjStart = '{';
        [PrivateApi]
        private const char JArrayStart = '[';
        [PrivateApi]
        private const string JsonErrorCode = "error";

        [PrivateApi]
        public static object AsDynamicJacket(string json, string fallback = EmptyJson) => WrapIfJObjectUnwrapIfJValue(AsJsonNode(json, fallback));

        [PrivateApi]
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
                                return JsonNode.Parse(json, JsonOptions.JsonNodeDefaultOptions, JsonOptions.JsonDocumentDefaultOptions)?.AsObject();
                            case JArrayStart:
                                return JsonNode.Parse(json, JsonOptions.JsonNodeDefaultOptions, JsonOptions.JsonDocumentDefaultOptions)?.AsArray();
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
                : JsonNode.Parse(fallback, JsonOptions.JsonNodeDefaultOptions, JsonOptions.JsonDocumentDefaultOptions);
        }

        /// <summary>
        /// Takes a result of a object query and ensures it will be treated as a DynamicJacket as well.
        /// So if it's a simple value, it's returned as a value, otherwise it's wrapped into a DynamicJacket again.
        /// </summary>
        /// <param name="original"></param>
        /// <returns></returns>
        [PrivateApi]
        internal static object WrapIfJObjectUnwrapIfJValue(object original)
        {
            
            if (!(original is JsonNode jsonNode)) return original;

            switch (jsonNode)
            {
                case JsonArray jArray:
                    return new DynamicJacketList(jArray);
                case JsonObject jResult: // it's another complex object, so return another wrapped reader
                    return new DynamicJacket(jResult);
                case JsonValue jValue: // it's a simple value - so we want to return the underlying real value
                    return jValue.AsValue();
                case null:
                    return null;
                default: // it's something else, let's just return that
                    return original;
            }
        }

    }
}
