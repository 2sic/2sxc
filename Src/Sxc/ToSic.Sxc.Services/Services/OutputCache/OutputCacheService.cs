using ToSic.Sxc.Context;
using ToSic.Sxc.Render.Sys.ModuleHtml;
using ToSic.Sxc.Services.Sys;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Services.OutputCache;

// Note 2dm 2025-06 - this doesn't seem to be in use anywhere!
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class OutputCacheService(IModuleHtmlService moduleHtmlService)
    : ServiceWithContext("Sxc.OutCac", connect: [moduleHtmlService]), IOutputCacheService
{
    public int ModuleId
    {
        get => _moduleId ??= ExCtx.GetCmsContext()?.Module?.Id ?? 0;
        set => _moduleId = value;
    }
    private int? _moduleId;

    public string Disable()
        => Configure(new() { IsEnabled = false });

    public string Enable(bool enable = true)
        => Configure(new() { IsEnabled = enable });

    public string Configure(OutputCacheSettings settings)
    {
        ((ModuleHtmlService)moduleHtmlService).ConfigureOutputCache(ModuleId, settings);
        return "";
    }
}