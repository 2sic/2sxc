using System;
using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using ToSic.Eav;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Interfaces;
using ToSic.SexyContent.Internal;

namespace ToSic.SexyContent
{
    internal class EntityContentBlock: IContentBlock
    {
        private IContentBlock Parent;

        public int ZoneId { get;  }
        public int AppId { get; }

        public App App { get;  }

        public bool ContentGroupExists => ContentGroup?.Exists ?? false;

        public bool ShowTemplateChooser { get; set; }
        public bool ParentIsEntity => true;
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
            ?? (_dataSource = ViewDataSource.ForContentGroupInSxc(SxcInstance, Template));

        public ContentGroup ContentGroup { get; }

        #region ContentBlock Definition Entity

        private IEntity _contentBlockDefinition;
        private string _appName;
        private Guid _contentGroupGuid;
        private Guid _previewTemplateGuid;

        private void ParseContentBlockDefinition(IEntity cbDefinition)
        {
            _contentBlockDefinition = cbDefinition;
            _appName = _contentBlockDefinition.GetBestValue("App")?.ToString() ?? "";

            string temp = _contentBlockDefinition.GetBestValue("ContentGroup")?.ToString() ?? "";
            Guid.TryParse(temp, out _contentGroupGuid);

            temp = _contentBlockDefinition.GetBestValue("Template")?.ToString() ?? "";
            Guid.TryParse(temp, out _previewTemplateGuid);
            
            temp = _contentBlockDefinition.GetBestValue("ShowTemplateChooser")?.ToString() ?? "";
            bool show;
            if (bool.TryParse(temp, out show))
                ShowTemplateChooser = show;

        }
        #endregion

        public EntityContentBlock(IContentBlock parent, IEntity cbDefinition)
        {
            ParseContentBlockDefinition(cbDefinition);
            Parent = parent;
            ParentId = parent.ParentId;
            ContentBlockId = -cbDefinition.EntityId;// "mod:" + ParentId +  "-ent:" + cbDefinition.EntityId;


            // Ensure we know what portal the stuff is coming from
            PortalSettings = Parent.App.OwnerPortalSettings;

            ZoneId = Parent.ZoneId;

            
            AppId = AppHelpers.GetAppIdFromName(ZoneId, _appName); // should be 0 if unknown, must test

            if (AppId != 0)
            {
                // try to load the app - if possible
                App = new App(PortalSettings, AppId, ZoneId);
                ContentGroup =  App.ContentGroupManager.GetContentGroupOrGeneratePreview(_contentGroupGuid, _previewTemplateGuid);

                // use the content-group template, which already covers stored data + module-level stored settings
                Template = ContentGroup.Template;

                // maybe ensure that App.Data is ready?
                // App.InitData(...)?
            }
        }

        private SxcInstance _sxcInstance;
        public SxcInstance SxcInstance => _sxcInstance ??
                                          (_sxcInstance = new SxcInstance(this, Parent.SxcInstance));

        public bool IsContentApp => _appName == Constants.DefaultAppName;

    }
}