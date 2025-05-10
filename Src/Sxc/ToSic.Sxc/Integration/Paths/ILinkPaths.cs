namespace ToSic.Sxc.Integration.Paths;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface ILinkPaths
{
    string AsSeenFromTheDomainRoot(string virtualPath);

    string GetCurrentRequestUrl();

    string GetCurrentLinkRoot();
}