using ToSic.Eav.Logging;
using ToSic.Eav.LookUp;

namespace ToSic.Sxc.Mvc.Run
{
    public class MvcGetLookupEngine: IGetEngine
    {
        public ILookUpEngine GetEngine(int instanceId, ILog parentLog)
        {
            // BIG TODO to provide more context, properties etc.
            return new LookUpEngine(parentLog);
        }
    }
}
