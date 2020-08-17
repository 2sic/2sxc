using System;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;

namespace ToSic.Sxc.Run
{
    [PrivateApi]
    public interface IEnvironmentConnector: IHasLog<IEnvironmentConnector>
    {

        void SetAppId(IContainer instance, IAppEnvironment env, int? appId, ILog parentLog);

        void SetPreview(int instanceId, Guid previewView);

        void SetContentGroup(int instanceId, bool blockExists, Guid guid);

        void UpdateTitle(Blocks.IBlockBuilder blockBuilder, IEntity titleItem);
    }
}