namespace ToSic.Sxc.Web.Sys.PageServiceShared;

public record UrlParameterSpecs(string Name, IEnumerable<string>? Values = null, bool ForceNameLowerCase = true, bool ValuesCaseVariationAllowed = false)
{
    public IEnumerable<string> Values { get; init; } = Values ?? [];
}
