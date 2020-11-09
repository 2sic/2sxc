using System.Collections.Generic;
using ToSic.Eav.Apps.Enums;
using ToSic.Sxc.Cms.Publishing;

namespace ToSic.Sxc.Mvc.Run
{
    internal class MvcPagePublishingResolver : PagePublishingResolverBase
    {
        public MvcPagePublishingResolver() : base("Mvc.Publsh") { }

        //public override bool Supported => false;

        private readonly Dictionary<int, PublishingMode> _cache = new Dictionary<int, PublishingMode>();

        public override PublishingMode Requirements(int instanceId)
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


    }
}
