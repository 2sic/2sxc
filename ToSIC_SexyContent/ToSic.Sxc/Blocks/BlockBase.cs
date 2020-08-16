using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;
using ToSic.Eav.Run;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.LookUp;
using App = ToSic.Sxc.Apps.App;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Blocks
{
    internal abstract class BlockBase : HasLog<BlockBase>, IBlock
    {
        #region Constructor and DI

        protected BlockBase(string logName) : base(logName) { }

        protected void Init(ITenant tenant, IAppIdentity appId, ILog parentLog)
        {
            Init(parentLog);
            Tenant = tenant;
            ZoneId = appId.ZoneId;
            AppId = appId.AppId;

        }

        protected T CompleteInit<T>(
            IBlockBuilder rootBuilder, 
            IContainer container,
            IBlockIdentifier blockId,
            int blockNumber
            ) where T : class
        {
            var wrapLog = Log.Call();

            ParentId = container.Id;
            ContentBlockId = blockNumber;

            Log.Add($"parent#{ParentId}, content-block#{ContentBlockId}, z#{ZoneId}, a#{AppId}");

            if (AppId == AppConstants.AppIdNotFound)
            {
                DataIsMissing = true;
                wrapLog("data is missing, will stop here");
                return this as T;
            }

            BlockBuilder = new BlockBuilder(rootBuilder, this, container, Log);
            // in case the root didn't exist yet, use the new one
            rootBuilder = rootBuilder ?? BlockBuilder;

            if (AppId == 0)
            {
                wrapLog($"ok a:{AppId}, container:{BlockBuilder.Container.Id}, content-group:{Configuration?.Id}");
                return this as T;
            }


            Log.Add("real app, will load data");


            App = new App(rootBuilder.Environment, Tenant)
                .Init(this, ConfigurationProvider.Build(BlockBuilder, false),
                    true, Log);


            var cms = new CmsRuntime(App, Log, rootBuilder.UserMayEdit, 
                rootBuilder.Environment.PagePublishing.IsEnabled(rootBuilder.Container.Id));

            Configuration = cms.Blocks.GetOrGeneratePreviewConfig(blockId); // blockGuid, previewViewGuid);

            // handle cases where the content group is missing - usually because of incomplete import
            if (Configuration.DataIsMissing)
            {
                DataIsMissing = true;
                App = null;
                wrapLog($"ok a:{AppId}, container:{BlockBuilder.Container.Id}, content-group:{Configuration?.Id}");
                return this as T;
            }

            // use the content-group template, which already covers stored data + module-level stored settings
            ((BlockBuilder)BlockBuilder).SetTemplateOrOverrideFromUrl(Configuration.View);
            wrapLog($"ok a:{AppId}, container:{BlockBuilder.Container.Id}, content-group:{Configuration?.Id}");
            return this as T;
        }

        #endregion

        public IBlock Parent;

        public int ZoneId { get; protected set; }

        public int AppId { get; protected set; }

        public IApp App { get; protected set; }

        public bool ContentGroupExists => Configuration?.Exists ?? false;

        // 2020-08-14 #2146 2dm believe unused
        //public bool ShowTemplateChooser { get; protected set; } = true;

        // 2020-08-16 clean-up #2148
        //public virtual bool ParentIsEntity => false;

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

        public ITenant Tenant { get; protected set; }

        // ReSharper disable once InconsistentNaming
        protected IBlockDataSource _dataSource;


        public IBlockDataSource Data => _dataSource
                                        ?? (_dataSource = Block.GetBlockDataSource(BlockBuilder, View,
                                            App?.ConfigurationProvider, Log));
        //public virtual IBlockDataSource Data => null;

        public BlockConfiguration Configuration { get; protected set; }
        
        public IBlockBuilder BlockBuilder { get; protected set; }

        public bool IsContentApp { get; protected set; }
    }
}