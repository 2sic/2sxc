using System;
using ToSic.Eav;
using ToSic.Eav.Data;
using ToSic.Lib.Logging;
using ToSic.Eav.Run.Unknown;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;


namespace ToSic.Sxc.Run
{
    internal class BasicModuleUpdater: HasLog, IPlatformModuleUpdater
    {
        /// <summary>
        /// Empty constructor for DI
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public BasicModuleUpdater(WarnUseOfUnknown<BasicModuleUpdater> warn) : base($"{LogNames.NotImplemented}.MapA2I") { }


        public void SetAppId(IModule instance, int? appId)
        {
            // do nothing
        }

        public void SetPreview(int instanceId, Guid previewTemplateGuid)
        {
            // do nothing
        }

        public void SetContentGroup(int instanceId, bool wasCreated, Guid guid)
        {
            // do nothing
        }

        public void UpdateTitle(IBlock block, IEntity titleItem)
        {
            // do nothing
        }
    }
}
