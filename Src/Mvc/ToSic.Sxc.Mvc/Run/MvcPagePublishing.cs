using System;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;
using ToSic.Sxc.Cms.Publishing;

namespace ToSic.Sxc.Mvc.Run
{
    internal class MvcPagePublishing : HasLog<IPagePublishing>, IPagePublishing
    {
        public MvcPagePublishing() : base("Mvc.Publsh") { }

        
        public void DoInsidePublishing(IInstanceContext context, Action<VersioningActionInfo> action)
        {
            var containerId = context.Container.Id;
            var userId = 0;
            var enabled = context.Publishing.ForceDraft;
            Log.Add($"DoInsidePublishing(module:{containerId}, user:{userId}, enabled:{enabled})");
            if (enabled)
            {
                /* ignore */
            }

            var versioningActionInfo = new VersioningActionInfo();
            action.Invoke(versioningActionInfo);
            Log.Add("/DoInsidePublishing");
        }



        public int GetLatestVersion(int instanceId) => 0;

        public int GetPublishedVersion(int instanceId) => 0;


        public void Publish(int instanceId, int version)
        {
            Log.Add($"Publish(m:{instanceId}, v:{version})");
            Log.Add("publish never happened ");
        }

    }
}
