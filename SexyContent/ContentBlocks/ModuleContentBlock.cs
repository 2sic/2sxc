using System;
using System.Collections.Generic;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent.ContentBlocks
{
    internal sealed class ModuleContentBlock: ContentBlockBase
    {
        public ModuleInfo ModuleInfo;

        public override ContentGroupReferenceManagerBase Manager => new ModuleContentGroupReferenceManager(SxcInstance);

        public override bool ParentIsEntity => false;

        public override ViewDataSource Data => _dataSource 
            ?? (_dataSource = ViewDataSource.ForContentGroupInSxc(SxcInstance, Template, ModuleInfo.ModuleID));

        private readonly IEnumerable<KeyValuePair<string, string>> _urlParams;

        public ModuleContentBlock(ModuleInfo moduleInfo, IEnumerable<KeyValuePair<string, string>> overrideParams = null)
        {
            if(moduleInfo == null)
                throw new Exception("Need valid ModuleInfo / ModuleConfiguration of runtime");

            ModuleInfo = moduleInfo;
            ParentId = moduleInfo.ModuleID;
            ContentBlockId = ParentId;

            // url-params
            _urlParams = overrideParams ?? DnnWebForms.Helpers.SystemWeb.GetUrlParams();
            

            // Ensure we know what portal the stuff is coming from
            // PortalSettings is null, when in search mode
            PortalSettings = PortalSettings.Current == null || moduleInfo.OwnerPortalID != moduleInfo.PortalID
                ? new PortalSettings(moduleInfo.OwnerPortalID)
                : PortalSettings.Current;

            // important: don't use the SxcInstance.Environment, as it would try to init the Sxc-object before the app is known, causing various side-effects
            ZoneId = new Environment.DnnEnvironment().ZoneMapper.GetZoneId(moduleInfo.OwnerPortalID);// ZoneHelpers.GetZoneId(moduleInfo.OwnerPortalID) ?? 0; // new
            
            AppId = AppHelpers.GetAppIdFromModule(moduleInfo, ZoneId) ?? 0;// fallback/undefined YET

            if (AppId == Settings.DataIsMissingInDb)
            {
                _dataIsMissing = true;
                return;
            }

            if (AppId != 0)
            {
                // try to load the app - if possible
                App = new App(ZoneId, AppId, PortalSettings);
                // maybe ensure that App.Data is ready
                App.InitData(SxcInstance.Environment.Permissions.UserMayEditContent, new Environment.Dnn7.PagePublishing().IsVersioningEnabled(moduleInfo.ModuleID), Data.ConfigurationProvider);


                ContentGroup = App.ContentGroupManager.GetContentGroupForModule(moduleInfo.ModuleID, moduleInfo.TabID);

                if (ContentGroup.DataIsMissing)
                {
                    _dataIsMissing = true;
                    App = null;
                    return;
                }

                // use the content-group template, which already covers stored data + module-level stored settings
                Template = ContentGroup.Template;

                SxcInstance.CheckTemplateOverrides();

                // set show-status of the template/view picker
                var showStatus = moduleInfo.ModuleSettings[Settings.SettingsShowTemplateChooser];
                bool show;
                if (bool.TryParse((showStatus ?? true).ToString(), out show))
                    ShowTemplateChooser = show;

                
            }
        }


        public override SxcInstance SxcInstance => _sxcInstance ??
                                          (_sxcInstance = new SxcInstance(this, ModuleInfo, _urlParams));

        public override bool IsContentApp => ModuleInfo.DesktopModule.ModuleName == "2sxc";

    }
}