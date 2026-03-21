using ToSic.Eav.Apps;
using ToSic.Sxc.Render.Sys.ModuleHtml;
using ToSic.Sxc.Services.Cache;
using ToSic.Sxc.Services.Sys;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Services.OutputCache;

// Note 2dm 2025-06 - this doesn't seem to be in use anywhere!
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class ModuleOutputCacheService(IModuleHtmlService moduleHtmlService, INamedCacheDependencyService namedDependencies)
    : ServiceWithContext("Sxc.OutCac", connect: [moduleHtmlService, namedDependencies]), IModuleOutputCacheService
{
    [PrivateApi("internal use only, external API should not know about this.")]
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

    public int Flush(IEnumerable<string>? dependencies = null)
    {
        var appId = ResolveAppId();
        var normalized = dependencies?.ToArray();
        if (normalized == null || normalized.Length == 0)
        {
            namedDependencies.TouchApp(CacheDependencyScopes.OutputCache, appId);
            return 0;
        }

        return namedDependencies.Touch(CacheDependencyScopes.OutputCache, appId, normalized);
    }

    // Flush methods target app-scoped output-cache dependency markers, so they must resolve an app
    // even though the render-time Configure/DependOn calls only buffer module-specific state.
    private int ResolveAppId()
    {
        if (ExCtxOrNull == null)
            throw new InvalidOperationException("OutputCache flush requires an execution context.");

        return ExCtxOrNull.GetBlock()?.AppId
               ?? TryGetAttachedAppId()
               ?? TryGetAppId()
               ?? throw new InvalidOperationException("OutputCache flush requires a current app.");
    }

    // Some execution contexts, especially mocked or explicitly attached ones, carry the current app
    // without also providing a block or IAppReader state.
    private int? TryGetAttachedAppId()
    {
        try
        {
            return ExCtxOrNull?.GetApp().AppId;
        }
        catch
        {
            return null;
        }
    }

    private int? TryGetAppId()
    {
        try
        {
            return ExCtxOrNull?.GetState<IAppReader>().AppId;
        }
        catch
        {
            return null;
        }
    }
}
