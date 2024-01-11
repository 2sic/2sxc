using ToSic.Eav.Cms.Internal;
using ToSic.Eav.Context;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Cms.Internal.Publishing;

/// <summary>
/// This is the fallback page publishing strategy, which basically says that page publishing isn't enabled
/// NOTE: It is currently not in use, and that's ok. 
/// </summary>
// ReSharper disable once UnusedMember.Global
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class NoPagePublishing() : ServiceBase("Eav.NoPubl"), IPagePublishing
{
    public void DoInsidePublishing(IContextOfSite context, Action<VersioningActionInfo> action)
    {
        var info = new VersioningActionInfo();
        action.Invoke(info);
    }

    //public void DoInsidePublishLatestVersion(int instanceId, Action<VersioningActionInfo> action)
    //{
    //    // NOTE: Do nothing!
    //}

    //public void DoInsideDeleteLatestVersion(int instanceId, Action<VersioningActionInfo> action)
    //{
    //    // NOTE: Do nothing!
    //}

    public int GetLatestVersion(int instanceId)
    {
        return 0;
    }

    public int GetPublishedVersion(int instanceId)
    {
        return 0;
    }

    public void Publish(int instanceId, int version)
    {
        // ignore
    }


}