using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Users;
using ToSic.Sxc.Code;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Dnn.Context
{
    public class DnnContext : IDnnContext, INeedsCodeRoot
    {
        public void AddBlockContext(IDynamicCodeRoot codeRoot)
        {
            throw new System.NotImplementedException();
        }


        /// <summary>
        /// Build DNN Helper
        /// Note that the context can be null, in which case it will have no module context, and default to the current portal
        /// </summary>
        /// <param name="moduleContext"></param>
        public DnnContext Init(IModule moduleContext)
        {
            Module = (moduleContext as Module<ModuleInfo>)?.UnwrappedContents;
            // note: this may be a bug, I assume it should be Module.OwnerPortalId
            Portal = PortalSettings.Current ?? 
                (moduleContext != null ? new PortalSettings(Module.PortalID): null);
            return this;
        }



        public ModuleInfo Module { get; private set; }

        /// <summary>
        /// This used to just be
        /// </summary>
        public TabInfo Tab
        {
            get
            {
                return Portal?.ActiveTab;
                // Experimental, trying to fix https://github.com/2sic/2sxc/issues/2315
                // wasn't successful - the SkinPath was always empty with all variations of the code...
                //if (_tab != null) return _tab;
                //_tab = Portal?.ActiveTab;
                //if (Portal == null || _tab == null) return null;
                
                //// check if tab info is fully populated
                //if (string.IsNullOrEmpty(_tab.SkinPath))
                //{
                //    var tabController = new TabController();
                //    _tab = tabController.GetTab(_tab.TabID, Portal.PortalId);
                //}
                //return _tab;
            }
        }

        //private TabInfo _tab;

        public PortalSettings Portal { get; private set; }

        public UserInfo User => Portal.UserInfo;
    }
}