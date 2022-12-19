using System;
using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Run;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.LookUp;
using App = ToSic.Sxc.Apps.App;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Blocks
{
    public abstract partial class BlockBase : HasLog, IBlock
    {
        #region Constructor and DI

        public class Dependencies: DependenciesBase<Dependencies>
        {
            public Dependencies(
                Lazy<BlockDataSourceFactory> bdsFactoryLazy,
                Lazy<App> appLazy,
                Lazy<AppConfigDelegate> appConfigDelegateLazy,
                Lazy<CmsRuntime> cmsLazy,
                LazyInitLog<BlockBuilder> blockBuilder
            ) => AddToLogQueue(
                BdsFactoryLazy = bdsFactoryLazy,
                AppLazy = appLazy,
                AppConfigDelegateLazy = appConfigDelegateLazy,
                CmsLazy = cmsLazy,
                BlockBuilder = blockBuilder
            );

            internal Lazy<BlockDataSourceFactory> BdsFactoryLazy { get; }
            internal Lazy<App> AppLazy { get; }
            internal Lazy<AppConfigDelegate> AppConfigDelegateLazy { get; }
            internal Lazy<CmsRuntime> CmsLazy { get; }
            public LazyInitLog<BlockBuilder> BlockBuilder { get; }
        }

        protected BlockBase(Dependencies dependencies, string logName) : base(logName)
        {
            _deps = dependencies.SetLog(Log);
        }

        private readonly Dependencies _deps;

        protected void Init(IContextOfBlock context, IAppIdentity appId, ILog parentLog)
        {
            this.Init(parentLog);
            Context = context;
            ZoneId = appId.ZoneId;
            AppId = appId.AppId;
        }

        protected bool CompleteInit(IBlockBuilder rootBuilderOrNull, IBlockIdentifier blockId, int blockNumberUnsureIfNeeded)
        {
            var wrapLog = Log.Fn<bool>();

            ParentId = Context.Module.Id;
            ContentBlockId = blockNumberUnsureIfNeeded;

            Log.A($"parent#{ParentId}, content-block#{ContentBlockId}, z#{ZoneId}, a#{AppId}");

            // 2020-09-04 2dm - new change, moved BlockBuilder up so it's never null - may solve various issues
            // but may introduce new ones
            BlockBuilder = _deps.BlockBuilder.Value.Init(rootBuilderOrNull, this);

            // If specifically no app found, end initialization here
            // Means we have no data, and no BlockBuilder
            if (AppId == AppConstants.AppIdNotFound || AppId == Eav.Constants.NullId)
            {
                DataIsMissing = true;
                return wrapLog.ReturnTrue("stop: app & data are missing");
            }

            // If no app yet, stop now with BlockBuilder created
            if (AppId == Eav.Constants.AppIdEmpty)
                return wrapLog.ReturnTrue($"stop a:{AppId}, container:{Context.Module.Id}, content-group:{Configuration?.Id}");

            Log.A("Real app specified, will load App object with Data");

            // Get App for this block
            Log.A("About to create app");
            App = _deps.AppLazy.Value.PreInit(Context.Site)
                .Init(this, _deps.AppConfigDelegateLazy.Value.BuildForNewBlock(Context, this), Log);
            Log.A("App created");

            // note: requires EditAllowed, which isn't ready till App is created
            var cms = _deps.CmsLazy.Value.Init(App, Context.UserMayEdit, Log);

            Configuration = cms.Blocks.GetOrGeneratePreviewConfig(blockId);

            // handle cases where the content group is missing - usually because of incomplete import
            if (Configuration.DataIsMissing)
            {
                DataIsMissing = true;
                App = null;
                return wrapLog.ReturnTrue($"DataIsMissing a:{AppId}, container:{Context.Module.Id}, content-group:{Configuration?.Id}");
            }

            // use the content-group template, which already covers stored data + module-level stored settings
            View = new BlockViewLoader(Log).PickView(this, Configuration.View, Context, cms);
            return wrapLog.ReturnTrue($"ok a:{AppId}, container:{Context.Module.Id}, content-group:{Configuration?.Id}");
        }

        #endregion

        public IBlock Parent;

        public int ZoneId { get; protected set; }

        public int AppId { get; protected set; }

        public IApp App { get; protected set; }

        public bool ContentGroupExists => Configuration?.Exists ?? false;

        public List<string> BlockFeatureKeys { get; } = new List<string>();


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
        public IContextOfBlock Context { get; protected set; }



        public IBlockDataSource Data
        {
            get
            {
                if (_dataSource != null) return _dataSource;
                Log.A(
                    $"About to load data source with possible app configuration provider. App is probably null: {App}");
                _dataSource = _deps.BdsFactoryLazy.Value.GetBlockDataSource(this, App?.ConfigurationProvider);
                return _dataSource;
            }
        }

        // ReSharper disable once InconsistentNaming
        protected IBlockDataSource _dataSource;

        public BlockConfiguration Configuration { get; protected set; }
        
        public IBlockBuilder BlockBuilder { get; protected set; }

        public bool IsContentApp { get; protected set; }
    }
}