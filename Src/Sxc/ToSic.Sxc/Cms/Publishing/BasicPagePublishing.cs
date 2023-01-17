using System;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Context;
using ToSic.Lib.Logging;
using ToSic.Eav.Run.Unknown;
using ToSic.Lib.Services;


namespace ToSic.Sxc.Cms.Publishing
{
    internal class BasicPagePublishing : ServiceBase, IPagePublishing
    {
        public BasicPagePublishing(WarnUseOfUnknown<BasicPagePublishing> warn) : base($"{LogScopes.NotImplemented}.Publsh") { }

        public void DoInsidePublishing(IContextOfSite context, Action<VersioningActionInfo> action) => Log.Do(() =>
        {
            var versioningActionInfo = new VersioningActionInfo();
            action.Invoke(versioningActionInfo);
        });



        public int GetLatestVersion(int instanceId) => 0;

        public int GetPublishedVersion(int instanceId) => 0;


        public void Publish(int instanceId, int version)
        {
            Log.A($"Publish(m:{instanceId}, v:{version})");
            Log.A("publish never happened ");
        }

    }
}
