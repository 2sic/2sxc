using ToSic.Eav.Cms.Internal;
using ToSic.Eav.Context;
using ToSic.Eav.Internal.Unknown;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Cms.Internal.Publishing;

internal class BasicPagePublishing : ServiceBase, IPagePublishing
{
    public BasicPagePublishing(WarnUseOfUnknown<BasicPagePublishing> _) : base($"{LogScopes.NotImplemented}.Publsh") { }

    public void DoInsidePublishing(IContextOfSite context, Action<VersioningActionInfo> action)
    {
        var l = Log.Fn();
        var versioningActionInfo = new VersioningActionInfo();
        action.Invoke(versioningActionInfo);
        l.Done();
    }



    public int GetLatestVersion(int instanceId) => 0;

    public int GetPublishedVersion(int instanceId) => 0;


    public void Publish(int instanceId, int version)
    {
        Log.A($"Publish(m:{instanceId}, v:{version})");
        Log.A("publish never happened ");
    }

}