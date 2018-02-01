using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Logging.Simple;

namespace ToSic.SexyContent.Interfaces
{
    public interface IMapAppToInstance
    {
        int? GetAppIdFromInstance(IInstanceInfo instance, int zoneId);
        void SetAppIdForInstance(IInstanceInfo instance, IEnvironment env, int? appId, Log parentLog);
    }
}