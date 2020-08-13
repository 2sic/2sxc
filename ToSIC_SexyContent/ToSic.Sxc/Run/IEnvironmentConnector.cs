using System;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Run
{
    internal interface IEnvironmentConnector
    {
        // 2020-08-13 2dm - remove in October
        //int? GetAppIdFromInstance(IContainer instance, int zoneId);

        void SetAppIdForInstance(IContainer instance, IAppEnvironment env, int? appId, ILog parentLog);

        // 2020-08-11 disabled, doesn't seem to be needed outside of code
        //void ClearPreviewTemplate(int instanceId);

        void SetPreviewTemplate(int instanceId, Guid previewTemplateGuid);

        void SetContentGroup(int instanceId, bool wasCreated, Guid guid);

        // 2020-08-13 2dm - remove in October
        //BlockConfiguration GetInstanceContentGroup(BlocksRuntime cgm, IContainer instance, ILog log); // int instanceId, int? pageId);

        void UpdateTitle(Blocks.IBlockBuilder blockBuilder, IEntity titleItem);
    }
}