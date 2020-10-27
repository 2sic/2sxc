using System;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Oqt.Shared.Dev;
using ToSic.Sxc.Run;

namespace ToSic.Sxc.Oqt.Server.Run
{
    internal class OqtaneModuleUpdater: HasLog, IPlatformModuleUpdater
    {
        /// <summary>
        /// Empty constructor for DI
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public OqtaneModuleUpdater() : base("Mvc.MapA2I") { }


        public IPlatformModuleUpdater Init(ILog parent)
        {
            Log.LinkTo(parent);
            return this;
        }


        public void SetAppId(IContainer instance, int? appId, ILog parentLog) => throw new NotImplementedException();

        public void SetPreview(int instanceId, Guid previewTemplateGuid) => throw new NotImplementedException();

        public void SetContentGroup(int instanceId, bool wasCreated, Guid guid) => throw new NotImplementedException();

        public void UpdateTitle(IBlock block, IEntity titleItem) => WipConstants.DontDoAnythingImplementLater();
    }
}
