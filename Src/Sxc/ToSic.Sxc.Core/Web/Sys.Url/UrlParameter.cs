namespace ToSic.Sxc.Web.Sys.Url;

[ShowApiWhenReleased(ShowApiMode.Never)]
public record UrlParameter(
    string? Name,
    object? Value)
{
    public string? Prefix { get; init; }

    public string FullName => Prefix + Name;
}
