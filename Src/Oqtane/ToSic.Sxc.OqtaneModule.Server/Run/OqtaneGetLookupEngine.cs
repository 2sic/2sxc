using ToSic.Eav.Logging;
using ToSic.Eav.LookUp;

// TODO: #Oqtane - must provide additional sources like Context (http) etc.

namespace ToSic.Sxc.OqtaneModule.Server.Run
{
    public class OqtaneGetLookupEngine: IGetEngine
    {
        public ILookUpEngine GetEngine(int instanceId, ILog parentLog)
        {
            return new LookUpEngine(parentLog);
        }
    }
}
