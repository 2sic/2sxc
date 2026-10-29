namespace ToSic.Sxc.Web.Sys.Url;

/// <summary>
/// Describes an url parameter property.
/// Since there are various ways it can be made before assembling the final url, there is an interface
/// so that each combination works by itself.
/// </summary>
[InternalApi_DoNotUse_MayChangeWithoutNotice]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IUrlParam
{
    public string GetSerialized(ObjectToUrlOptions options);
}