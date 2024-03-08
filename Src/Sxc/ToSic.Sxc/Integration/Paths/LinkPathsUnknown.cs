using ToSic.Eav.Internal.Unknown;
using ToSic.Sxc.Services.Internal;
#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Integration.Paths;

internal class LinkPathsUnknown(WarnUseOfUnknown<LinkServiceUnknown> _) : ILinkPaths, IIsUnknown
{
    public string AsSeenFromTheDomainRoot(string virtualPath) => throw new NotImplementedException();

    // Stub CurrentPage
    public string GetCurrentRequestUrl() => LinkServiceUnknown.NiceCurrentUrl;

    // Stub DomainName
    public string GetCurrentLinkRoot() => LinkServiceUnknown.DefRoot;
}