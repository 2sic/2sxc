using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using System.Text;
using System.Text.Json;
using ToSic.Eav;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.AppJson;
using ToSic.Eav.Apps.Sys.Caching;
using ToSic.Eav.Apps.Sys.FileSystemState;
using ToSic.Eav.Apps.Sys.Loaders;
using ToSic.Eav.Run.Startup;
using ToSic.Eav.Sys;
using ToSic.Sxc.Data;
using ToSic.Sxc.DataSources;
using ToSic.Sxc.Services;
using ToSic.Sys.Configuration;

namespace ToSic.Sxc.WebApi.Tests.Extensions;

public class StartupExtensionsTests : StartupTestsEavDataBuild
{
    public override void ConfigureServices(IServiceCollection services)
    {
        base.ConfigureServices(services);
        services.AddDataSourceSystem();
        services.AddContextFallbacks();

        services.RemoveAll<IAppJsonConfigurationService>();
        services.RemoveAll<IGlobalConfiguration>();

        services.AddTransient<IAppReaderFactory, ExtensionsTestAppReaderFactory>();
        services.AddTransient<IJsonService, ExtensionsTestJsonService>();
        services.AddTransient<IGlobalConfiguration, ExtensionsTestGlobalConfiguration>();
        services.AddSingleton<ExtensionsTestAppJsonConfigurationService>();
        services.AddSingleton<IAppJsonConfigurationService>(sp => sp.GetRequiredService<ExtensionsTestAppJsonConfigurationService>());
        services.AddSingleton<IAppsCatalog, ExtensionsTestAppsCatalog>();
        services.AddSingleton<AppsCacheSwitch, ExtensionsTestAppsCacheSwitch>();
        services.AddTransient<AppCachePurger>();
        services.AddTransient<ExtensionManifestService>();
        services.AddTransient<AppEditions>();
    }
}

internal sealed class ExtensionsTestAppReaderFactory : IAppReaderFactory
{
    public IAppReader Get(int appId)
        => null!;
    public IAppReader Get(IAppIdentity appIdentity)
        => null!;
    public IAppReader GetSystemPreset()
        => null!;
    public IAppIdentityPure AppIdentity(int appId)
        => new AppIdentityPure(1, appId);
    public IAppReader GetZonePrimary(int zoneId)
        => throw new NotImplementedException();
    public IAppReader? TryGet(IAppIdentity appIdentity)
        => null;
    public IAppReader? ToReader(IAppStateCache? state)
        => null;
    public IAppReader? TryGetSystemPreset(bool nullIfNotLoaded)
        => null;
    public IAppReader GetOrKeep(IAppIdentity appIdOrReader)
        => throw new NotImplementedException();
}

internal sealed class ExtensionsTestJsonService : IJsonService
{
    private static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = null,
        WriteIndented = true
    };

    public string ToJson(object item) => item switch
    {
        string s => s,
        JsonElement je => je.GetRawText(),
        _ => JsonSerializer.Serialize(item, Options)
    };
    public string ToJson(object item, int indentation)
        => JsonSerializer.Serialize(item, new JsonSerializerOptions(Options) { WriteIndented = indentation > 0 });
    public T? To<T>(string json)
        => JsonSerializer.Deserialize<T>(json, Options);
    public object? ToObject(string json)
    {
        try
        {
            using var doc = JsonDocument.Parse(json);
            return doc.RootElement.Clone();
        }
        catch
        {
            return null;
        }
    }
    public ITyped ToTyped(string json, NoParamOrder noParamOrder = default, string? fallback = default, bool? propsRequired = default)
        => throw new NotImplementedException();
    public IEnumerable<ITyped> ToTypedList(string json, NoParamOrder noParamOrder = default, string? fallback = default, bool? propsRequired = default)
        => throw new NotImplementedException();
}

internal sealed class ExtensionsTestGlobalConfiguration : IGlobalConfiguration
{
    private readonly Dictionary<string, string?> _values = new();

    public string? GetThis(string? key = null)
        => _values.TryGetValue(key!, out var value) ? value : null;

    public string? GetThisOrSet(Func<string> generator, string? key = null)
    {
        if (!_values.TryGetValue(key!, out var value))
        {
            value = generator();
            _values[key!] = value;
        }
        return value;
    }

    public string GetThisErrorOnNull(string? key = null)
        => GetThis(key) ?? throw new InvalidOperationException($"Config key '{key}' is null");

    public string? SetThis(string? value, string? key = null)
    {
        _values[key!] = value;
        return value;
    }
}

public sealed class ExtensionsTestAppJsonConfigurationService : IAppJsonConfigurationService
{
    private readonly AsyncLocal<State?> _state = new();

    public void UseAppRoot(string appRoot)
    {
        _state.Value = new(appRoot, DefaultConfiguration());
        PersistConfiguration();
    }

    public void MoveAppJsonTemplateFromOldToNewLocation()
    {
    }

    public AppJsonConfiguration? GetAppJson(int appId, bool useShared)
        => StateForTest.Configuration;

    public string AppJsonCacheKey(int appId, bool useShared)
        => string.Empty;

    public ICollection<string> ExcludeSearchPatterns(string sourceFolder, int appId, bool useShared)
        => Array.Empty<string>();

    public void EnsureEdition(string editionName)
    {
        if (editionName == null)
            return;

        var state = StateForTest;
        if (state.Configuration.Editions.ContainsKey(editionName))
            return;

        state.Configuration.Editions[editionName] = new AppJsonConfiguration.EditionInfo();
        PersistConfiguration();
    }

    private State StateForTest
        => _state.Value ??= new(Path.GetTempPath(), DefaultConfiguration());

    private static AppJsonConfiguration DefaultConfiguration()
        => new()
        {
            IsConfigured = true,
            Editions =
            {
                [string.Empty] = new AppJsonConfiguration.EditionInfo
                {
                    Description = "Root edition shared by all variants.",
                    IsDefault = true
                }
            }
        };

    private void PersistConfiguration()
    {
        var state = StateForTest;
        var appData = Path.Combine(state.AppRoot, FolderConstants.DataFolderProtected);
        Directory.CreateDirectory(appData);
        var appJsonPath = Path.Combine(appData, FolderConstants.AppJsonFile);
        var json = JsonSerializer.Serialize(state.Configuration, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(appJsonPath, json, new UTF8Encoding(false));
    }

    private sealed record State(string AppRoot, AppJsonConfiguration Configuration);
}

internal sealed class ExtensionsTestAppsCatalog : IAppsCatalog
{
    public IReadOnlyDictionary<int, string> Apps(int zoneId)
        => new Dictionary<int, string>();

    public IReadOnlyDictionary<int, Zone> Zones
        => new Dictionary<int, Zone>();

    public Zone Zone(int zoneId)
        => new(zoneId, 1, 2, new Dictionary<int, string>(), []);

    public IAppIdentityPure DefaultAppIdentity(int zoneId)
        => new AppIdentityPure(zoneId, 1);

    public IAppIdentityPure PrimaryAppIdentity(int zoneId)
        => new AppIdentityPure(zoneId, 2);

    public IAppIdentityPure AppIdentity(int appId)
        => new AppIdentityPure(1, appId);

    public string AppNameId(IAppIdentity appIdentity)
        => Guid.NewGuid().ToString();
}

internal sealed class ExtensionsTestAppsCacheSwitch : AppsCacheSwitch
{
    public ExtensionsTestAppsCacheSwitch() : base(null!, null!, null!, null!)
    {
        var field = typeof(AppsCacheSwitch).GetField("_value", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
        var getOnce = field?.GetValue(this);
        var resetMethod = getOnce?.GetType().GetMethod("Reset", [typeof(IAppsCacheSwitchable)]);
        resetMethod?.Invoke(getOnce, [new ExtensionsTestAppsCacheSwitchable()]);
    }
}

internal sealed class ExtensionsTestAppsCacheSwitchable : IAppsCacheSwitchable
{
    public void Purge(IAppIdentity app) { }
    public void PurgeZones() { }
    public IAppStateCache Get(IAppIdentity app, IAppLoaderTools tools) => null!;
    IReadOnlyDictionary<int, Zone> IAppsCache.Zones(IAppLoaderTools tools) => new Dictionary<int, Zone>();
    int IAppsCache.ZoneIdOfApp(int appId, IAppLoaderTools tools) => 1;
    public bool Has(IAppIdentity app) => false;
    void IAppsCache.Update(IAppIdentity app, IEnumerable<int> entities, ILog log, IAppLoaderTools tools) { }
    public void Add(IAppStateCache appState) { }
    public void Load(IAppIdentity app, string primaryLanguage, IAppLoaderTools tools) { }
    public bool IsViable() => true;
    public int Priority { get; } = 0;
    public string NameId { get; } = "Fake";
}
