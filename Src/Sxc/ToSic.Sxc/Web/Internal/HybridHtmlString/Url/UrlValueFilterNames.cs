namespace ToSic.Sxc.Web.Internal.Url;

/// <summary>
/// Helper to process url values - and keep or skip certain properties.
/// Note that it is case-insensitive
/// </summary>
internal class UrlValueFilterNames: UrlValueProcess
{
    /// <summary>
    /// Determine names of properties to preserve in the final parameters
    /// </summary>
    /// <param name="defaultSerialize"></param>
    /// <param name="opposite"></param>
    public UrlValueFilterNames(bool defaultSerialize, IEnumerable<string> opposite)
    {
        PropSerializeDefault = defaultSerialize;
        foreach (var sProp in opposite)
            PropSerializeMap[sProp] = !PropSerializeDefault;
    }

    /// <summary>
    /// Determine if not-found properties should be preserved or not - default is preserve, but init can reverse this
    /// </summary>
    internal bool PropSerializeDefault;
    internal Dictionary<string, bool> PropSerializeMap = new(StringComparer.InvariantCultureIgnoreCase);


    public override NameObjectSet Process(NameObjectSet set)
    {
        return PropSerializeMap.TryGetValue(set.Name, out var reallyUse)
            ? new(set, keep: reallyUse) 
            : new NameObjectSet(set, keep: PropSerializeDefault); 
    }
}