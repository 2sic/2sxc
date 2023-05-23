using System.Collections.Generic;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Apps.Run;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.LookUp;
using App = ToSic.Sxc.Apps.App;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Blocks
{
    public abstract partial class BlockBase : ServiceBase<BlockBase.MyServices>, IBlock
    {
        #region Constructor and DI

        public class MyServices: MyServicesBase
        {
            public MyServices(
                LazySvc<BlockDataSourceFactory> bdsFactoryLazy,
                LazySvc<App> appLazy,
                LazySvc<AppConfigDelegate> appConfigDelegateLazy,
                LazySvc<CmsRuntime> cmsLazy,
                LazySvc<BlockBuilder> blockBuilder
            )
            {
                ConnectServices(
                    BdsFactoryLazy = bdsFactoryLazy,
                    AppLazy = appLazy,
                    AppConfigDelegateLazy = appConfigDelegateLazy,
                    CmsLazy = cmsLazy,
                    BlockBuilder = blockBuilder
                );
            }

            internal LazySvc<BlockDataSourceFactory> BdsFactoryLazy { get; }
            internal LazySvc<App> AppLazy { get; }
            internal LazySvc<AppConfigDelegate> AppConfigDelegateLazy { get; }
            internal LazySvc<CmsRuntime> CmsLazy { get; }
            public LazySvc<BlockBuilder> BlockBuilder { get; }
        }

        protected BlockBase(MyServices services, string logName) : base(services, logName)
        {
        }

        protected void Init(IContextOfBlock context, IAppIdentity appId)
        {
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
            BlockBuilder = Services.BlockBuilder.Value.Init(rootBuilderOrNull, this);

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
            App = Services.AppLazy.Value
                .PreInit(Context.Site)
                .Init(this, Services.AppConfigDelegateLazy.Value.BuildForNewBlock(Context, this));
            Log.A("App created");

            // note: requires EditAllowed, which isn't ready till App is created
            var cms = Services.CmsLazy.Value.InitQ(App);

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
            set => Log.Setter(() =>
            {
                _view = value;
                _data.Reset(); // reset this if the view changed...
            });
        }
        private IView _view;

        #endregion


        /// <inheritdoc />
        public IContextOfBlock Context { get; protected set; }



        public IContextData Data => _data.Get(Log, l =>
        {
            l.A($"About to load data source with possible app configuration provider. App is probably null: {App}");
            var dataSource = Services.BdsFactoryLazy.Value.GetBlockDataSource(this, App?.ConfigurationProvider);
            return dataSource;
        });
        private readonly GetOnce<IContextData> _data = new GetOnce<IContextData>();

        public BlockConfiguration Configuration { get; protected set; }
        
        public IBlockBuilder BlockBuilder { get; protected set; }

        public bool IsContentApp { get; protected set; }
    }
}