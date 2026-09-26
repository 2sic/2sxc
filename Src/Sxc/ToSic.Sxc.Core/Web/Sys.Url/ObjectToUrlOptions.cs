namespace ToSic.Sxc.Web.Sys.Url;

public record ObjectToUrlOptions
{
    public string ArrayBoxStart { get; set; } = "";
    public string ArrayBoxEnd { get; set; } = "";
    public string ArraySeparator { get; set; } = ",";
    public string DepthSeparator { get; set; } = ":";
    public string PairSeparator { get; set; } = UrlParts.ValuePairSeparator.ToString();

    public string KeyValueSeparator { get; set; } = "=";

    public string? Prefix { get; init; }
    internal IEnumerable<IUrlParameterInGroupOperation>? PreProcessors { get; init; }

}