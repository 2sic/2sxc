using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Blocks;
using ToSic.Eav.Apps.Environment;
using ToSic.Sxc.Dnn;

namespace ToSic.SexyContent.Razor.Helpers
{
    public class DnnHelper : IDnnContext
    {
        /// <summary>
        /// Build DNN Helper
        /// Note that the context can be null, in which case it will have no module context, and default to the current portal
        /// </summary>
        /// <param name="moduleContext"></param>
        public DnnHelper(ICmsBlock moduleContext)
        {
            Module = (moduleContext as CmsBlock<ModuleInfo>)?.Original;
            Portal = PortalSettings.Current ?? 
                (moduleContext != null ? new PortalSettings(Module.PortalID): null);
        }

        public ModuleInfo Module { get; }

        public TabInfo Tab => Portal?.ActiveTab;

        public PortalSettings Portal { get; }

        public UserInfo User => Portal.UserInfo;
    }
}