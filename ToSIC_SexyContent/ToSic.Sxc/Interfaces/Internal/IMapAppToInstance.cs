using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;
using ICmsBlock = ToSic.Eav.Apps.Blocks.ICmsBlock;

namespace ToSic.Sxc.Interfaces
{
    internal interface IMapAppToInstance
    {
        int? GetAppIdFromInstance(ICmsBlock instance, int zoneId);
        void SetAppIdForInstance(ICmsBlock instance, IAppEnvironment env, int? appId, ILog parentLog);


        void ClearPreviewTemplate(int instanceId);

        void SetPreviewTemplate(int instanceId, Guid previewTemplateGuid);

        void SetContentGroup(int instanceId, bool wasCreated, Guid guid);

        BlockConfiguration GetInstanceContentGroup(BlocksRuntime cgm, ILog log, int instanceId, int? pageId);

        void UpdateTitle(Blocks.ICmsBlock cmsInstance, IEntity titleItem);
    }
}