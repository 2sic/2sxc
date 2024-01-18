using ToSic.Eav.Cms.Internal;

namespace ToSic.Sxc.Context;

/// <summary>
/// A unit / block within the CMS. Contains all necessary identification to pass around.
/// </summary>
[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IModule
#if NETFRAMEWORK
#pragma warning disable 618
        // in this case we must also inherit from IContainer - legacy of the signature for CustomizeSearch
        : ToSic.Eav.Run.IContainer
#pragma warning restore 618
#endif
{
    [PrivateApi("Workaround till we have DI to inject the current container")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    IModule Init(int id);

    int Id { get; }


    /// <summary>
    /// Determines if this is a the primary App (the content-app) as opposed to any additional app
    /// </summary>
    [PrivateApi("don't think this should be here! also not sure if it's the primary - or the contentApp! reason seems to be that we detect it by the DNN module name")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    bool IsContent { get; }

    /// <summary>
    /// Identifies the content-block which should be shown in this container
    /// </summary>
    [PrivateApi("still experimental")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    IBlockIdentifier BlockIdentifier { get; }
}