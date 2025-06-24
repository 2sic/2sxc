using ToSic.Eav.Apps;
using ToSic.Eav.DataSource;
using ToSic.Sxc.Blocks.Internal.Render;
using ToSic.Sxc.Context.Internal;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Blocks.Internal;

/// <summary>
/// INTERNAL: A unit / block of output in a CMS. 
/// </summary>
[PrivateApi("Was InternalApi_DoNotUse_... till v17")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IBlock: IAppIdentity //, IHasLog
{

    //// WIP
    //IEntity? Entity { get; }

    public bool IsInnerBlock { get; }

    /// <summary>
    /// The module ID or the parent-content-block id, probably not ideal here, but not sure
    /// </summary>
    [PrivateApi]
    int ParentId { get; }

    [PrivateApi]
    List<ProblemReport> Problems { get; }

    [PrivateApi]
    int ContentBlockId { get; }

    #region Values related to the current unit of content / the view
        
    /// <summary>
    /// The context we're running in, with tenant, container etc.
    /// </summary>
    IContextOfBlock Context { get; }

    /// <summary>
    /// The view which will be used to render this block
    /// </summary>
    IView View { get; }
    bool ViewIsReady { get; }

    [PrivateApi("unsure if this should be public, or only needed to initialize it?")]
    BlockConfiguration Configuration { get; }

    /// <summary>
    /// The app this block is running in
    /// </summary>
    IApp App { get; }

    /// <summary>
    /// The DataSource which delivers data for this block (will be used by the <see cref="IEngine"/> together with the View)
    /// </summary>
    IDataSource Data { get; }

    [PrivateApi("might rename this some time")]
    bool IsContentApp { get; }
    #endregion

    ///// <summary>
    ///// The BlockBuilder.
    ///// Must be stored on this object, but the type is in another project so here it's untyped.
    ///// Access to this must go through the extension method `GetBlockBuilder()`
    ///// </summary>
    //[PrivateApi("naming not final")]
    //object BlockBuilder { get; }

    [PrivateApi("naming not final")]
    bool ContentGroupExists { get; }

    /// <summary>
    /// All the keys / features which were added in this block; in case the block should also modify its behavior.
    /// </summary>
    [PrivateApi("WIP 13.x do get/set if toolbar/context are used")]
    List<string> BlockFeatureKeys { get; }

    /// <summary>
    /// The parent block of this block, if any.
    /// </summary>
    [PrivateApi]
    IBlock? ParentBlockOrNull { get; }

    /// <summary>
    /// The root block of this block - can be the same as `this`.
    /// </summary>
    [PrivateApi]
    public IBlock RootBlock { get; }

    bool DataIsReady { get; }
    bool ConfigurationIsReady { get; }
    IApp? AppOrNull { get; }

    /// <summary>
    /// This list is only populated on the root builder. Child builders don't actually use this.
    /// </summary>
    IList<IDependentApp> DependentApps { get; }


    //List<IPageFeature> BlockFeatures(ILog? log = default);
    //BlockSpecs SwapView(IView value);
}