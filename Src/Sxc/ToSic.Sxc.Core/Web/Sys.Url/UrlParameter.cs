namespace ToSic.Sxc.Web.Sys.Url;

[ShowApiWhenReleased(ShowApiMode.Never)]
public record UrlParameter(string? Name, object? Value): IUrlParam
{
    public string? Prefix { get; init; }

    public string FullName => Prefix + Name;

    public string GetSerialized(ObjectToUrlOptions options)
    {
        throw new NotImplementedException();
    }
}
