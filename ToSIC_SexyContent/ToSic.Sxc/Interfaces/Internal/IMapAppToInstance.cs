using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.SexyContent;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Interfaces
{
    internal interface IMapAppToInstance
    {
        int? GetAppIdFromInstance(IInstanceInfo instance, int zoneId);
        void SetAppIdForInstance(IInstanceInfo instance, IAppEnvironment env, int? appId, ILog parentLog);


        void ClearPreviewTemplate(int instanceId);

        void SetPreviewTemplate(int instanceId, Guid previewTemplateGuid);

        void SetContentGroup(int instanceId, bool wasCreated, Guid guid);

        BlockConfiguration GetInstanceContentGroup(BlocksManager cgm, ILog log, int instanceId, int? pageId);

        void UpdateTitle(ICmsBlock cmsInstance, IEntity titleItem);
    }
}