using System;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Run;

namespace ToSic.Sxc.Mvc.Run
{
    internal class MvcModuleUpdater: HasLog, IPlatformModuleUpdater
    {
        /// <summary>
        /// Empty constructor for DI
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public MvcModuleUpdater() : base("Mvc.MapA2I") { }


        public IPlatformModuleUpdater Init(ILog parent)
        {
            Log.LinkTo(parent);
            return this;
        }


        public void SetAppId(IContainer instance, int? appId) => throw new NotImplementedException();

        public void SetPreview(int instanceId, Guid previewTemplateGuid) => throw new NotImplementedException();

        public void SetContentGroup(int instanceId, bool wasCreated, Guid guid) => throw new NotImplementedException();

        public void UpdateTitle(IBlock block, IEntity titleItem) => throw new NotImplementedException();
    }
}
