using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Sxc.Cms.Publishing.Sys;
using ToSic.Sxc.Sys.Render.PageContext;

namespace ToSic.Sxc.Context.Sys;

/// <summary>
/// TODO: SHOULD actually be called ContextOfModule!
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
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
    IPageServiceShared PageServiceShared { get; }

    /// <summary>
    /// Create a clone of the context, usually for then making a slightly different context
    /// </summary>
    /// <returns></returns>
    IContextOfBlock Clone(ILog newParentLog, IAppIdentity appIdentity);


}