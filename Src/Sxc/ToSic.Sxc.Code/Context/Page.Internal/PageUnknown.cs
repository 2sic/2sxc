using ToSic.Eav.Sys;
using ToSic.Lib.Services;

#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Context.Internal;

internal class PageUnknown(WarnUseOfUnknown<PageUnknown> _) : IPage, IIsUnknown
{
    public IPage Init(int id)
    {
        Id = id;
        return this;
    }
        
    public int Id { get; private set; } = EavConstants.NullId;

    public string Url => EavConstants.UrlNotInitialized;

    public IParameters Parameters => new Parameters();

}