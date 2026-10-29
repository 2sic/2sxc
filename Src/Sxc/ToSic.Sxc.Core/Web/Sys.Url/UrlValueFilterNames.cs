namespace ToSic.Sxc.Web.Sys.Url;

/// <summary>
/// Helper to process url values - and keep or skip certain properties.
/// Note that it is case-insensitive
/// </summary>
internal class UrlValueFilterNames: IUrlParameterInGroupOperation
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


    public UrlParameter? Process(UrlParameter? set)
    {
        if (set?.Name == null)
            return null;

        var keep = PropSerializeMap.TryGetValue(set.Name, out var reallyUse)
            ? reallyUse
            : PropSerializeDefault;

        return keep ? set : null;
    }
}