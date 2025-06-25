using ToSic.Eav.Apps.Sys;
using ToSic.Eav.DataSource;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Blocks.Sys.Problems;
using ToSic.Sxc.Context.Sys;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Blocks.Sys;

/// <summary>
/// INTERNAL: A unit / block of output in a CMS. 
/// </summary>
[PrivateApi("Was InternalApi_DoNotUse_... till v17")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public record BlockSpecs : IBlock
{
    public required int AppId { get; init; }
    public required int ZoneId { get; init; }

    public required bool IsInnerBlock { get; init; }

    /// <summary>
    /// The module ID or the parent-content-block id, probably not ideal here, but not sure
    /// </summary>
    public int ParentId => Context.Module?.Id ?? 0;

    public List<ProblemReport> Problems { get; init; } = [];

    public int ContentBlockId { get; init; }

    #region Values related to the current unit of content / the view
        
    /// <summary>
    /// The context we're running in, with tenant, container etc.
    /// </summary>
    public required IContextOfBlock Context { get; init; }

    /// <summary>
    /// The view which will be used to render this block
    /// </summary>
    public IView View => ViewOrNull ?? throw new($"View is not available and can't be accessed. Rode running early on accessing the view, must first check for {nameof(ViewIsReady)}");
    public IView? ViewOrNull { get; /*init;*/ set; }
    public bool ViewIsReady => ViewOrNull != null;

    public BlockConfiguration Configuration
    {
        get => _configuration ?? throw new($"BlockConfiguration is not available and can't be accessed. Code running early on must first check for {nameof(ConfigurationIsReady)}");
        init => _configuration = value;
    }

    private readonly BlockConfiguration? _configuration;
    public bool ConfigurationIsReady => _configuration != null;

    /// <summary>
    /// The app this block is running in
    /// </summary>
    public IApp App => AppOrNull
                       ?? throw new($"App and Data are missing and can't be accessed. Code running early on must first check for .{nameof(DataIsReady)} or use {nameof(AppOrNull)}");
    public bool DataIsReady => AppOrNull != null;
    public IApp? AppOrNull { get; init; }

    /// <summary>
    /// The DataSource which delivers data for this block (will be used by the Engine together with the View)
    /// </summary>
    [field: AllowNull, MaybeNull]
    public IDataSource Data
    {
        get => field ?? throw new NullReferenceException($"Can't access {nameof(Data)}, it was never properly initialized.");// ??= GetData();
        set;
    } = null!;

    public required bool IsContentApp { get; init; }

    #endregion

    public bool ContentGroupExists => _configuration?.Exists ?? false; // This check may happen before Configuration is accessed, so we need the null check

    /// <summary>
    /// All the keys / features which were added in this block; in case the block should also modify its behavior.
    /// </summary>
    public List<string> BlockFeatureKeys { get; init; } = [];

    /// <summary>
    /// The parent block of this block, if any.
    /// </summary>
    [PrivateApi]
    public IBlock? ParentBlockOrNull { get; init; }

    /// <summary>
    /// The root block of this block - can be the same as `this`.
    /// </summary>
    [PrivateApi]
    [field: AllowNull, MaybeNull]
    public IBlock RootBlock
    {
        get => field ?? this; // never store the result, as the fallback should still return me-object in future if never set.
        init;
    }

    public IList<IDependentApp> DependentApps { get; } = [];

    //List<IPageFeature> BlockFeatures(ILog? log = default);
    //public ILog? Log { get; }

    /// <summary>
    /// Must override ToString, otherwise any ToString() will result in properties being accessed, which throw because they are not available yet.
    /// </summary>
    public override string ToString()
        => $"Block with App: {this.Show()}; IsInner: {IsInnerBlock}; IsContent: {IsContentApp}; View: {ViewIsReady} {ViewOrNull}; Parent: {ParentId}; Block: {ContentBlockId}";
}