using System.IO;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Integration;
using ToSic.Eav.Caching;
using ToSic.Eav.Context;
using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Internal;
using ToSic.Eav.DataSource.Internal.AppDataSources;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Eav.DataSource.VisualQuery.Internal;
using ToSic.Eav.Helpers;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Code.Internal.HotBuild;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Polymorphism.Internal;

namespace ToSic.Sxc.DataSources.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class AppDataSourcesLoader(
    ILogStore logStore,
    ISite site,
    IAppReaderFactory appReaders,
    LazySvc<IAppPathsMicroSvc> appPathsLazy,
    LazySvc<CodeCompiler> codeCompilerLazy,
    LazySvc<AppCodeLoader> appCodeLoaderLazy,
    ISxcContextResolver ctxResolver,
    PolymorphConfigReader polymorphism,
    MemoryCacheService memoryCacheService)
    : ServiceBase("Eav.AppDtaSrcLoad",
        connect:
        [
            logStore, site, appReaders, appPathsLazy, codeCompilerLazy, appCodeLoaderLazy, ctxResolver, polymorphism,
            memoryCacheService
        ]), IAppDataSourcesLoader
{
    private const string DataSourcesFolder = "DataSources";

    private readonly MemoryCacheService _memoryCacheService = memoryCacheService;

    public (List<DataSourceInfo> data, TimeSpan slidingExpiration, IList<string> folderPaths, IEnumerable<string> cacheKeys) CompileDynamicDataSources(int appId)
    {
        logStore.Add(EavLogs.LogStoreAppDataSourcesLoader, Log);
        // Initial message for insights-overview
        var l = Log.Fn<(List<DataSourceInfo> data, TimeSpan slidingExpiration, IList<string> folderPaths, IEnumerable<string> cacheKeys)>($"{nameof(appId)}: {appId}", timer: true);
        
        try
        {
            var spec = BuildHotBuildSpec(appId);
            l.A($"{spec}");

            // 1. Get Custom Dynamic DataSources from 'AppCode' assembly
            var data = CreateDataSourceInfos(appId, LoadAppCodeDataSources(spec, out var cacheKey));
            l.A($"Custom Dynamic DataSources in {Constants.AppCode}:{data.Count}");

            // 2. Get Custom Dynamic DataSources from 'DataSources' folder
            var (physicalPath, relativePath) = GetAppDataSourceFolderPaths(appId);
            var physicalPathExists = Directory.Exists(physicalPath);
            if (physicalPathExists)
            {
                var dsInDataSources = CreateDataSourceInfos(appId, LoadAppDataSources(spec, physicalPath, relativePath));
                l.A($"Custom Dynamic DataSources in {DataSourcesFolder}:{dsInDataSources.Count}");
                data = data.Concat(dsInDataSources).ToList();
            }

            l.A($"Total Custom Dynamic DataSources:{data.Count}");

            // If the directory doesn't exist, return an empty list with a 3 minute policy
            // just so we don't keep trying to do this on every query
            if (!data.Any() && !physicalPathExists)
                return l.Return((
                            data: [],
                            slidingExpiration: new(0, 5, 0),
                            folderPaths: null,
                            cacheKeys: null), 
                            "error, directory do not exist");

            return l.ReturnAsOk((
                        data: data,
                        slidingExpiration: new TimeSpan(1, 0, 0),
                        folderPaths: physicalPathExists ? [physicalPath] : null,
                        cacheKeys: (data.Any() && !string.IsNullOrEmpty(cacheKey)) ? [cacheKey] : null // cache dependency on existing cache item with AppCode assembly
                        ));
        }
        catch
        {
            return l.ReturnAsError((
                        data: [],
                        slidingExpiration: new(0, 5, 0),
                        folderPaths: null,
                        cacheKeys: null));
        }
    }

    private HotBuildSpec BuildHotBuildSpec(int appId)
    {
        var l = Log.Fn<HotBuildSpec>($"{appId}:'{appId}'", timer: true);

        // Prepare / Get App State
        var appState = appReaders.Get(appId).Specs;

        // Figure out the current edition
        var edition = FigureEdition().TrimLastSlash();

        var spec = new HotBuildSpec(appState?.AppId ?? Eav.Constants.AppIdEmpty, edition: edition, appState?.Name);

        return l.ReturnAsOk(spec);
    }

    private string FigureEdition()
    {
        var l = Log.Fn<string>(timer: true);

        var block = ctxResolver.BlockOrNull();
        var edition = block.NullOrGetWith(polymorphism.UseViewEditionOrGet);

        return l.Return(edition);
    }

    private (string physicalPath, string relativePath) GetAppDataSourceFolderPaths(int appId)
    {
        var appReader = appReaders.Get(appId);
        var appPaths = appPathsLazy.Value.Get(appReader, site);
        var physicalPath = Path.Combine(appPaths.PhysicalPath, DataSourcesFolder);
        var relativePath = Path.Combine(appPaths.RelativePath, DataSourcesFolder);
        return (physicalPath, relativePath);
    }

    private class TempDsInfo
    {
        public string ClassName;
        public Type Type;
        public DataSourceInfoError Error;
    }

    /// <summary>
    /// Load Custom Dynamic DataSources from 'AppCode' assembly
    /// </summary>
    /// <param name="spec"></param>
    /// <param name="cacheKey">return for CacheEntryChangeMonitor</param>
    /// <returns></returns>
    private IEnumerable<TempDsInfo> LoadAppCodeDataSources(HotBuildSpec spec, out string cacheKey)
    {
        var l = Log.Fn<IEnumerable<TempDsInfo>>();

        l.A("Search for DataSources in AppCode");
        var (result, _) = appCodeLoaderLazy.Value.GetAppCode(spec);

        cacheKey = result?.CacheDependencyId; // return for CacheEntryChangeMonitor

        var appCodeAssembly = result?.Assembly;
        if (appCodeAssembly == null)
            return l.Return([], "no AppCode assembly found");

        l.A($"AppCode:{appCodeAssembly.GetName().Name}");

        // find all types in the assembly that are derived from IAppDataSource
        var types = appCodeAssembly.GetTypes()
            .Where(t => typeof(Custom.DataSource.DataSource16).IsAssignableFrom(t) && !t.IsAbstract)
            .Select(type =>
            {
                var className = type.Name;
                try
                {
                    return new() { ClassName = className, Type = type };
                }
                catch (Exception ex)
                {
                    l.Ex(ex);
                    return new TempDsInfo { ClassName = className, Error = new("Unknown Exception", ex.Message) };
                }
            })
            .ToList();

        return types.Any()
            ? l.Return(types, $"OK, DataSources:{types.Count} ({string.Join(";", types.Select(t => t.ClassName))}) in AppCode:{appCodeAssembly.GetName().Name}")
            : l.Return(types, $"OK, no working DataSources found in AppCode:{appCodeAssembly.GetName().Name}");
    }

    /// <summary>
    /// Load Custom Dynamic DataSources from 'DataSources' folder
    /// </summary>
    /// <param name="spec"></param>
    /// <param name="physicalPath"></param>
    /// <param name="relativePath"></param>
    /// <returns></returns>
    private IEnumerable<TempDsInfo> LoadAppDataSources(HotBuildSpec spec, string physicalPath, string relativePath)
    {
        var l = Log.Fn<IEnumerable<TempDsInfo>>(
            $"{spec}; {nameof(physicalPath)}: '{physicalPath}'; {nameof(relativePath)}: '{relativePath}'");

        if (!Directory.Exists(physicalPath))
            return l.Return([], $"no {DataSourcesFolder} folder {physicalPath}");

        var compiler = codeCompilerLazy.Value;

        var types = Directory
            .GetFiles(physicalPath, "*.cs", SearchOption.TopDirectoryOnly)
            .Select(dataSourceFile =>
            {
                var className = Path.GetFileNameWithoutExtension(dataSourceFile);
                try
                {
                    var (type, errorMessages) = compiler.GetTypeOrErrorMessages(
                        relativePath: Path.Combine(relativePath, Path.GetFileName(dataSourceFile)),
                        className: className,
                        throwOnError: false,
                        spec: spec);

                    if (!errorMessages.HasValue())
                        return new() { ClassName = className, Type = type };
                    l.E(errorMessages);
                    return new()
                    {
                        ClassName = className,
                        Type = type,
                        Error = new("Error Compiling", errorMessages)
                    };
                }
                catch (Exception ex)
                {
                    l.Ex(ex);
                    return new TempDsInfo { ClassName = className, Error = new("Unknown Exception", ex.Message) };
                }
            })
            .ToList();

        return types.Any()
            ? l.Return(types, $"OK, DataSources:{types.Count} ({string.Join(";", types.Select(t => t.ClassName))}), path:{relativePath}")
            : l.Return(types, $"OK, no working DataSources found, path:{relativePath}");
    }

    private List<DataSourceInfo> CreateDataSourceInfos(int appId, IEnumerable<TempDsInfo> types)
    {
        var l = Log.Fn<List<DataSourceInfo>>($"{nameof(appId)}: {appId}; has {nameof(types)}: {types != null}");

        // null check
        if (types == null) return l.Return([], "types are null");

        // App state for automatic lookup of configuration content-types
        var appState = appReaders.Get(appId);
        var data = types
            .Select(pair =>
            {
                var l2 = l.Fn<DataSourceInfo>(pair.ClassName);

                // 0. If error then type is null, in this case, return a specially crafted DSI
                if (pair.Type == null)
                    return l2.Return(DataSourceInfo.CreateError(pair.ClassName, false, DataSourceType.App, pair.Error), "error: type==null");

                // 1. Make sure we only keep DataSources and not other classes in the same folder
                // but assume all null-types are errors, which we should preserve
                if (!typeof(IDataSource).IsAssignableFrom(pair.Type))
                    return l2.ReturnNull($"error: not a {nameof(IDataSource)}");

                // 2. Get VisualQuery Attribute if available, or create new, since it's optional in DynamicCode
                var vq = pair.Type.GetDirectlyAttachedAttribute<VisualQueryAttribute>()
                         ?? new VisualQueryAttribute();

                var typeName = pair.Type.Name;

                // 3. Update various properties which are needed for further functionality
                // The global name is always necessary
                vq.NameId = vq.NameId.NullIfNoValue() ?? typeName;
                // The configuration type is automatically picked as *Configuration (if the type exists)
                vq.ConfigurationType = vq.ConfigurationType.NullIfNoValue();
                if (vq.ConfigurationType == null)
                {
                    var autoConfigTypeName = $"{typeName}Configuration";
                    var autoConfigType = appState.GetContentType(autoConfigTypeName);
                    vq.ConfigurationType = autoConfigType?.NameId;
                    l2.A($"Type name '{typeName}' had no config in definition, checked '{autoConfigTypeName}', found: {autoConfigType != null}");
                }

                // Force the type of all local DataSources to be App
                vq.Type = DataSourceType.App;
                // Optionally set the star-icon if none is set
                vq.Icon = vq.Icon.NullIfNoValue() ?? "star";
                // If In has not been set, make sure we show the Default In as an option
                vq.In ??= [DataSourceConstants.StreamDefaultName];
                // Only set dynamic in if it was never set
                if (!vq._DynamicInWasSet) vq.DynamicIn = true;

                // 4. Build DataSourceInfo with the manually built Visual Query Attribute
                return l2.ReturnAsOk(new(pair.Type, false, overrideVisualQuery: vq));
            })
            .Where(dsi => dsi != null)
            .ToList();

        return l.Return(data, $"{data.Count}");
    }
}