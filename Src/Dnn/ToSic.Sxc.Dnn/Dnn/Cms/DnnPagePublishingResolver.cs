using System.Collections.Generic;
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using ToSic.Eav.Apps.Enums;
using ToSic.Sxc.Cms.Publishing;

namespace ToSic.Sxc.Dnn.Cms
{
    public class DnnPagePublishingResolver : PagePublishingResolverBase
    {
        #region DI Constructors and More
        
        public DnnPagePublishingResolver(): base("Dnn.PubRes") { }
        
        #endregion

        //public override bool Supported => true;

        private readonly Dictionary<int, PublishingMode> _cache = new Dictionary<int, PublishingMode>();
        
        public override PublishingMode Requirements(int instanceId)
        {
            var wrapLog = Log.Call<PublishingMode>($"{instanceId}");
            if (_cache.ContainsKey(instanceId)) return wrapLog("in cache", _cache[instanceId]);

            Log.Add($"Requirements(mod:{instanceId}) - checking first time (others will be cached)");
            try
            {
                var moduleInfo = ModuleController.Instance.GetModule(instanceId, Null.NullInteger, true);
                PublishingMode decision;
                var versioningEnabled =
                    TabChangeSettings.Instance.IsChangeControlEnabled(moduleInfo.PortalID, moduleInfo.TabID);
                if (!versioningEnabled)
                    decision = PublishingMode.DraftOptional;
                else if (!new PortalSettings(moduleInfo.PortalID).UserInfo.IsSuperUser)
                    decision = PublishingMode.DraftRequired;
                else
                    decision = PublishingMode.DraftRequired;

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
