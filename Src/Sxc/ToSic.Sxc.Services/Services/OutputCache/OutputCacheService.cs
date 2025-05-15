using ToSic.Sxc.Context;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Services.OutputCache;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class OutputCacheService(IModuleService moduleService)
    : ServiceForDynamicCode("Sxc.OutCac", connect: [moduleService]), IOutputCacheService
{
    public int ModuleId
    {
        get => _moduleId ??= ExCtx?.GetState<ICmsContext>()?.Module?.Id ?? 0;
        set => _moduleId = value;
    }
    private int? _moduleId;

    public string Disable() => Configure(new() { IsEnabled = false });

    public string Enable(bool enable = true) => Configure(new() { IsEnabled = enable });

    public string Configure(OutputCacheSettings settings)
    {
        ((ModuleService)moduleService).ConfigureOutputCache(ModuleId, settings);
        return "";
    }
}