using ToSic.Eav.Plumbing;

namespace ToSic.Sxc.Web.Internal.Url;

/// <summary>
/// Helper to process url values - and keep or skip certain properties.
/// Note that it is case-insensitive
/// </summary>
internal class UrlValueCamelCase : UrlValueProcess
{
    public override NameObjectSet Process(NameObjectSet set)
    {
        if (set == null || !set.Name.HasValue()) return set;
        var firstCharLower = char.ToLowerInvariant(set.Name[0]);
        if (firstCharLower == set.Name[0]) return set;
        var newName = firstCharLower + set.Name.Substring(1);
        return new(set, newName);
    }
}