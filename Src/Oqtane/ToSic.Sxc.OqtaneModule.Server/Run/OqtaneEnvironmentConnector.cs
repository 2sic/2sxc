using System;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Run;

namespace ToSic.Sxc.OqtaneModule.Server.Run
{
    internal class OqtaneEnvironmentConnector: HasLog, IEnvironmentConnector
    {
        /// <summary>
        /// Empty constructor for DI
        /// </summary>
        // ReSharper disable once UnusedMember.Global
        public OqtaneEnvironmentConnector() : base("Mvc.MapA2I") { }


        public IEnvironmentConnector Init(ILog parent)
        {
            Log.LinkTo(parent);
            return this;
        }


        public void SetAppId(IContainer instance, int? appId, ILog parentLog) => throw new NotImplementedException();

        public void SetPreview(int instanceId, Guid previewTemplateGuid) => throw new NotImplementedException();

        public void SetContentGroup(int instanceId, bool wasCreated, Guid guid) => throw new NotImplementedException();

        public void UpdateTitle(IBlock block, IEntity titleItem) => throw new NotImplementedException();
    }
}
