using System;
using System.Web;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Users;
using DotNetNuke.UI.Modules;
using ToSic.SexyContent.Interfaces;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent
{
    public class ModuleContentBlock
    {
        public int? ZoneId;
        public int? AppId;
        public ModuleInfo ModuleInfo;

        public bool UnreliableInfoThatSettingsAreStored;

        public bool ShowTemplatePicker { get; set; }
        public bool ParentIsEntity => false;
        public int ParentId { get; }
        public string ParentFieldName => null;
        public int ParentFieldSortOrder => 0;

        public Template Template { get; }
        public ContentGroup ContentGroup { get; }

        public ModuleContentBlock(ModuleInfo ModuleConfiguration, UserInfo UserInfo, HttpRequest Request, bool AllowUrlOverrides)
        {
            ModuleInfo = ModuleConfiguration;
            ParentId = ModuleConfiguration.ModuleID;

            // Set ZoneId based on the context
            ZoneId = (AllowUrlOverrides && UserInfo.IsSuperUser && !string.IsNullOrEmpty(Request.QueryString["ZoneId"])
                ? int.Parse(Request.QueryString["ZoneId"])
                : ZoneHelpers.GetZoneID(ModuleConfiguration.OwnerPortalID));


            AppId = AppHelpers.GetAppIdFromModule(ModuleConfiguration);
            if (AppId != null)
                UnreliableInfoThatSettingsAreStored = true;


        }

        private SxcInstance _sxcInstance;

        public SxcInstance SxcInstance => _sxcInstance ??
                                          (_sxcInstance = new SxcInstance(ZoneId ?? 0, AppId ?? 0, ModuleInfo.OwnerPortalID, ModuleInfo));
    }
}