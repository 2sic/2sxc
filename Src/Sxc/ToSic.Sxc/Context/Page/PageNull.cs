using System.Collections.Generic;
using ToSic.Eav.Documentation;
using ToSic.Eav.Run;
using ToSic.Eav.Run.Unknown;
using ToSic.Sxc.Context.Query;

namespace ToSic.Sxc.Context
{
    [PrivateApi]
    public class PageUnknown: IPage, IIsUnknown
    {
        public PageUnknown(WarnUseOfUnknown<PageUnknown> warn) { }

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
