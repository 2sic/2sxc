using ToSic.Sys.Utils;

namespace ToSic.Sxc.Web.Sys.Url;

/// <summary>
/// Helper to process url values - and keep or skip certain properties.
/// Note that it is case-insensitive
/// </summary>
internal class UrlValueCamelCase : IUrlParameterInGroupOperation
{
    public UrlParameter? Process(UrlParameter? set)
    {
        // Nothing or empty name; return original
        var name = set?.Name;
        if (set == null || name.IsEmptyOrWs())
            return set;

        // If first char is already lower case, return original.
        var firstCharLower = char.ToLowerInvariant(name[0]);
        if (firstCharLower == name[0])
            return set;

        // Return set with first char lower case
        var newName = firstCharLower + name.Substring(1);
        return set with { Name = newName };
    }
}