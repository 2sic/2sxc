using ToSic.Eav.Sys;
using ToSic.Lib.Services;
using ToSic.Sxc.Context.Internal;

#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Context.Sys.Page;

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