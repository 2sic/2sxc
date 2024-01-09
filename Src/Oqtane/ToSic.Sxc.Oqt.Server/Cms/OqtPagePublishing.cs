using System;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Context;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Cms.Internal.Publishing;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Cms;

internal class OqtPagePublishing : ServiceBase, IPagePublishing
{
    #region Constructor / DI

    public OqtPagePublishing() : base($"{OqtConstants.OqtLogPrefix}.Publsh") { }

    #endregion

    public void DoInsidePublishing(IContextOfSite context, Action<VersioningActionInfo> action)
    {
        var containerId = (context as IContextOfBlock)?.Module.Id ?? Eav.Constants.IdNotInitialized;
        var userId = 0;
        var enabled = false;
        Log.A($"DoInsidePublishing(module:{containerId}, user:{userId}, enabled:{enabled})");
        if (enabled)
        {
            /* ignore */
        }

        var versioningActionInfo = new VersioningActionInfo();
        action.Invoke(versioningActionInfo);
        Log.A("/DoInsidePublishing");
    }



    public int GetLatestVersion(int instanceId) => 0;

    public int GetPublishedVersion(int instanceId) => 0;


    public void Publish(int instanceId, int version) 
        => Log.A($"Publish(m:{instanceId}, v:{version}) - not supported in Oqtane, publish never happened ");
}