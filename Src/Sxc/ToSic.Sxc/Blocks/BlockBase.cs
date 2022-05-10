using System;
using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Run;
using ToSic.Eav.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Blocks.Output;
using ToSic.Sxc.Context;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Engines;
using ToSic.Sxc.LookUp;
using ToSic.Sxc.Run;
using App = ToSic.Sxc.Apps.App;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Blocks
{
    public abstract partial class BlockBase : HasLog, IBlock
    {
        #region Constructor and DI

        public class Dependencies
        {
            public Dependencies(
                Lazy<BlockDataSourceFactory> bdsFactoryLazy,
                Lazy<App> appLazy,
                Lazy<AppConfigDelegate> appConfigDelegateLazy,
                Lazy<CmsRuntime> cmsLazy,
                Generator<IEnvironmentInstaller> envInstGen,
                Generator<IRenderingHelper> renderHelpGen,
                Generator<IRazorEngine> razorEngineGen, 
                Generator<TokenEngine> tokenEngineGen,
                LazyInitLog<IBlockResourceExtractor> resourceExtractor
                )
            {
                BdsFactoryLazy = bdsFactoryLazy;
                AppLazy = appLazy;
                AppConfigDelegateLazy = appConfigDelegateLazy;
                CmsLazy = cmsLazy;
                EnvInstGen = envInstGen;
                RenderHelpGen = renderHelpGen;
                RazorEngineGen = razorEngineGen;
                TokenEngineGen = tokenEngineGen;
                ResourceExtractor = resourceExtractor;
            }
            internal Lazy<BlockDataSourceFactory> BdsFactoryLazy { get; }
            internal Lazy<App> AppLazy { get; }
            internal Lazy<AppConfigDelegate> AppConfigDelegateLazy { get; }
            internal Lazy<CmsRuntime> CmsLazy { get; }
            internal Generator<IEnvironmentInstaller> EnvInstGen { get; }
            internal Generator<IRenderingHelper> RenderHelpGen { get; }
            internal Generator<IRazorEngine> RazorEngineGen { get; }
            internal Generator<TokenEngine> TokenEngineGen { get; }
            public LazyInitLog<IBlockResourceExtractor> ResourceExtractor { get; }
        }

        protected BlockBase(Dependencies dependencies, string logName) : base(logName) => _deps = dependencies;
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
            var wrapLog = Log.Call<bool>();

            ParentId = Context.Module.Id;
            ContentBlockId = blockNumberUnsureIfNeeded;

            Log.Add($"parent#{ParentId}, content-block#{ContentBlockId}, z#{ZoneId}, a#{AppId}");

            // 2020-09-04 2dm - new change, moved BlockBuilder up so it's never null - may solve various issues
            // but may introduce new ones
            BlockBuilder = new BlockBuilder(rootBuilderOrNull, this, _deps.EnvInstGen, _deps.RenderHelpGen, _deps.RazorEngineGen, _deps.TokenEngineGen, _deps.ResourceExtractor, Log);

            // If specifically no app found, end initialization here
            // Means we have no data, and no BlockBuilder
            if (AppId == AppConstants.AppIdNotFound || AppId == Eav.Constants.NullId)
            {
                DataIsMissing = true;
                return wrapLog("stop: app & data are missing", true);
            }

            // If no app yet, stop now with BlockBuilder created
            if (AppId == Eav.Constants.AppIdEmpty)
            {
                var msg = $"stop a:{AppId}, container:{Context.Module.Id}, content-group:{Configuration?.Id}";
                return wrapLog(msg, true);
            }


            Log.Add("Real app specified, will load App object with Data");

            // Get App for this block
            Log.Add("About to create app");
            App = _deps.AppLazy.Value.PreInit(Context.Site)
                .Init(this, _deps.AppConfigDelegateLazy.Value.Init(Log).BuildForNewBlock(Context, this), Log);
            Log.Add("App created");

            // note: requires EditAllowed, which isn't ready till App is created
            var cms = _deps.CmsLazy.Value.Init(App, Context.UserMayEdit, Log);

            Configuration = cms.Blocks.GetOrGeneratePreviewConfig(blockId);

            // handle cases where the content group is missing - usually because of incomplete import
            if (Configuration.DataIsMissing)
            {
                DataIsMissing = true;
                App = null;
                return wrapLog($"DataIsMissing a:{AppId}, container:{Context.Module.Id}, content-group:{Configuration?.Id}", true);
            }

            // use the content-group template, which already covers stored data + module-level stored settings
            View = new BlockViewLoader(Log).PickView(this, Configuration.View, Context, cms);
            return wrapLog($"ok a:{AppId}, container:{Context.Module.Id}, content-group:{Configuration?.Id}", true);
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
                Log.Add(
                    $"About to load data source with possible app configuration provider. App is probably null: {App}");
                _dataSource = _deps.BdsFactoryLazy.Value.Init(Log).GetBlockDataSource(this, App?.ConfigurationProvider);
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