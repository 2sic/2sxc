using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Interfaces
{
    internal interface IMapAppToInstance
    {
        int? GetAppIdFromInstance(IContainer instance, int zoneId);
        void SetAppIdForInstance(IContainer instance, IAppEnvironment env, int? appId, ILog parentLog);


        void ClearPreviewTemplate(int instanceId);

        void SetPreviewTemplate(int instanceId, Guid previewTemplateGuid);

        void SetContentGroup(int instanceId, bool wasCreated, Guid guid);

        BlockConfiguration GetInstanceContentGroup(BlocksRuntime cgm, ILog log, int instanceId, int? pageId);

        void UpdateTitle(Blocks.IBlockBuilder blockBuilder, IEntity titleItem);
    }
}