using System.Collections.Concurrent;
using ToSic.Eav.Apps.Enums;
using ToSic.Eav.Logging;

namespace ToSic.Sxc.Cms.Publishing
{
    public abstract class PagePublishingSettingsBase: HasLog, IPagePublishingSettings
    {
        protected PagePublishingSettingsBase(string logPrefix) : base(logPrefix + ".PubRes") { }

        protected PublishingMode Requirements(int instanceId)
        {
            var wrapLog = Log.Call<PublishingMode>($"{instanceId}");
            if (instanceId < 0) return wrapLog("no instance", PublishingMode.DraftOptional);
            if (Cache.ContainsKey(instanceId)) return wrapLog("in cache", Cache[instanceId]);

            var decision = LookupRequirements(instanceId);
            Cache.TryAdd(instanceId, decision);
            return wrapLog("decision: ", decision);
        }
        protected static readonly ConcurrentDictionary<int, PublishingMode> Cache = new ConcurrentDictionary<int, PublishingMode>();

        /// <summary>
        /// The lookup must be implemented for each platform
        /// </summary>
        /// <param name="moduleId"></param>
        /// <returns></returns>
        protected abstract PublishingMode LookupRequirements(int moduleId);

        public BlockPublishingSettings SettingsOfModule(int moduleId)
        {
            var mode = Requirements(moduleId);
            return new BlockPublishingSettings { ForceDraft = mode == PublishingMode.DraftRequired, Mode = mode };
        }

        #region SwitchableService


        public virtual string NameId => "Default";

        public virtual bool IsViable() => true;

        public virtual int Priority => 1;

        #endregion
    }
}
