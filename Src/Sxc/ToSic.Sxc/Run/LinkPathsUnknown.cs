using System;
using ToSic.Eav.Internal.Unknown;
using ToSic.Eav.Run;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Run;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class LinkPathsUnknown : ILinkPaths, IIsUnknown
{
    public LinkPathsUnknown(WarnUseOfUnknown<LinkServiceUnknown> _)
    {
            
    }
        
    public string AsSeenFromTheDomainRoot(string virtualPath)
    {
        throw new NotImplementedException();
    }

    // Stub CurrentPage
    public string GetCurrentRequestUrl() => LinkServiceUnknown.NiceCurrentUrl;

    // Stub DomainName
    public string GetCurrentLinkRoot() => LinkServiceUnknown.DefRoot;
}