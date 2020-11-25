using ToSic.Eav.Logging;
using ToSic.Eav.LookUp;

namespace ToSic.Sxc.Mvc.NotImplemented
{
    public class NotImplementedGetLookupEngine: HasLog<IGetEngine>, IGetEngine
    {
        public NotImplementedGetLookupEngine() : base("Mvc.LookUp")
        {
        }

        public ILookUpEngine GetEngine(int instanceId)
        {
            // BIG TODO to provide more context, properties etc.
            return new LookUpEngine(Log);
        }
    }
}
