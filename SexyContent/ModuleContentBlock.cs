using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Interfaces;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent
{
    internal class ModuleContentBlock: IContentBlock
    {
        public int ZoneId { get;  }
        public int AppId { get; }

        public App App { get;  }

        public ModuleInfo ModuleInfo;

        public bool ContentGroupExists => ContentGroup?.Exists ?? false;

        public bool ShowTemplateChooser { get; set; }
        public bool ParentIsEntity => false;
        public int ParentId { get; }
        public int ContentBlockId { get; }
        public string ParentFieldName => null;
        public int ParentFieldSortOrder => 0;

        #region Template and extensive template-choice initialization
        private Template _template;

        // ensure the data is also set correctly...
        // Sequence of determining template
        // 3. If content-group exists, use template definition there
        // 4. If module-settings exists, use that
        // 5. If nothing exists, ensure system knows nothing applied 
        // #. possible override: If specifically defined in some object calls (like web-api), use that (set when opening this object?)
        // #. possible override in url - and allowed by permissions (admin/host), use that
        public Template Template
        {
            get 
            {
                return _template; 
            }
            set
            {
                _template = value;
                _dataSource = null; // reset this
            }
        }

        #endregion

        public PortalSettings PortalSettings { get; }

        private ViewDataSource _dataSource;
        public ViewDataSource Data => _dataSource 
            ?? (_dataSource = ViewDataSource.ForContentGroupInSxc(SxcInstance, Template, ModuleInfo.ModuleID));

        public ContentGroup ContentGroup { get; }

        public ModuleContentBlock(ModuleInfo moduleInfo)
        {
            if(moduleInfo == null)
                throw new Exception("Need valid ModuleInfo / ModuleConfiguration of runtime");

            ModuleInfo = moduleInfo;
            ParentId = moduleInfo.ModuleID;
            ContentBlockId = ParentId;// "mod:" + ParentId + "-mod:" + ParentId;

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
                // App.InitData(...)?
            }
        }

        private SxcInstance _sxcInstance;
        public SxcInstance SxcInstance => _sxcInstance ??
                                          (_sxcInstance = new SxcInstance(this, ModuleInfo));

        public bool IsContentApp => ModuleInfo.DesktopModule.ModuleName == "2sxc";

    }
}