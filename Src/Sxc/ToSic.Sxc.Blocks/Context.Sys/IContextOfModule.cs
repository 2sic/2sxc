using ToSic.Eav.Context;
using ToSic.Sxc.Cms.Publishing.Sys;
using ToSic.Sxc.Sys.Render.PageContext;

namespace ToSic.Sxc.Context.Sys;

/// <summary>
/// The entire context of a module, with Page, Module, Publishing etc.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IContextOfModule: IContextOfApp
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
}