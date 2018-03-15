using DotNetNuke.Entities.Portals;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Logging.Simple;
using ToSic.SexyContent.DnnWebForms.Helpers;
using ToSic.SexyContent.Razor.Helpers;

namespace ToSic.SexyContent.Environment.Dnn7
{
    public class DnnAppAndDataHelpers : AppAndDataHelpersBase
    {
        public DnnAppAndDataHelpers(SxcInstance sxcInstance) : this(sxcInstance, sxcInstance.InstanceInfo, null) {}

        public DnnAppAndDataHelpers(SxcInstance sxcInstance, IInstanceInfo instance, Log parentLog): base(sxcInstance, new DnnTenant(null), parentLog)
        {
            // Init things than require module-info or similar, but not 2sxc
            Dnn = new DnnHelper(instance);
            Link = new DnnLinkHelper(Dnn);

            if (sxcInstance == null)
                return;

            // If PortalSettings is null - for example, while search index runs - HasEditPermission would fail
            // But in search mode, it shouldn't show drafts, so this is ok.
            // Note that app could be null, if a user is in admin-ui of a module which hasn't actually be configured yet
            var userMayEdit = sxcInstance.UserMayEdit;// Eav.Factory.Resolve<IPermissions>().UserMayEditContent(sexy.InstanceInfo);
            App?.InitData(PortalSettings.Current != null && userMayEdit, 
                PortalSettings.Current != null && sxcInstance.Environment.PagePublishing.IsEnabled(instance.Id), 
                Data.ConfigurationProvider);
        }


        /// <summary>
        /// Dnn context with module, page, portal etc.
        /// </summary>
        public DnnHelper Dnn { get; }


    }
}