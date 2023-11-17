using System;
using ToSic.Eav.Data;
using ToSic.Eav.Internal.Unknown;
using ToSic.Lib.Logging;
using ToSic.Eav.Run.Unknown;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;


namespace ToSic.Sxc.Run
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    internal class BasicModuleUpdater: ServiceBase, IPlatformModuleUpdater
    {
        /// <summary>
        /// Empty constructor for DI
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public BasicModuleUpdater(WarnUseOfUnknown<BasicModuleUpdater> _) : base($"{LogScopes.NotImplemented}.MapA2I") { }


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
