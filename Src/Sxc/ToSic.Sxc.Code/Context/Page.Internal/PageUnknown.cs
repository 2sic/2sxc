using ToSic.Eav.Internal.Unknown;
#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Context.Internal;

internal class PageUnknown(WarnUseOfUnknown<PageUnknown> _) : IPage, IIsUnknown
{
    public IPage Init(int id)
    {
        Id = id;
        return this;
    }
        
    public int Id { get; private set; } = Eav.Constants.NullId;

    public string Url => Eav.Constants.UrlNotInitialized;

    public IParameters Parameters => new Parameters();

}