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
        
        public DnnPagePublishingResolver(): base(DnnConstants.LogName) { }
        
        #endregion

        protected override PublishingMode LookupRequirements(int instanceId)
        {
            PublishingMode decision;
            Log.Add($"Requirements(mod:{instanceId}) - checking first time (others will be cached)");
            try
            {
                var moduleInfo = ModuleController.Instance.GetModule(instanceId, Null.NullInteger, true);
                var versioningEnabled =
                    TabChangeSettings.Instance.IsChangeControlEnabled(moduleInfo.PortalID, moduleInfo.TabID);
                if (!versioningEnabled)
                    decision = PublishingMode.DraftOptional;
                else if (!new PortalSettings(moduleInfo.PortalID).UserInfo.IsSuperUser)
                    decision = PublishingMode.DraftRequired;
                else
                    decision = PublishingMode.DraftRequired;
            }
            catch
            {
                Log.Add("Requirements had exception!");
                throw;
            }

            return decision;
        }
    }
}
