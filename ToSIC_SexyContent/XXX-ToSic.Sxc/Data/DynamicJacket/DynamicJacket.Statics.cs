using Newtonsoft.Json.Linq;
using ToSic.Eav.Documentation;

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
        public static object AsDynamicJacket(string json, string fallback = EmptyJson) => WrapOrUnwrap(AsDynamic(json, fallback));

        [PrivateApi]
        private static JToken AsDynamic(string json, string fallback = EmptyJson)
        {
            if (!string.IsNullOrWhiteSpace(json))
                try
                {
                    // find first possible opening character
                    var firstCharPos = json.IndexOfAny(new[] { JObjStart, JArrayStart });
                    if (firstCharPos > -1)
                    {
                        var firstChar = json[firstCharPos];
                        if (firstChar == JObjStart)
                            return JObject.Parse(json);
                        if (firstChar == JArrayStart)
                            return JArray.Parse(json);
                    }
                }
                catch
                {
                    if (fallback == JsonErrorCode) throw;
                }

            // fallback
            return fallback == null
                ? null
                : JObject.Parse(fallback);
        }

        [PrivateApi]
        public static object WrapOrUnwrap(object original)
        {
            switch (original)
            {
                case JArray jArray:
                    return new DynamicJacketList(jArray);
                case JObject jResult: // it's another complex object, so return another wrapped reader
                    return new Sxc.Data.DynamicJacket(jResult);
                case JValue jValue: // it's a simple value - so we want to return the underlying real value
                    return jValue.Value;
                default: // it's something else, let's just return that
                    return original;
            }
        }

    }
}
