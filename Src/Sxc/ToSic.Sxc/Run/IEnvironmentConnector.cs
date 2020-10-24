using System;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Run
{
    // Todo#Rename - something like ISaveToEnvironment
    [PrivateApi]
    public interface IEnvironmentConnector: IHasLog<IEnvironmentConnector>
    {

        void SetAppId(IContainer instance, int? appId, ILog parentLog);

        void SetPreview(int instanceId, Guid previewView);

        void SetContentGroup(int instanceId, bool blockExists, Guid guid);

        void UpdateTitle(IBlock block, IEntity titleItem);
    }
}