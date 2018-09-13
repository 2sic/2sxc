using DotNetNuke.Entities.Portals;
using ToSic.Eav.Logging.Simple;
using ToSic.SexyContent.Razor.Helpers;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class DnnAppAndDataHelpers : AppAndDataHelpersBase
    {
        public DnnAppAndDataHelpers(SxcInstance sxcInstance) : this(sxcInstance, null) {}

        public DnnAppAndDataHelpers(SxcInstance sxcInstance, Log parentLog): base(sxcInstance, new DnnTenant(null), parentLog)
        {
            // Init things than require module-info or similar, but not 2sxc
            var instance = sxcInstance?.EnvInstance;
            Dnn = new DnnHelper(instance);
            Link = new DnnLinkHelper(Dnn);

            // If PortalSettings is null - for example, while search index runs - HasEditPermission would fail
            // But in search mode, it shouldn't show drafts, so this is ok.
            // Note that app could be null, if a user is in admin-ui of a module which hasn't actually be configured yet
            InitAppDataFromContext(App, PortalSettings.Current, sxcInstance);
        }

        // todo: maybe move to somewhere more appropriate, just not sure where
        private static void InitAppDataFromContext(App app, PortalSettings portalSettings, SxcInstance sxcInstance)
        {
            // check if we have known context, otherwise ignore
            if (sxcInstance == null)
                return;

            app?.InitData(portalSettings != null && sxcInstance.UserMayEdit,
                portalSettings != null && sxcInstance.Environment.PagePublishing.IsEnabled(sxcInstance.EnvInstance.Id),
                sxcInstance.Data.ConfigurationProvider);
        }


        /// <summary>
        /// Dnn context with module, page, portal etc.
        /// </summary>
        public DnnHelper Dnn { get; }
    }
}