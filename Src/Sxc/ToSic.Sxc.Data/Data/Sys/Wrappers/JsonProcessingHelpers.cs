using System.Text.Json;
using System.Text.Json.Nodes;
using static ToSic.Eav.Serialization.Sys.Json.JsonOptions;
using static ToSic.Sxc.Data.Sys.Wrappers.WrapperConstants;

namespace ToSic.Sxc.Data.Sys.Wrappers;

internal class JsonProcessingHelpers
{
    internal static JsonNode? AsJsonNode(string? json, string? fallback = EmptyJson)
    {
        if (!json.HasValue())
            return fallback == null
                ? null
                : StandardParse(fallback);

        try
        {
            var (isComplex, _) = AnalyzeJson(json);
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

        JsonNode? StandardParse(string jsonToParse)
            => JsonNode.Parse(jsonToParse, JsonNodeDefaultOptions, JsonDocumentDefaultOptions);
    }

    /// <summary>
    /// Make sure a JsonNode which is a JValue containing an Array or Object is correctly packages
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    public static (JsonNode? Node, bool Repackaged) NeutralizeValueToObjectOrArray(JsonNode node)
    {
        if (node is not JsonValue jValue)
            return (node, false);
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


    public static object? JsonValueGetContents(JsonNode? jValue)
    {
        if (jValue == null)
            return null;

        var je = jValue.GetValue<JsonElement>();
        return je.ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            JsonValueKind.Number when je.TryGetInt32(out var intValue) => intValue,
            JsonValueKind.Number when je.TryGetInt64(out var longValue) => longValue,
            JsonValueKind.Number => je.GetDouble(),
            JsonValueKind.String when je.TryGetDateTime(out var dateTime) => dateTime,
            JsonValueKind.String => je.GetString(),
            JsonValueKind.Null or JsonValueKind.Undefined => null,
            //case JsonValueKind.Object: return new DynamicJacket(JsonObject.Create(je));
            //case JsonValueKind.Array: return new DynamicJacketList(JsonArray.Create(je));
            _ => jValue.AsValue()
        };
    }

    //private (object Final, bool Ok, JsonValueKind ValueKind) IfJsonTryConvertToExpected(object original) =>
    //    Settings.WrapToDynamic
    //        ? ((object Final, bool Ok, JsonValueKind ValueKind))IfJsonTryConvertToJacket(original)
    //        : IfJsonTryConvertToJacket<ITyped>(original, CreateTypedObject, CreateTypedList);
}