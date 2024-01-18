using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Integration.Modules;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IPlatformModuleUpdater: IHasLog
{
    /// <summary>
    /// Set the App of a Container / Module
    /// </summary>
    /// <param name="instance"></param>
    /// <param name="appId"></param>
    void SetAppId(IModule instance, int? appId);

    /// <summary>
    /// Set the preview view of the Container / Module
    /// </summary>
    /// <param name="instanceId"></param>
    /// <param name="previewView"></param>
    void SetPreview(int instanceId, Guid previewView);

    /// <summary>
    /// Persist the Content-Group once created. 
    /// </summary>
    /// <param name="instanceId"></param>
    /// <param name="blockExists"></param>
    /// <param name="guid"></param>
    void SetContentGroup(int instanceId, bool blockExists, Guid guid);

    /// <summary>
    /// Update the title in the platform
    /// </summary>
    /// <param name="block"></param>
    /// <param name="titleItem"></param>
    void UpdateTitle(IBlock block, IEntity titleItem);
}