using ToSic.Eav.Apps;
using ToSic.Eav.DataSource;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Web.Internal.PageFeatures;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Blocks.Internal;

/// <summary>
/// INTERNAL: A unit / block of output in a CMS. 
/// </summary>
[PrivateApi("Was InternalApi_DoNotUse_... till v17")]
[ShowApiWhenReleased(ShowApiMode.Never)]
public record BlockInfoWip : IAppIdentity, IBlock
{
    public int AppId { get; init; }
    public int ZoneId { get; init; }


    /// <summary>
    /// The module ID or the parent-content-block id, probably not ideal here, but not sure
    /// </summary>
    public int ParentId { get; init; }

    public List<ProblemReport> Problems { get; init; }

    public int ContentBlockId { get; init; }

    #region Values related to the current unit of content / the view
        
    /// <summary>
    /// The context we're running in, with tenant, container etc.
    /// </summary>
    public IContextOfBlock Context { get; init; }

    /// <summary>
    /// The view which will be used to render this block
    /// </summary>
    public IView View { get; set; } // WIP!

    public bool ViewIsReady { get; init; }

    [PrivateApi("unsure if this should be public, or only needed to initialize it?")]
    public BlockConfiguration Configuration { get; init; }

    /// <summary>
    /// The app this block is running in
    /// </summary>
    public IApp App { get; init; }

    /// <summary>
    /// The DataSource which delivers data for this block (will be used by the <see cref="IEngine"/> together with the View)
    /// </summary>
    public IDataSource Data { get; init; }

    [PrivateApi("might rename this some time")]
    public bool IsContentApp { get; init; }

    #endregion

    ///// <summary>
    ///// The BlockBuilder.
    ///// Must be stored on this object, but the type is in another project so here it's untyped.
    ///// Access to this must go through the extension method `GetBlockBuilder()`
    ///// </summary>
    //[PrivateApi("naming not final")]
    //object BlockBuilder { get; }

    public bool ContentGroupExists { get; }

    /// <summary>
    /// All the keys / features which were added in this block; in case the block should also modify its behavior.
    /// </summary>
    public List<string> BlockFeatureKeys { get; init; }

    /// <summary>
    /// The parent block of this block, if any.
    /// </summary>
    [PrivateApi]
    public IBlock? ParentBlock { get; init; }

    /// <summary>
    /// The root block of this block - can be the same as `this`.
    /// </summary>
    [PrivateApi]
    public IBlock? RootBlock { get; init; }

    public bool DataIsReady { get; init; }
    public bool ConfigurationIsReady { get; init; }
    public IApp? AppOrNull { get; init; }
    public List<IPageFeature> BlockFeatures(ILog? log = default)
    {
        throw new NotImplementedException();
    }


    //List<IPageFeature> BlockFeatures(ILog? log = default);
    public ILog? Log { get; }
}