using DotNetNuke.Entities.Portals;
using ToSic.SexyContent.DataSources;
using ToSic.SexyContent.Interfaces;

namespace ToSic.SexyContent.ContentBlock
{
    internal class ContentBlockBase: IContentBlock
    {
        public IContentBlock Parent;// => SxcInstance;

        public int ZoneId { get; protected set; }
        public int AppId { get; protected set; }

        public App App { get; protected set; }

        public bool ContentGroupExists => ContentGroup?.Exists ?? false;
        public virtual ContentBlockManagerBase Manager => null;


        public bool ShowTemplateChooser { get; protected set; } = true;
        public virtual bool ParentIsEntity => false;
        public int ParentId { get; protected set; }
        public int ContentBlockId { get; protected set; }
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

        public PortalSettings PortalSettings { get; protected set; }

        // ReSharper disable once InconsistentNaming
        protected ViewDataSource _dataSource;
        public virtual ViewDataSource Data => null;

        public ContentGroup ContentGroup { get; protected set; }


        // ReSharper disable once InconsistentNaming
        protected SxcInstance _sxcInstance;
        public virtual SxcInstance SxcInstance => _sxcInstance 
            ?? (_sxcInstance = new SxcInstance(this, Parent.SxcInstance));

        public virtual bool IsContentApp => false;

    }
}