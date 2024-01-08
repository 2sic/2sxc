using ToSic.Eav.Internal.Unknown;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Context;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class PageUnknown: IPage, IIsUnknown
{
    public PageUnknown(WarnUseOfUnknown<PageUnknown> _) { }

    public IPage Init(int id)
    {
        Id = id;
        return this;
    }
        
    public int Id { get; private set; } = Eav.Constants.NullId;

    public string Url => Eav.Constants.UrlNotInitialized;

    public IParameters Parameters => new Parameters.Parameters(null);

}