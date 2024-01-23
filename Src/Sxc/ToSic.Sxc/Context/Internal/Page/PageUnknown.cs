using ToSic.Eav.Internal.Unknown;

namespace ToSic.Sxc.Context.Internal;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class PageUnknown(WarnUseOfUnknown<PageUnknown> _) : IPage, IIsUnknown
{
    public IPage Init(int id)
    {
        Id = id;
        return this;
    }
        
    public int Id { get; private set; } = Eav.Constants.NullId;

    public string Url => Eav.Constants.UrlNotInitialized;

    public IParameters Parameters => new Parameters(null);

}