using System.Collections;
using System.Diagnostics.CodeAnalysis;
using ToSic.Sys.Performance;
using ToSic.Sys.Utils;
using ToSic.Sys.Utils.Types;

namespace ToSic.Sxc.Web.Sys.Url;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ObjectToUrl
{
    public ObjectToUrlOptions MyOptions { get; init; } = new();

    public string? Serialize(object? data)
        => Serialize(data, prefix: MyOptions.Prefix);

    public string? SerializeChild(object child, string? prefix)
        => SerializeWithChild(null, child, prefix);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="main"></param>
    /// <param name="child"></param>
    /// <param name="childPrefix">Prefix to use for the child - it is not the same as the Prefix of the main object! as that applies to all data, not child-data</param>
    /// <returns></returns>
    public string? SerializeWithChild(object? main, object? child, string? childPrefix = null)
    {
        // Exit early
        if (main == null && child == null)
            return null;

        var asString = Serialize(main, prefix: null);
        if (child == null)
            return asString;

        childPrefix ??= ""; // null catch
        string prefillAddOn;
        if (child is string strPrefill)
        {
            var parts = strPrefill
                .Split(UrlParts.ValuePairSeparator)
                .Where(p => p.HasValue())
                .Select(p => p.StartsWith(childPrefix) ? p : childPrefix + p);
            prefillAddOn = string.Join(MyOptions.PairSeparator, parts);
        }
        else
            prefillAddOn = Serialize(child, childPrefix);

        return UrlParts.ConnectParameters(asString, prefillAddOn);
    }


    [return: NotNullIfNotNull(nameof(data))]
    private string? Serialize(object? data, string? prefix)
    {
        switch (data)
        {
            // Case #1: Null, return that
            case null: return null;
            // Case #2: Already a string, return that
            // check early, as strings also implement IEnumerable, so we need to check for string first
            case string str: return str;
            // Case #3: It's an object or an array of objects (but not a string)
            default:
            {
                var objectList = data as IEnumerable ?? new[] { data };

                // Get all properties on the object
                var properties = objectList
                    .Cast<object>()
                    .Select(d => PropsOfOne(d, prefix))
                    .ToList();

                // Concat all key/value pairs into a string separated by ampersand
                return string.Join(MyOptions.PairSeparator, properties); //.Select(p => p.GetSerialized(MyOptions)));
            }
        }
    }

    // https://ole.michelsen.dk/blog/serialize-object-into-a-query-string-with-reflection/
    // https://stackoverflow.com/questions/6848296/how-do-i-serialize-an-object-into-query-string-format
    //private IEnumerable<IUrlParam> PropsOfOne(object data, string? prefix) =>
    private string? PropsOfOne(object data, string? prefix) =>
        data switch
        {
            // Case #1: Null, return that; should never happen
            //null => [],
            // Case #2: Already a string, return that
            string str => str.HasValue()
                ? new UrlParamPrepared(str).GetSerialized(MyOptions)
                : null,
            // Case #3: It's an object or an array of objects (but not a string)
            _ => string.Join(
                MyOptions.PairSeparator,
                data.GetType()
                    // Get all properties on the object
                    .GetProperties()
                    .Where(x => x.CanRead)
                    .Select(x => ValueSerialize(new(x.Name, x.GetValue(data, null)) { Prefix = prefix }))
                    .OfType<IUrlParam>()
                    .ToListOpt()
            )
        };

    private IUrlParam? ValueSerialize(UrlParameter set)
    {
        foreach (var pP in MyOptions.PreProcessors ?? [])
        {
            var setOrNull = pP.Process(set);
            if (setOrNull == null)
                return null;
            set = setOrNull;
        }

        switch (set.Value)
        {
            case null: return null;
            case string strValue: return new UrlParamKvp(set.FullName, strValue);
        }

        var valueType = set.Value.GetType();

        // Check array - not sure yet if we care
        if (set.Value is IEnumerable enumerable)
        {
            var isGeneric = valueType.IsGenericType;
            var valueElemType = isGeneric
                ? valueType.GetGenericArguments()[0]
                : valueType.GetElementType();

            // If no match, this is so unexpected that we better throw, so the developer notices early during development
            if (valueElemType == null)
                throw new ArgumentNullException($"The field: '{set.FullName}', isGeneric: {isGeneric} with base type {valueType} to add to url seems to have a confusing setup");

            // It's an IEnumerable of primitive types or strings, so we can serialize it as a single string with a separator
            if (valueElemType.IsPrimitive || valueElemType == typeof(string))
                return new UrlParamKvp(set.FullName, $"{MyOptions.ArrayBoxStart}{string.Join(MyOptions.ArraySeparator, enumerable.Cast<object>())}{MyOptions.ArrayBoxEnd}");

            return new UrlParamKvp(set.FullName, "array-like-but-unclear-what");
        }

        if (valueType.IsSimpleType())
            return new UrlParamKvp(set.FullName, set.Value is bool bln
                ? bln ? "true" : "false"
                : set.Value.ToString()
            );

        var maybeValue = Serialize(set.Value, prefix: $"{set.FullName}{MyOptions.DepthSeparator}");
        return new UrlParamPrepared(maybeValue);
    }
}

// #DropObjectToUrlKeyValuePairProperty - Clean up in 2027

// This was the code in the select above...
// Check if it's a key value pair (from a dictionary) - like on note
//var kvpPairOrNull = UrlParameterFromObjectKvp.Process(data);
//var preSerialize = kvpPairOrNull != null
//    ? kvpPairOrNull with { Prefix = prefix }
//    : new(x.Name, x.GetValue(data, null)) { Prefix = prefix };
//return ValueSerialize(preSerialize);


// Note: This was introduced 2023-04-06 on commit dd117de4
// At that time, we were passing a dictionary into the system for notes
// since then we switched to an anonymous object instead
// so as of 2026-09-25 this looks completely unused.
// It also looks like the implementation was a bit buggy TBH,
// as the loop calling it seems to loop through properties, and then jump back to serializing the
// main object - which is very odd. 
// I will disable it for now, leave in till ca. 2027
// in case it was used elsewhere
//internal class UrlParameterFromObjectKvp
//{
//    // https://stackoverflow.com/questions/2729614/c-sharp-reflection-how-can-i-tell-if-object-o-is-of-type-keyvaluepair-and-then
//    public static UrlParameterInGroup? Process(object? value)
//    {
//        if (value == null)
//            return null;
//        var valueType = value.GetType();
//        if (!valueType.IsGenericType)
//            return null;

//        var baseType = valueType.GetGenericTypeDefinition();
//        if (baseType != typeof(KeyValuePair<,>))
//            return null;

//        //var argTypes = baseType.GetGenericArguments();
//        // now process the values
//        if (valueType.GetProperty("Key")?.GetValue(value, null) is not string kvpKey)
//            return null;

//        return valueType.GetProperty("Value")?.GetValue(value, null) is not { } kvpValue
//            ? null
//            : new UrlParameterInGroup(Name: kvpKey, Value: kvpValue);
//    }
//}