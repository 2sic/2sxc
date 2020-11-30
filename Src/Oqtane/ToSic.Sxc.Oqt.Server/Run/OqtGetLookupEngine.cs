using ToSic.Eav.Logging;
using ToSic.Eav.LookUp;
using ToSic.Sxc.Oqt.Shared;

// TODO: #Oqtane - must provide additional sources like Context (http) etc.

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtGetLookupEngine: HasLog<ILookUpEngineResolver>, ILookUpEngineResolver
    {
        public OqtGetLookupEngine() : base($"{OqtConstants.OqtLogPrefix}.LookUp")
        {
        }

        public ILookUpEngine GetLookUpEngine(int instanceId/*, ILog parentLog*/)
        {
            return new LookUpEngine(Log);
        }
    }
}
