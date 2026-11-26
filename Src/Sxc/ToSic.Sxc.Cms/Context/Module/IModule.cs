using ToSic.Sxc.Blocks.Sys;

namespace ToSic.Sxc.Context;

/// <summary>
/// A unit / block within the CMS. Contains all necessary identification to pass around.
/// </summary>
[PrivateApi, ShowApiWhenReleased(ShowApiMode.Never)]
public interface IModule
{
    /// <summary>
    /// Module Id as used in the CMS.
    /// </summary>
    int Id { get; }

    /// <summary>
    /// Determines if this is the primary App (the content-app) as opposed to any additional app
    /// </summary>
    [PrivateApi("don't think this should be here! also not sure if it's the primary - or the contentApp! reason seems to be that we detect it by the DNN module name")]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    bool IsContent { get; }

    /// <summary>
    /// Identifies the content-block which should be shown in this container
    /// </summary>
    [PrivateApi("still experimental")]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    IBlockIdentifier? BlockIdentifier { get; }
}
