namespace ToSic.Sxc.Run
{
    public interface ILinkPaths
    {
        string AsSeenFromTheDomainRoot(string virtualPath);

        string GetCurrentRequestUrl();
    }
}
