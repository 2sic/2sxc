using ToSic.Lib.Services;

#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Integration.Paths;

internal class LinkPathsUnknown(WarnUseOfUnknown<LinkPathsUnknown> _) : ILinkPaths, IIsUnknown
{
    public string AsSeenFromTheDomainRoot(string virtualPath) => throw new NotImplementedException();

    // Stub CurrentPage
    public string GetCurrentRequestUrl() => GetCurrentLinkRoot() + "/folder/sub-folder/current-page";

    // Stub DomainName
    public string GetCurrentLinkRoot() => "https://unknown.2sxc.org";
}