using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Enums;
using ToSic.Eav.Apps.Environment;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Data;
using ToSic.Eav.Logging;
using ToSic.Sxc.DataSources;

namespace ToSic.Sxc.Mvc.Run
{
    internal class MvcPagePublishing : HasLog, IPagePublishing
    {
        public MvcPagePublishing() : base("Mvc.Publsh")
        {
        }

        public IPagePublishing Init(ILog parent)
        {
            Log.LinkTo(parent, "Mvc.Publsh");
            return this;
        }


        public bool Supported => false;

        private readonly Dictionary<int, PublishingMode> _cache = new Dictionary<int, PublishingMode>();

        public PublishingMode Requirements(int instanceId)
        {
            var wrapLog = Log.Call<PublishingMode>($"{instanceId}");
            if (_cache.ContainsKey(instanceId)) return wrapLog("in cache", _cache[instanceId]);

            Log.Add($"Requirements(mod:{instanceId}) - checking first time (others will be cached)");
            try
            {
                PublishingMode decision = PublishingMode.DraftOptional;
                _cache.Add(instanceId, decision);
                return wrapLog("decision: ", decision);
            }
            catch
            {
                Log.Add("Requirements had exception!");
                throw;
            }
        }

        public bool IsEnabled(int instanceId) => Requirements(instanceId) != PublishingMode.DraftOptional;

        public void DoInsidePublishing(IInstanceContext context, Action<VersioningActionInfo> action)
        {
            var containerId = context.Container.Id;
            var userId = 0;
            var enabled = IsEnabled(containerId);
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

        private IEnumerable<IEntity> TryToAddStream(IEnumerable<IEntity> list, IBlockDataSource data, string key)
        {
            var cont = data.Out.ContainsKey(key) ? data[key]?.Immutable/*?.ToList()*/ : null;
            Log.Add($"TryToAddStream(..., ..., key:{key}), found:{cont != null} add⋮{cont?.Count ?? 0}");
            if (cont != null) list = list.Concat(cont);
            return list;
        }

    }
}
