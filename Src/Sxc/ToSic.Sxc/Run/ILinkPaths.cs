namespace ToSic.Sxc.Run;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface ILinkPaths
{
    string AsSeenFromTheDomainRoot(string virtualPath);

    string GetCurrentRequestUrl();

    string GetCurrentLinkRoot();
}