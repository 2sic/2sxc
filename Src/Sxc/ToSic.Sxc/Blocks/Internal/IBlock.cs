using ToSic.Eav.Apps;
using ToSic.Sxc.Apps.Internal;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Engines;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Blocks.Internal;

/// <summary>
/// INTERNAL: A unit / block of output in a CMS. 
/// </summary>
[PrivateApi("Was InternalApi_DoNotUse_... till v17")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IBlock: IAppIdentity, IHasLog
{
    /// <summary>
    /// The module ID or the parent-content-block id, probably not ideal here, but not sure
    /// </summary>
    [PrivateApi]
    int ParentId { get; }

    [PrivateApi]
    bool DataIsMissing { get; }

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
    IView View { get; set; }

    [PrivateApi("unsure if this should be public, or only needed to initialize it?")]
    BlockConfiguration Configuration { get; }

    /// <summary>
    /// The app this block is running in
    /// </summary>
    IApp App { get; }

    /// <summary>
    /// The <see cref="IBlockInstance"/> which delivers data for this block (will be used by the <see cref="IEngine"/> together with the View)
    /// </summary>
    IBlockInstance Data { get; }

    [PrivateApi("might rename this some time")]
    bool IsContentApp { get; }
    #endregion

    [PrivateApi("naming not final")]
    IBlockBuilder BlockBuilder { get; }

    [PrivateApi("naming not final")]
    bool ContentGroupExists { get; }

    [PrivateApi("WIP 13.x do get/set if toolbar/context are used")]
    List<string> BlockFeatureKeys { get; }
}