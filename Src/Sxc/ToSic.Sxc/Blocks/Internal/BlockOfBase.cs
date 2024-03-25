using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Apps.Internal.Work;
using ToSic.Eav.Cms.Internal;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Lib.Services;
using ToSic.Sxc.Apps.Internal;
using ToSic.Sxc.Apps.Internal.Work;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.DataSources.Internal;
using ToSic.Sxc.LookUp.Internal;
using App = ToSic.Sxc.Apps.App;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Blocks.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public abstract class BlockBase(BlockBase.MyServices services, string logName, object[] connect = default)
    : ServiceBase<BlockBase.MyServices>(services, logName, connect: connect ?? []), IBlock
{
    #region Constructor and DI

    public class MyServices(
        GenWorkPlus<WorkViews> workViews,
        GenWorkPlus<WorkBlocks> appBlocks,
        LazySvc<BlockDataSourceFactory> bdsFactoryLazy,
        LazySvc<App> appLazy,
        LazySvc<BlockBuilder> blockBuilder)
        : MyServicesBase(connect: [bdsFactoryLazy, appLazy, blockBuilder, workViews, appBlocks])
    {
        internal LazySvc<BlockDataSourceFactory> BdsFactoryLazy { get; } = bdsFactoryLazy;
        internal LazySvc<App> AppLazy { get; } = appLazy;
        public LazySvc<BlockBuilder> BlockBuilder { get; } = blockBuilder;
        public GenWorkPlus<WorkViews> WorkViews { get; } = workViews;
        public GenWorkPlus<WorkBlocks> AppBlocks { get; } = appBlocks;
    }

    protected void Init(IContextOfBlock context, IAppIdentity appId)
    {
        Context = context;
        ZoneId = appId.ZoneId;
        AppId = appId.AppId;
    }

    protected bool CompleteInit(IBlockBuilder rootBuilderOrNull, IBlockIdentifier blockId, int blockNumberUnsureIfNeeded)
    {
        var l = Log.Fn<bool>();

        ParentId = Context.Module.Id;
        ContentBlockId = blockNumberUnsureIfNeeded;

        l.A($"parent#{ParentId}, content-block#{ContentBlockId}, z#{ZoneId}, a#{AppId}");

        // 2020-09-04 2dm - new change, moved BlockBuilder up, so it's never null - may solve various issues
        // but may introduce new ones
        BlockBuilder = Services.BlockBuilder.Value.Init(rootBuilderOrNull, this);

        switch (AppId)
        {
            // If specifically no app found, end initialization here
            // Means we have no data, and no BlockBuilder
            case AppConstants.AppIdNotFound or Eav.Constants.NullId:
                DataIsMissing = true;
                return l.ReturnTrue("stop: app & data are missing");
            // If no app yet, stop now with BlockBuilder created
            case Eav.Constants.AppIdEmpty:
                return l.ReturnTrue($"stop a:{AppId}, container:{Context.Module.Id}, content-group:{Configuration?.Id}");
        }

        l.A("Real app specified, will load App object with Data");

        // Get App for this block
        var app = Services.AppLazy.Value; //.PreInit(Context.Site);
        app.Init(Context.Site, this.PureIdentity(), new SxcAppDataConfigSpecs { BlockForLookupOrNull = this });
        App = app;
        l.A("App created");

        // note: requires EditAllowed, which isn't ready till App is created
        var appWorkCtxPlus = Services.WorkViews.CtxSvc.ContextPlus(this);
        Configuration = Services.AppBlocks.New(appWorkCtxPlus).GetOrGeneratePreviewConfig(blockId);

        // handle cases where the content group is missing - usually because of incomplete import
        if (Configuration.DataIsMissing)
        {
            DataIsMissing = true;
            App = null;
            return l.ReturnTrue($"DataIsMissing a:{AppId}, container:{Context.Module.Id}, content-group:{Configuration?.Id}");
        }

        // use the content-group template, which already covers stored data + module-level stored settings
        View = new BlockViewLoader(Log).PickView(this, Configuration.View, Context, Services.WorkViews.New(appWorkCtxPlus));
        return l.ReturnTrue($"ok a:{AppId}, container:{Context.Module.Id}, content-group:{Configuration?.Id}");
    }

    #endregion

    public IBlock Parent;

    public int ZoneId { get; protected set; }

    public int AppId { get; protected set; }

    public IApp App { get; protected set; }

    public bool ContentGroupExists => Configuration?.Exists ?? false;

    public List<string> BlockFeatureKeys { get; } = [];


    public int ParentId { get; protected set; }

    public bool DataIsMissing { get; private set; }
    public List<ProblemReport> Problems { get; } = [];

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



    public IBlockInstance Data => _data.GetL(Log, l =>
    {
        l.A($"About to load data source with possible app configuration provider. App is probably null: {App}");
        var dataSource = Services.BdsFactoryLazy.Value.GetContextDataSource(this, App?.ConfigurationProvider);
        return dataSource;
    });
    private readonly GetOnce<IBlockInstance> _data = new();

    public BlockConfiguration Configuration { get; protected set; }
        
    public IBlockBuilder BlockBuilder { get; protected set; }

    public bool IsContentApp { get; protected set; }
}