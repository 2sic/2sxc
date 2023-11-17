using ToSic.Eav.Internal.Unknown;
using ToSic.Eav.Run;
using ToSic.Eav.Run.Unknown;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Context.Query;

namespace ToSic.Sxc.Context
{
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

        public IParameters Parameters => new Parameters(null);

    }
}
