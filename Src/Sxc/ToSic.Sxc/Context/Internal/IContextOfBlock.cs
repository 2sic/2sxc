using ToSic.Eav.Context;
using ToSic.Sxc.Cms.Internal.Publishing;
using ToSic.Sxc.Web.Internal.PageService;

namespace ToSic.Sxc.Context.Internal;

/// <summary>
/// TODO: SHOULD actually be called ContextOfModule!
/// </summary>
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IContextOfBlock: IContextOfApp
{
    /// <summary>
    /// The page it's running on + parameters for queries, url etc.
    /// </summary>
    IPage Page { get; }

    /// <summary>
    /// The container for our block, basically the module
    /// </summary>
    IModule Module { get; }

    /// <summary>
    /// Publishing information about the current context
    /// </summary>
    BlockPublishingSettings Publishing { get; }

    /// <summary>
    /// WIP
    /// </summary>
    PageServiceShared PageServiceShared { get; }

}