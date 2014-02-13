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
        private readonly ModuleInstanceContext _context;

        public DnnHelper(ModuleInstanceContext context)
        {
            _context = context;
        }

        public ModuleInfo Module
        {
            get
            {
                return _context.Configuration;
            }
        }

        public TabInfo Tab
        {
            get
            {
                return _context.PortalSettings.ActiveTab;
            }
        }

        public PortalSettings Portal
        {
            get
            {
                return _context.PortalSettings;
            }
        }

        public UserInfo User
        {
            get
            {
                return _context.PortalSettings.UserInfo;
            }
        }
    }
}