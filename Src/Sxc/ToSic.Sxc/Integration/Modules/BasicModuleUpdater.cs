using ToSic.Eav.Internal.Unknown;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Context;
#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Integration.Modules;

/// <summary>
/// Empty constructor for DI
/// </summary>
internal class BasicModuleUpdater(WarnUseOfUnknown<BasicModuleUpdater> _) : ServiceBase($"{LogScopes.NotImplemented}.MapA2I"), IPlatformModuleUpdater
{
    public void SetAppId(IModule instance, int? appId)
    {
        // do nothing
    }

    public void SetPreview(int instanceId, Guid previewTemplateGuid)
    {
        // do nothing
    }

    public void SetContentGroup(int instanceId, bool wasCreated, Guid guid)
    {
        // do nothing
    }

    public void UpdateTitle(IBlock block, IEntity titleItem)
    {
        // do nothing
    }
}