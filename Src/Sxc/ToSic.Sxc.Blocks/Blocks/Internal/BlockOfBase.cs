using ToSic.Eav.Apps.Sys;
using ToSic.Eav.DataSource;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks.Internal.Render;
using ToSic.Sxc.Context.Internal;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Blocks.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
public abstract class BlockOfBase(BlockGeneratorHelpers services, string logName, object[]? connect = default)
    : ServiceBase<BlockGeneratorHelpers>(services, logName, connect: connect ?? []) // , IBlock
{
    // New: WIP replacing the block with a stateless record
    public BlockSpecs Specs { get; protected set; } = null!;


    public IBlock? ParentBlockOrNull => Specs.ParentBlockOrNull;

    public IBlock RootBlock => Specs.RootBlock;

    public int ZoneId => Specs.ZoneId;

    public int AppId => Specs.AppId;

    public IApp App => Specs.App;
    public bool DataIsReady => Specs.DataIsReady;
    public IApp? AppOrNull => Specs.AppOrNull;


    // ReSharper disable once ConditionalAccessQualifierIsNonNullableAccordingToAPIContract
    public bool ContentGroupExists => Specs.ContentGroupExists;

    public List<string> BlockFeatureKeys => Specs.BlockFeatureKeys;

    public int ParentId => Specs.ParentId; // This is the module id, not the block id

    public List<ProblemReport> Problems => Specs.Problems;

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

    #endregion


    /// <inheritdoc />
    public IContextOfBlock Context => Specs.Context;


    [field: AllowNull, MaybeNull]
    public IDataSource Data
    {
        get => field ??= GetData();
        private set;
    }

    protected IDataSource GetData()
    {
        var l = Log.Fn<IDataSource>($"About to load data source with possible app configuration provider. App is probably null: {Specs.AppOrNull.Show()}");
        var dataSource = Services.BdsFactoryLazy.Value.GetContextDataSource(Specs, Specs.AppOrNull?.ConfigurationProvider);
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