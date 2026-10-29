using ToSic.Eav.Sys;

#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Context.Sys.Page;

internal class PageUnknown(WarnUseOfUnknown<PageUnknown> _) : IPage, IIsUnknown
{
    public int Id { get; private set; } = EavConstants.NullId;

    public string Url => EavConstants.UrlNotInitialized;

    public IParameters Parameters => new Parameters();

}