using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using ToSic.Eav.Apps.Enums;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Dnn.Cms
{
    public class DnnPagePublishingSettings : PagePublishingSettingsBase
    {
        #region DI Constructors and More
        
        public DnnPagePublishingSettings(IFeaturesService featuresService) : base(DnnConstants.LogName)
        {
            _featuresService = featuresService;
        }
        private readonly IFeaturesService _featuresService;

        #endregion

        protected override PublishingMode LookupRequirements(int moduleId)
        {
            Log.Add($"Requirements(mod:{moduleId}) - checking first time (others will be cached)");
            try
            {
                // TODO V14 - probably we can set ignoreCache to false then, as it's probably just a workaround for an old bug
                var mod = ModuleController.Instance.GetModule(moduleId, Null.NullInteger, true);
                var versioningEnabled = TabChangeSettings.Instance.IsChangeControlEnabled(mod.PortalID, mod.TabID);
                if (!versioningEnabled)
                    return PublishingMode.DraftOptional;
                if (!new PortalSettings(mod.PortalID).UserInfo.IsSuperUser)
                    return PublishingMode.DraftRequired;
                return PublishingMode.DraftRequired;
            }
            catch
            {
                Log.Add("Requirements had exception!");
                throw;
            }
        }

        #region SwitchableService

        public override string NameId => DnnConstants.LogName + "PublishingSettings";

        public override int Priority => 10;

        /// <summary>
        /// It's viable if it has not been turned off
        /// </summary>
        /// <returns></returns>
        public override bool IsViable() => _featuresService.IsEnabled(Configuration.Features.BuiltInFeatures.DnnPageWorkflow.NameId);

        #endregion
    }
}
