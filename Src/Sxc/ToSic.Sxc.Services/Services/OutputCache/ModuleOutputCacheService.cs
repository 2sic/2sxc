using ToSic.Sxc.Render.Sys.ModuleHtml;
using ToSic.Sxc.Services.Sys;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Services.OutputCache;

// Note 2dm 2025-06 - this doesn't seem to be in use anywhere!
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class ModuleOutputCacheService(IModuleHtmlService moduleHtmlService)
    : ServiceWithContext("Sxc.OutCac", connect: [moduleHtmlService]), IModuleOutputCacheService
{
    [PrivateApi("internal use only, external API should not know about this.")]
    public int ModuleId
    {
        get => _moduleId ??= ExCtxOrNull?.GetBlock()?.Context?.Module?.Id ?? 0;
        set => _moduleId = value;
    }
    private int? _moduleId;

    public string Disable()
        => Configure(new() { IsEnabled = false });

    public string Enable(bool enable = true)
        => Configure(new() { IsEnabled = enable });

    public string DependOn(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Dependency key must not be empty.", nameof(key));

        ((ModuleHtmlService)moduleHtmlService).AddOutputCacheDependency(ModuleId, key);
        return "";
    }

    public string Configure(OutputCacheSettings settings)
    {
        ((ModuleHtmlService)moduleHtmlService).ConfigureOutputCache(ModuleId, settings);
        return "";
    }
}
