using System;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Run
{
    internal interface IEnvironmentConnector
    {
        void SetAppIdForInstance(IContainer instance, IAppEnvironment env, int? appId, ILog parentLog);

        void SetPreviewTemplate(int instanceId, Guid previewTemplateGuid);

        void SetContentGroup(int instanceId, bool wasCreated, Guid guid);

        void UpdateTitle(Blocks.IBlockBuilder blockBuilder, IEntity titleItem);
    }
}