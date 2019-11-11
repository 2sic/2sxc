using ToSic.Eav.Apps;
using ToSic.Eav.Logging;
using ToSic.Sxc.Apps.Blocks;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Blocks
{
    internal abstract class BlockBase : HasLog, IBlock
    {
        protected BlockBase(ILog parentLog, string logName) : base(logName, parentLog) { }

        public IBlock Parent;

        public int ZoneId { get; protected set; }
        public int AppId { get; protected set; }

        public IApp App { get; protected set; }

        public bool ContentGroupExists => Configuration?.Exists ?? false;
        public virtual BlockEditorBase Editor => null;

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
        private IView _view;

        // ensure the data is also set correctly...
        // Sequence of determining template
        // 3. If content-group exists, use template definition there
        // 4. If module-settings exists, use that
        // 5. If nothing exists, ensure system knows nothing applied 
        // #. possible override: If specifically defined in some object calls (like web-api), use that (set when opening this object?)
        // #. possible override in url - and allowed by permissions (admin/host), use that
        public IView View
        {
            get 
            {
                return _view; 
            }
            set
            {
                _view = value;
                _dataSource = null; // reset this
            }
        }

        #endregion

        public ITenant Tenant { get; protected set; }

        // ReSharper disable once InconsistentNaming
        protected IBlockDataSource _dataSource;
        public virtual IBlockDataSource Data => null;

        public BlockConfiguration Configuration { get; protected set; }


        public ICmsBlock CmsInstance { get; protected set; }

        public virtual bool IsContentApp => false;


    }
}