using System.Text.Json;
using System.Text.Json.Nodes;
using ToSic.Eav.Plumbing;
using static ToSic.Eav.Serialization.JsonOptions;
using static ToSic.Sxc.Data.Internal.Wrapper.WrapperConstants;

namespace ToSic.Sxc.Data.Internal.Wrapper;

internal class JsonProcessingHelpers
{
    internal static JsonNode AsJsonNode(string json, string fallback = EmptyJson)
    {
        JsonNode StandardParse(string jsonToParse) =>
            JsonNode.Parse(jsonToParse, JsonNodeDefaultOptions, JsonDocumentDefaultOptions);

        if (!json.HasValue()) return fallback == null ? null : StandardParse(fallback);

        try
        {
            var (isComplex, isArray) = AnalyzeJson(json);
            if (isComplex)
            {
                var node = StandardParse(json);
                if (node != null) return node;
            }
        }
        catch
        {
            if (fallback == JsonErrorCode) throw;
        }

        // fallback
        return fallback == null ? null : StandardParse(fallback);
    }

    /// <summary>
    /// Make sure a JsonNode which is a JValue containing an Array or Object is correctly packages
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static (JsonNode Node, bool Repackaged) NeutralizeValueToObjectOrArray(JsonNode node)
    {
        if (node is not JsonValue jValue) return (node, false);
        var je = jValue.GetValue<JsonElement>();
        switch (je.ValueKind)
        {
            case JsonValueKind.Array: return (JsonArray.Create(je), true);
            case JsonValueKind.Object: return (JsonObject.Create(je), true);
            default: return (node, false);
        }
    }

    /// <summary>
    /// Find out if a string is a complex object (obj/array) and if it's an array.
    /// </summary>
    /// <param name="json"></param>
    /// <returns></returns>
    public static (bool IsComplex, bool IsArray) AnalyzeJson(string json)
    {
        // find first possible opening character
        var firstCharPos = json.IndexOfAny([JObjStart, JArrayStart]);
        return firstCharPos <= -1
            ? (false, false)
            : (true, json[firstCharPos] == JArrayStart);
    }


    public static object JsonValueGetContents(JsonNode jValue)
    {
        var je = jValue.GetValue<JsonElement>();
        switch (je.ValueKind)
        {
            case JsonValueKind.True: return true;
            case JsonValueKind.False: return false;
            case JsonValueKind.Number when je.TryGetInt32(out var intValue): return intValue;
            case JsonValueKind.Number when je.TryGetInt64(out var longValue): return longValue;
            case JsonValueKind.Number: return je.GetDouble();
            case JsonValueKind.String when je.TryGetDateTime(out var dateTime): return dateTime;
            case JsonValueKind.String: return je.GetString();
            case JsonValueKind.Null:
            case JsonValueKind.Undefined: return null;
            //case JsonValueKind.Object: return new DynamicJacket(JsonObject.Create(je));
            //case JsonValueKind.Array: return new DynamicJacketList(JsonArray.Create(je));
            default: return jValue.AsValue();
        }
    }

    //private (object Final, bool Ok, JsonValueKind ValueKind) IfJsonTryConvertToExpected(object original) =>
    //    Settings.WrapToDynamic
    //        ? ((object Final, bool Ok, JsonValueKind ValueKind))IfJsonTryConvertToJacket(original)
    //        : IfJsonTryConvertToJacket<ITyped>(original, CreateTypedObject, CreateTypedList);
}