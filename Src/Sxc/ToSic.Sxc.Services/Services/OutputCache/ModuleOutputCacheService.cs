using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Render.Output.Sys;

namespace ToSic.Sxc.Services.OutputCache;

/// <summary>
/// Implementation of the Kit.OutputCache - to configure how the resulting module HTML is cached.
/// </summary>
/// <param name="modulesOutputService"></param>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class ModuleOutputCacheService(Ctx<IBlock> ctxBlock, IModulesOutputService modulesOutputService)
    : ServiceBase("Sxc.OutCac", connect: [modulesOutputService]), IModuleOutputCacheService
{
    [PrivateApi("internal use only, external API should not know about this.")]
    public int ModuleId
    {
        get => _moduleId ??= ctxBlock.ValueOptional?.Context?.Module?.Id ?? 0;
        set => _moduleId = value;
    }
    private int? _moduleId;

    public string Disable()
        => Configure(new() { IsEnabled = false });

    public string Enable(bool enable = true)
        => Configure(new() { IsEnabled = enable });

    /// <inheritdoc/>
    public string DependOn(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException("Dependency key must not be empty.", nameof(key));

        modulesOutputService.AddOutputCacheDependency(ModuleId, key);
        return "";
    }

    /// <inheritdoc/>
    public string Configure(OutputCacheSettings settings)
    {
        modulesOutputService.ConfigureOutputCache(ModuleId, settings);
        return "";
    }
}
