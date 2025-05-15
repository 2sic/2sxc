using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Internal;
using ToSic.Eav.Cms.Internal;
using ToSic.Eav.DataSource;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks.Internal.Render;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.LookUp.Internal;
using ToSic.Sxc.Web.Internal.PageFeatures;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Blocks.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class BlockOfBase(BlockServices services, string logName, object[] connect = default)
    : ServiceBase<BlockServices>(services, logName, connect: connect ?? []), IBlock
{
    #region Constructor and DI

    protected void Init(IContextOfBlock context, IAppIdentity appId)
    {
        Context = context;
        ZoneId = appId.ZoneId;
        AppId = appId.AppId;
    }

    protected bool CompleteInit(IBlock parentBlockOrNull, IBlockIdentifier blockId, int blockNumberUnsureIfNeeded)
    {
        var l = Log.Fn<bool>();

        ParentBlock = parentBlockOrNull;
        RootBlock = parentBlockOrNull?.RootBlock ?? this; // if parent is null, this is the root block

        // Note: this is "just" the module id, not the block id
        ParentId = Context.Module.Id;
        ContentBlockId = blockNumberUnsureIfNeeded;

        l.A($"parent#{ParentId}, content-block#{ContentBlockId}, z#{ZoneId}, a#{AppId}");

        // 2020-09-04 2dm - new change, moved BlockBuilder up, so it's never null - may solve various issues
        // but may introduce new ones
        //BlockBuilder = Services.BlockBuilder.Value.Setup(this);

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
        var app = Services.AppLazy.Value;
        app.Init(Context.Site, this.PureIdentity(), new SxcAppDataConfigSpecs { BlockForLookupOrNull = this });
        App = app;
        l.A("App created");

        // note: requires EditAllowed, which isn't ready till App is created
        var appWorkCtxPlus = Services.WorkViews.CtxSvc.ContextPlus(this);
        Configuration = Services.AppBlocks.New(appWorkCtxPlus)
            .GetOrGeneratePreviewConfig(blockId);

        // handle cases where the content group is missing - usually because of incomplete import
        if (Configuration.DataIsMissing)
        {
            DataIsMissing = true;
            App = null;
            return l.ReturnTrue($"DataIsMissing a:{AppId}, container:{Context.Module.Id}, content-group:{Configuration?.Id}");
        }

        // use the content-group template, which already covers stored data + module-level stored settings
        View = new BlockViewLoader(Log)
            .PickView(this, Configuration.View, Context, Services.WorkViews.New(appWorkCtxPlus));
        return l.ReturnTrue($"ok a:{AppId}, container:{Context.Module.Id}, content-group:{Configuration?.Id}");
    }

    #endregion

    public IBlock ParentBlock { get; private set; }

    public IBlock RootBlock { get; private set; }

    public int ZoneId { get; protected set; }

    public int AppId { get; protected set; }

    public IApp App { get; protected set; }

    public bool ContentGroupExists => Configuration?.Exists ?? false;

    public List<string> BlockFeatureKeys { get; } = [];

    public List<IPageFeature> BlockFeatures(ILog? log = default)
        => !BlockFeatureKeys.Any()
            ? []
            : ((ContextOfBlock)Context).PageServiceShared.PageFeatures.GetWithDependents(BlockFeatureKeys, log ?? Log);

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
        get;
        // ReSharper disable once ExplicitCallerInfoArgument
        set => Log.Do(cName: $"set{nameof(value)}", action: () =>
        {
            field = value;
            Data = null; // reset this if the view changed...
        });
    }

    #endregion


    /// <inheritdoc />
    public IContextOfBlock Context { get; protected set; }


    public IDataSource Data
    {
        get => field ??= GetData();
        set;
    }

    private IDataSource GetData()
    {
        var l = Log.Fn<IDataSource>();
        l.A($"About to load data source with possible app configuration provider. App is probably null: {App}");
        var dataSource = Services.BdsFactoryLazy.Value.GetContextDataSource(this, App?.ConfigurationProvider);
        return l.Return(dataSource);
    }

    public BlockConfiguration Configuration { get; protected set; }
        
    /// <inheritdoc />
    public object BlockBuilder { get; protected set; }

    public bool IsContentApp { get; protected set; }

    #region Dependent Apps List so that caching knows what to monitor; relevant for inner-content scenarios

    /// <summary>
    /// This list is only populated on the root builder. Child builders don't actually use this.
    /// </summary>
    internal IList<IDependentApp> DependentApps { get; } = new List<IDependentApp>();

    internal void PushAppDependenciesToRoot()
    {
        var myAppId = AppId;
        // this is only relevant for the root builder, so we can skip it for child builders
        if (/*Block == null || Block.*/myAppId == 0)
            return;

        // Cast to current object type to access internal APIs
        if ((/*Block.*/RootBlock/*.BlockBuilder*/ ?? this) is not BlockOfBase rootBuilder)
            return;

        // add dependent appId only once
        if (rootBuilder.DependentApps.All(a => a.AppId != myAppId /*Block.AppId*/))
            rootBuilder.DependentApps.Add(new DependentApp { AppId = myAppId /*Block.AppId*/ });
    }


    #endregion


}