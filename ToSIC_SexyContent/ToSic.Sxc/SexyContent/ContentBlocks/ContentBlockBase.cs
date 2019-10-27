using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Interfaces;
using ToSic.Eav.Logging;
using ToSic.Eav.Logging.Simple;
using ToSic.SexyContent.DataSources;
using ToSic.Sxc.Interfaces;

namespace ToSic.SexyContent.ContentBlocks
{
    internal abstract class ContentBlockBase : HasLog, IContentBlock
    {
        protected ContentBlockBase(Log parentLog, string logName) : base(logName, parentLog) { }

        public IContentBlock Parent;

        public int ZoneId { get; protected set; }
        public int AppId { get; protected set; }

        public App App { get; protected set; }

        public bool ContentGroupExists => ContentGroup?.Exists ?? false;
        public virtual ContentGroupReferenceManagerBase Manager => null;

        public bool ShowTemplateChooser { get; protected set; } = true;
        public virtual bool ParentIsEntity => false;
        public int ParentId { get; protected set; }

        // ReSharper disable once InconsistentNaming
        protected bool _dataIsMissing = false;
        public bool DataIsMissing => _dataIsMissing;

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

        public ITenant Tenant { get; protected set; }

        // ReSharper disable once InconsistentNaming
        protected ViewDataSource _dataSource;
        public virtual ViewDataSource Data => null;

        public ContentGroup ContentGroup { get; protected set; }


        public SxcInstance SxcInstance { get; protected set; }

        public virtual bool IsContentApp => false;


    }
}