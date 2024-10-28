using System.Collections;
using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Web.Internal.Url;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ObjectToUrl
{
    public ObjectToUrl() { }

    internal ObjectToUrl(string prefix = null, IEnumerable<UrlValueProcess> preProcessors = null): this()
    {
        Prefix = prefix;
        _preProcessors = preProcessors;
    }

    private readonly IEnumerable<UrlValueProcess> _preProcessors;

    private string Prefix { get; }

    public string ArrayBoxStart { get; set; } = "";
    public string ArrayBoxEnd { get; set; } = "";
    public string ArraySeparator { get; set; } = ",";
    public string DepthSeparator { get; set; } = ":";
    public string PairSeparator { get; set; } = UrlParts.ValuePairSeparator.ToString();

    public string KeyValueSeparator { get; set; } = "=";


    public string Serialize(object data) => Serialize(data, Prefix);

    public string SerializeChild(object child, string prefix) => SerializeWithChild(null, child, prefix);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="main"></param>
    /// <param name="child"></param>
    /// <param name="childPrefix">Prefix to use for the child - it is not the same as the Prefix of the main object! as that applies to all data, not child-data</param>
    /// <returns></returns>
    public string SerializeWithChild(object main, object child, string childPrefix = null)
    {
        var asString = Serialize(main);
        if (child == null) return asString;
        childPrefix ??= ""; // null catch
        var prefillAddOn = "";
        if (child is string strPrefill)
        {
            var parts = strPrefill.Split(UrlParts.ValuePairSeparator)
                .Where(p => p.HasValue())
                .Select(p => p.StartsWith(childPrefix) ? p : childPrefix + p);
            prefillAddOn = string.Join(UrlParts.ValuePairSeparator.ToString(), parts);
        }
        else
            prefillAddOn = Serialize(child, childPrefix);

        return UrlParts.ConnectParameters(asString, prefillAddOn);
    }

    private UrlValuePair ValueSerialize(NameObjectSet set)
    {
        if (_preProcessors.SafeAny())
            foreach (var pP in _preProcessors)
            {
                set = pP.Process(set);
                if (!set.Keep) return null;
            }

        if (set.Value == null) return null;
        if (set.Value is string strValue) return new(set.FullName, strValue);

        var valueType = set.Value.GetType();

        // Check array - not sure yet if we care
        if (set.Value is IEnumerable enumerable)
        {
            var isGeneric = valueType.IsGenericType;
            var valueElemType = isGeneric
                ? valueType.GetGenericArguments()[0]
                : valueType.GetElementType();

            if (valueElemType == null)
                throw new ArgumentNullException($"The field: '{set.FullName}', isGeneric: {isGeneric} with base type {valueType} to add to url seems to have a confusing setup");

            if (valueElemType.IsPrimitive || valueElemType == typeof(string))
                return new(set.FullName, $"{ArrayBoxStart}{string.Join(ArraySeparator, enumerable.Cast<object>())}{ArrayBoxEnd}");

            return new(set.FullName, "array-like-but-unclear-what");
        }

        return valueType.IsSimpleType()
            // Simple type - just serialize, except for bool, which should be lower-cased
            ? new(set.FullName,
                set.Value is bool ? set.Value.ToString().ToLowerInvariant() : set.Value.ToString())
            // Complex object, recursive serialize with current name as prefix
            : new UrlValuePair(null, Serialize(set.Value, set.FullName + DepthSeparator), true);
    }

    private string Serialize(object data, string prefix)
    {
        // Case #1: Null, return that
        if (data == null) return null;

        // Case #2: Already a string, return that
        if (data is string str) return str;

        // Case #3: It's an object or an array of objects (but not a string)
        IEnumerable objectList = data is IEnumerable dataAsEnum
            ? dataAsEnum
            : new[] { data };

        // Get all properties on the object
        var properties = objectList
            .Cast<object>()
            .SelectMany(d => PropsOfOne(d, prefix) ?? [])
            .Where(d => d != null)
            .ToList();

        // Concat all key/value pairs into a string separated by ampersand
        return string.Join(PairSeparator, properties.Select(p => p.ToString()));

    }

    // https://ole.michelsen.dk/blog/serialize-object-into-a-query-string-with-reflection/
    // https://stackoverflow.com/questions/6848296/how-do-i-serialize-an-object-into-query-string-format
    private List<UrlValuePair> PropsOfOne(object data, string prefix)
    {
        // Case #1: Null, return that
        if (data == null) return null;

        // Case #2: Already a string, return that
        if (data is string str)
            return str.HasValue()
                ? [new(null, str, true)]
                : null;

        // Case #3: It's an object or an array of objects (but not a string)
        // Get all properties on the object
        var properties = data.GetType().GetProperties()
            .Where(x => x.CanRead)
            .Select(x =>
            {
                // Check if it's a key value pair (from a dictionary) - like on note
                var kvpPair = PropOfKvp(data);
                var preSerialize = kvpPair != null
                    ? new(kvpPair.Name, kvpPair.Value, prefix)
                    : new NameObjectSet(x.Name, x.GetValue(data, null), prefix);
                return ValueSerialize(preSerialize);
            })
            .Where(x => x?.Value != null)
            .ToList();

        // Concat all key/value pairs into a string separated by ampersand
        return properties;

    }

    // https://stackoverflow.com/questions/2729614/c-sharp-reflection-how-can-i-tell-if-object-o-is-of-type-keyvaluepair-and-then
    private NameObjectSet PropOfKvp(object value)
    {
        if (value == null) return null;
        var valueType = value.GetType();
        if (!valueType.IsGenericType) return null;
            
        var baseType = valueType.GetGenericTypeDefinition();
        if (baseType != typeof(KeyValuePair<,>)) return null;
            
        //var argTypes = baseType.GetGenericArguments();
        // now process the values
        if (valueType.GetProperty("Key")?.GetValue(value, null) is not string kvpKey)
            return null;

        return valueType.GetProperty("Value")?.GetValue(value, null) is not object kvpValue 
            ? null 
            : new NameObjectSet(name: kvpKey, value: kvpValue);
    }
}