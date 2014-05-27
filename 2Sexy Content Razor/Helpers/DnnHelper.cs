using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using DotNetNuke.UI.Modules;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Tabs;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Users;

namespace ToSic.SexyContent.Razor.Helpers
{
    public class DnnHelper
    {
        private readonly ModuleInfo _context;
        private readonly PortalSettings _portalSettings;

        public DnnHelper(ModuleInfo context)
        {
            _context = context;
            _portalSettings = PortalSettings.Current ?? new PortalSettings(context.PortalID);
        }

        public ModuleInfo Module
        {
            get
            {
                return _context;
            }
        }

        public TabInfo Tab
        {
            get
            {
                if (_portalSettings == null)
                    return null;
                return _portalSettings.ActiveTab;
            }
        }

        public PortalSettings Portal
        {
            get
            {
                return _portalSettings;
            }
        }

        public UserInfo User
        {
            get
            {
                return _portalSettings.UserInfo;
            }
        }
    }
}