using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Enums;
using ToSic.Eav.Apps.Environment;
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

        public PublishingMode Requirements(int moduleId)
        {
            var wrapLog = Log.Call<PublishingMode>($"{moduleId}");
            if (_cache.ContainsKey(moduleId)) return wrapLog("in cache", _cache[moduleId]);

            Log.Add($"Requirements(mod:{moduleId}) - checking first time (others will be cached)");
            try
            {
                PublishingMode decision = PublishingMode.DraftOptional;
                _cache.Add(moduleId, decision);
                return wrapLog("decision: ", decision);
            }
            catch
            {
                Log.Add("Requirements had exception!");
                throw;
            }
        }

        public bool IsEnabled(int moduleId) => Requirements(moduleId) != PublishingMode.DraftOptional;

        public void DoInsidePublishing(int moduleId, int userId, Action<VersioningActionInfo> action)
        {
            var enabled = IsEnabled(moduleId);
            Log.Add($"DoInsidePublishing(module:{moduleId}, user:{userId}, enabled:{enabled})");
            if (enabled)
            {
                /* ignore */
            }

            var versioningActionInfo = new VersioningActionInfo();
            action.Invoke(versioningActionInfo);
            Log.Add("/DoInsidePublishing");
        }



        public int GetLatestVersion(int moduleId) => 0;

        public int GetPublishedVersion(int moduleId) => 0;


        public void Publish(int instanceId, int version)
        {
            Log.Add($"Publish(m:{instanceId}, v:{version})");
            Log.Add("publish never happened ");
        }

        private IEnumerable<IEntity> TryToAddStream(IEnumerable<IEntity> list, IBlockDataSource data, string key)
        {
            var cont = data.Out.ContainsKey(key) ? data[key]?.List?.ToList() : null;
            Log.Add($"TryToAddStream(..., ..., key:{key}), found:{cont != null} add⋮{cont?.Count ?? 0}");
            if (cont != null) list = list.Concat(cont);
            return list;
        }

    }
}
