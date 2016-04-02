using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent.ContentBlock
{
    internal sealed class ModuleContentBlock: ContentBlockBase
    {
        public ModuleInfo ModuleInfo;

        public override ContentBlockManagerBase Manager => new ModuleContentBlockManager(SxcInstance);

        public override bool ParentIsEntity => false;

        public override ViewDataSource Data => _dataSource 
            ?? (_dataSource = ViewDataSource.ForContentGroupInSxc(SxcInstance, Template, ModuleInfo.ModuleID));

        public ModuleContentBlock(ModuleInfo moduleInfo)
        {
            if(moduleInfo == null)
                throw new Exception("Need valid ModuleInfo / ModuleConfiguration of runtime");

            ModuleInfo = moduleInfo;
            ParentId = moduleInfo.ModuleID;
            ContentBlockId = ParentId;

            // Ensure we know what portal the stuff is coming from
            PortalSettings = moduleInfo.OwnerPortalID != moduleInfo.PortalID
                ? new PortalSettings(moduleInfo.OwnerPortalID)
                : PortalSettings.Current;

            ZoneId = ZoneHelpers.GetZoneID(moduleInfo.OwnerPortalID) ?? 0; // new
            
            AppId = AppHelpers.GetAppIdFromModule(moduleInfo, ZoneId) ?? 0;// fallback/undefined YET

            if (AppId != 0)
            {
                // try to load the app - if possible
                App = new App(PortalSettings, AppId, ZoneId);
                ContentGroup = App.ContentGroupManager.GetContentGroupForModule(moduleInfo.ModuleID);

                // use the content-group template, which already covers stored data + module-level stored settings
                Template = ContentGroup.Template;

                // - in sxcinstance... CheckTemplateOverrides(); // check url-params, etc.

                // ensure data is initialized
                // nothing necessary, happens on the property

                // maybe ensure that App.Data is ready?
                App.InitData(SxcInstance.Environment.Permissions.UserMayEditContent, Data.ConfigurationProvider);
            }
        }

        public override SxcInstance SxcInstance => _sxcInstance ??
                                          (_sxcInstance = new SxcInstance(this, ModuleInfo));

        public override bool IsContentApp => ModuleInfo.DesktopModule.ModuleName == "2sxc";

    }
}