using ToSic.Sxc.Cms.Publishing.Sys;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Cms;

internal class OqtModulePublishing()
    : ServiceBase($"{OqtConstants.OqtLogPrefix}.Publsh"),
    IModulePublishing
{

    public int GetLatestVersion(int instanceId) => 0;

    public int GetPublishedVersion(int instanceId) => 0;


    public void Publish(int instanceId, int version) 
        => Log.A($"Publish(m:{instanceId}, v:{version}) - not supported in Oqtane, publish never happened ");
}
