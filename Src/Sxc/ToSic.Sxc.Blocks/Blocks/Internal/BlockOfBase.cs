using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Cms.Internal;
using ToSic.Eav.DataSource;
using ToSic.Eav.Sys;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks.Internal.Render;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.LookUp.Internal;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Blocks.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class BlockOfBase(BlockServices services, string logName, object[]? connect = default)
    : ServiceBase<BlockServices>(services, logName, connect: connect ?? []), IBlock
{
    // New: WIP replacing the block with a stateless record
    public BlockSpecs Specs { get; protected set; } = new();

    #region Constructor and DI

    //protected BlockSpecs CompleteInit(IBlock? parentBlockOrNull, IBlockIdentifier blockId, int blockNumberUnsureIfNeeded)
    //{
    //    var l = Log.Fn<BlockSpecs>();

    //    Specs = Specs with
    //    {
    //        ParentBlockOrNull = parentBlockOrNull,
    //        RootBlock = parentBlockOrNull?.RootBlock ?? this, // if parent is null, this is the root block

    //        // Note: this is "just" the module id, not the block id
    //        //ParentId = Context.Module.Id,
    //        ContentBlockId = blockNumberUnsureIfNeeded,
    //    };

    //    l.A($"parent#{ParentId}, content-block#{ContentBlockId}, z#{ZoneId}, a#{AppId}");

    //    switch (AppId)
    //    {
    //        // If specifically no app found, end initialization here
    //        // Means we have no data, and no BlockBuilder
    //        case AppConstants.AppIdNotFound or EavConstants.NullId:
    //            return l.Return(Specs, "stop: app & data are missing");
    //        // If no app yet, stop now with BlockBuilder created
    //        case KnownAppsConstants.AppIdEmpty:
    //            return l.Return(Specs, $"stop a:{AppId}, container:{Context.Module.Id}, content-group:{ContentBlockId}");
    //    }

    //    l.A("Real app specified, will load App object with Data");

    //    // note: requires EditAllowed, which isn't ready till App is created
    //    // 2dm #???
    //    var appWorkCtxPlus = Services.WorkViews.CtxSvc.ContextPlus(this.PureIdentity());
    //    var config = Services.AppBlocks
    //        .New(appWorkCtxPlus)
    //        .GetOrGeneratePreviewConfig(blockId);

    //    Specs = Specs with
    //    {
    //        Configuration = config,
    //    };

    //    // handle cases where the content group is missing - usually because of incomplete import
    //    if (config.DataIsMissing)
    //        return l.Return(Specs, $"DataIsMissing a:{AppId}, container:{Context.Module.Id}, content-group:{config.Id}");

    //    // Get App for this block
    //    var app = Services.AppLazy.Value;
    //    app.Init(Context.Site, this.PureIdentity(), new SxcAppDataConfigSpecs { BlockForLookupOrNull = this });
    //    Specs = Specs with
    //    {
    //        AppOrNull = app,
    //    };
    //    l.A("App created");

    //    // use the content-group template, which already covers stored data + module-level stored settings
    //    var view = new BlockViewLoader(Log)
    //        .PickView(Specs, config.View, Services.WorkViews.New(appWorkCtxPlus));

    //    if (view == null)
    //        return l.Return(Specs, $"no view; a:{AppId}, container:{Context.Module.Id}, content-group:{config.Id}");

    //    Specs = Specs with
    //    {
    //        ViewOrNull = view,
    //    };
    //    return l.Return(Specs, $"ok a:{AppId}, container:{Context.Module.Id}, content-group:{config.Id}");
    //}



    #endregion

    public IBlock? ParentBlockOrNull => Specs.ParentBlockOrNull;

    public IBlock RootBlock => Specs.RootBlock;

    public int ZoneId => Specs.ZoneId;

    public int AppId => Specs.AppId;

    public IApp App => Specs.App;
    public bool DataIsReady => Specs.DataIsReady;
    public IApp? AppOrNull => Specs.AppOrNull;


    // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
    public bool ContentGroupExists => Specs.ContentGroupExists;

    public List<string> BlockFeatureKeys { get; } = [];

    public int ParentId => Specs.ParentId; // This is the module id, not the block id

    public List<ProblemReport> Problems { get; } = [];

    public int ContentBlockId => Specs.ContentBlockId; // This is the content block id, not the module id

    #region Template and extensive template-choice initialization

    // ensure the data is also set correctly...
    // Sequence of determining template
    // 3. If content-group exists, use template definition there
    // 4. If module-settings exists, use that
    // 5. If nothing exists, ensure system knows nothing applied 
    // #. possible override: If specifically defined in some object calls (like web-api), use that (set when opening this object?)
    // #. possible override in url - and allowed by permissions (admin/host), use that
    public IView View => Specs.View;

    public IView? ViewOrNull => Specs.ViewOrNull;

    public bool ViewIsReady => Specs.ViewIsReady;

    public void SwapView(IView value)
    {
        var l = Log.Fn($"set{nameof(value)}");
        Specs = Specs with { ViewOrNull = value };
        Data = null!; // reset this if the view changed...
        l.Done();
    }

    #endregion


    /// <inheritdoc />
    public IContextOfBlock Context => Specs.Context;


    [field: AllowNull, MaybeNull]
    public IDataSource Data
    {
        get => field ??= GetData();
        private set;
    }

    private IDataSource GetData()
    {
        var l = Log.Fn<IDataSource>();
        l.A($"About to load data source with possible app configuration provider. App is probably null: {AppOrNull}");
        var dataSource = Services.BdsFactoryLazy.Value.GetContextDataSource(this, AppOrNull?.ConfigurationProvider);
        return l.Return(dataSource);
    }

    public BlockConfiguration Configuration => Specs.Configuration;

    public bool ConfigurationIsReady => Specs.ConfigurationIsReady;


    public bool IsContentApp => Specs.IsContentApp;

    #region Dependent Apps List so that caching knows what to monitor; relevant for inner-content scenarios

    /// <summary>
    /// This list is only populated on the root builder. Child builders don't actually use this.
    /// </summary>
    public IList<IDependentApp> DependentApps => Specs.DependentApps;

    #endregion


}