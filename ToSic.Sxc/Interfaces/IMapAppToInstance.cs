using System;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Logging.Simple;

namespace ToSic.SexyContent.Interfaces
{
    public interface IMapAppToInstance
    {
        int? GetAppIdFromInstance(IInstanceInfo instance, int zoneId);
        void SetAppIdForInstance(IInstanceInfo instance, IAppEnvironment env, int? appId, Log parentLog);


        void ClearPreviewTemplate(int instanceId);

        void SetPreviewTemplate(int instanceId, Guid previewTemplateGuid);

        void SetContentGroup(int instanceId, bool wasCreated, Guid guid);

        ContentGroup GetInstanceContentGroup(ContentGroupManager cgm, Log log, int instanceId, int? pageId);

        void UpdateTitle(SxcInstance sxcInstance, Eav.Interfaces.IEntity titleItem);
    }
}