using System;
using ToSic.Eav.Internal.Unknown;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Run;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class LinkPathsUnknown(WarnUseOfUnknown<LinkServiceUnknown> _) : ILinkPaths, IIsUnknown
{
    public string AsSeenFromTheDomainRoot(string virtualPath) => throw new NotImplementedException();

    // Stub CurrentPage
    public string GetCurrentRequestUrl() => LinkServiceUnknown.NiceCurrentUrl;

    // Stub DomainName
    public string GetCurrentLinkRoot() => LinkServiceUnknown.DefRoot;
}