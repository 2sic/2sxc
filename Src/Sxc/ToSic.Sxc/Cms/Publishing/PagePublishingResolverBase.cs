using System.Collections.Generic;
using ToSic.Eav.Apps.Enums;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Cms.Publishing
{
    public abstract class PagePublishingResolverBase: HasLog<IPagePublishingResolver>, IPagePublishingResolver
    {
        protected PagePublishingResolverBase(string logPrefix) : base(logPrefix + ".PubRes") { }

        protected PublishingMode Requirements(int instanceId)
        {
            var wrapLog = Log.Call<PublishingMode>($"{instanceId}");
            if (Cache.ContainsKey(instanceId)) return wrapLog("in cache", Cache[instanceId]);

            var decision = LookupRequirements(instanceId);
            Cache.Add(instanceId, decision);
            return wrapLog("decision: ", decision);
        }
        protected static readonly Dictionary<int, PublishingMode> Cache = new Dictionary<int, PublishingMode>();

        /// <summary>
        /// The lookup must be implemented for each platform
        /// </summary>
        /// <param name="instanceId"></param>
        /// <returns></returns>
        protected abstract PublishingMode LookupRequirements(int instanceId);

        protected bool RequirementsIsEnabled(PublishingMode mode) => mode != PublishingMode.DraftOptional;

        public InstancePublishingState GetPublishingState(int instanceId)
        {
            var mode = Requirements(instanceId);
            return new InstancePublishingState { ForceDraft = RequirementsIsEnabled(mode), Mode = mode };
        }

    }
}
