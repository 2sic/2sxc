using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks.Views;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.LookUp;
using App = ToSic.Sxc.Apps.App;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Blocks
{
    internal abstract partial class BlockBase : HasLog<BlockBase>, IBlock
    {
        #region Constructor and DI

        protected BlockBase(string logName) : base(logName) { }

        protected void Init(IInstanceContext context, IAppIdentity appId, ILog parentLog)
        {
            Init(parentLog);
            Context = context;
            ZoneId = appId.ZoneId;
            AppId = appId.AppId;
        }

        protected T CompleteInit<T>(IBlockBuilder rootBuilder, IBlockIdentifier blockId, int blockNumberUnsureIfNeeded) where T : class
        {
            var wrapLog = Log.Call<T>();

            ParentId = Context.Container.Id;
            ContentBlockId = blockNumberUnsureIfNeeded;

            Log.Add($"parent#{ParentId}, content-block#{ContentBlockId}, z#{ZoneId}, a#{AppId}");

            // 2020-09-04 2dm - new change, moved BlockBuilder up so it's never null - may solve various issues
            // but may introduce new ones
            BlockBuilder = new BlockBuilder(rootBuilder, this, Log);

            // If specifically no app found, end initialization here
            // Means we have no data, and no BlockBuilder
            if (AppId == AppConstants.AppIdNotFound || AppId == Eav.Constants.NullId)
            {
                DataIsMissing = true;
                return wrapLog("stop: app & data are missing", this as T);
            }

            // If no app yet, stop now with BlockBuilder created
            if (AppId == Eav.Constants.AppIdEmpty)
            {
                var msg = $"stop a:{AppId}, container:{Context.Container.Id}, content-group:{Configuration?.Id}";
                return wrapLog(msg, this as T);
            }


            Log.Add("Real app specified, will load App object with Data");

            // Get App for this block
            App = Eav.Factory.Resolve<App>().PreInit(Context.Tenant)
                .Init(this, ConfigurationProvider.Build(this, false),
                    true, Log);

            // note: requires EditAllowed, which isn't ready till App is created
            var publishing = Eav.Factory.Resolve<IPagePublishing>().Init(Log);
            var cms = new CmsRuntime(App, Log, EditAllowed, publishing.IsEnabled(Context.Container.Id));

            Configuration = cms.Blocks.GetOrGeneratePreviewConfig(blockId);

            // handle cases where the content group is missing - usually because of incomplete import
            if (Configuration.DataIsMissing)
            {
                DataIsMissing = true;
                App = null;
                return wrapLog($"DataIsMissing a:{AppId}, container:{Context.Container.Id}, content-group:{Configuration?.Id}", this as T);
            }

            // use the content-group template, which already covers stored data + module-level stored settings
            View = new BlockViewLoader(Log).PickView(this, Configuration.View, Context, cms);
            return wrapLog($"ok a:{AppId}, container:{Context.Container.Id}, content-group:{Configuration?.Id}", this as T);
        }

        #endregion

        public IBlock Parent;

        public int ZoneId { get; protected set; }

        public int AppId { get; protected set; }

        public IApp App { get; protected set; }

        public bool ContentGroupExists => Configuration?.Exists ?? false;

        public int ParentId { get; protected set; }

        public bool DataIsMissing { get; private set; }

        public int ContentBlockId { get; protected set; }
        
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
            get => _view;
            set
            {
                _view = value;
                _dataSource = null; // reset this if the view changed...
            }
        }

        #endregion


        /// <inheritdoc />
        public IInstanceContext Context { get; protected set; }

        // ReSharper disable once InconsistentNaming
        protected IBlockDataSource _dataSource;


        public IBlockDataSource Data => _dataSource
                                        ?? (_dataSource = Block.GetBlockDataSource(this, View,
                                            App?.ConfigurationProvider, Log));

        public BlockConfiguration Configuration { get; protected set; }
        
        public IBlockBuilder BlockBuilder { get; protected set; }

        public bool IsContentApp { get; protected set; }
    }
}