using ToSic.Eav.Logging;
using ToSic.Eav.LookUp;

// TODO: #Oqtane - must provide additional sources like Context (http) etc.

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtGetLookupEngine: IGetEngine
    {
        public ILookUpEngine GetEngine(int instanceId, ILog parentLog)
        {
            return new LookUpEngine(parentLog);
        }
    }
}
