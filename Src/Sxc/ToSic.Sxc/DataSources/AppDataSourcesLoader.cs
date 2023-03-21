using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Caching;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Paths;
using ToSic.Eav.Caching.CachingMonitors;
using ToSic.Eav.Context;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Catalog;
using ToSic.Eav.DataSources.Queries;
using ToSic.Eav.Plumbing;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.DataSources
{
    public class AppDataSourcesLoader : ServiceBase, IAppDataSourcesLoader
    {
        private const string DataSourcesFolder = "DataSources";

        public AppDataSourcesLoader(ILogStore logStore, ISite site, IAppStates appStates, LazySvc<AppPaths> appPathsLazy, LazySvc<CodeCompiler> codeCompilerLazy) : base("Eav.AppDtaSrcLoad")
        {
            ConnectServices(
                _logStore = logStore,
                _site = site,
                _appStates = appStates,
                _appPathsLazy = appPathsLazy,
                _codeCompilerLazy = codeCompilerLazy
            );
        }
        private readonly ILogStore _logStore;
        private readonly ISite _site;
        private readonly IAppStates _appStates;
        private readonly LazySvc<AppPaths> _appPathsLazy;
        private readonly LazySvc<CodeCompiler> _codeCompilerLazy;

        public (List<DataSourceInfo> data, CacheItemPolicy policy) CreateAndReturnAppCache(int appId)
        {
            _logStore.Add(EavLogs.LogStoreAppDataSourcesLoader, Log);
            var l = Log.Fn<(List<DataSourceInfo> data, CacheItemPolicy policy)>();
            var expiration = new TimeSpan(1, 0, 0);
            var policy = new CacheItemPolicy { SlidingExpiration = expiration };
            try
            {
                // Get paths
                var (physicalPath, relativePath) = GetAppDataSourceFolderPaths(appId);

                // If the directory doesn't exist, return an empty list with a 3 minute policy
                // just so we don't keep trying to do this on every query
                if (!Directory.Exists(physicalPath))
                    return l.Return((new List<DataSourceInfo>(),
                        new CacheItemPolicy { SlidingExpiration = new TimeSpan(0, 5, 0) }), "error");

                policy.ChangeMonitors.Add(new FolderChangeMonitor(new List<string> { physicalPath }));

                var data = CreateDataSourceInfos(appId, physicalPath, relativePath);

                return l.ReturnAsOk((data, policy));
            }
            catch
            {
                return l.Return((new List<DataSourceInfo>(), policy), "error");
            }
        }

        private List<DataSourceInfo> CreateDataSourceInfos(int appId, string physicalPath, string relativePath)
        {
            // App state for automatic lookup of configuration content-types
            var appState = _appStates.Get(appId);

            var types = LoadAppDataSources(appId, physicalPath, relativePath);
            var data = types
                .Select(pair =>
                {

                    // 0. If error then type is null, in this case, return a specially crafted DSI
                    if (pair.Type == null)
                        return DataSourceInfo.CreateError(pair.ClassName, false, DataSourceType.App, pair.Error);

                    // 1. Make sure we only keep DataSources and not other classes in the same folder
                    // but assume all null-types are errors, which we should preserve
                    if (!typeof(IDataSource).IsAssignableFrom(pair.Type)) return null;

                    // 2. Get VisualQuery Attribute if available, or create new, since it's optional in DynamicCode
                    var vq = pair.Type.GetDirectlyAttachedAttribute<VisualQueryAttribute>()
                             ?? new VisualQueryAttribute();

                    var typeName = pair.Type.Name;
                  

                    // 3. Update various properties which are needed for further functionality
                    // The global name is always necessary
                    vq.NameId = vq.NameId.NullIfNoValue() ?? typeName;
                    // The configuration type is automatically picked as *Configuration (if the type exists)
                    vq.ConfigurationType = vq.ConfigurationType.NullIfNoValue() ?? appState.GetContentType($"{typeName}Configuration")?.NameId;
                    // Force the type of all local DataSources to be App
                    vq.Type = DataSourceType.App;
                    // Optionally set the star-icon if none is set
                    vq.Icon = vq.Icon.NullIfNoValue() ?? "star";
                    // If In has not been set, make sure we show the Default In as an option
                    vq.In = vq.In ?? new[] { DataSourceConstants.StreamDefaultName };
                    // Only set dynamic in if it was never set
                    if (!vq._DynamicInWasSet) vq.DynamicIn = true;

                    // 4. Build DataSourceInfo with the manually built Visual Query Attribute
                    return new DataSourceInfo(pair.Type, false, overrideVisualQuery: vq);
                })
                .Where(dsi => dsi != null)
                .ToList();

            return data;
        }

        private (string physicalPath, string relativePath) GetAppDataSourceFolderPaths(int appId)
        {
            var appState = _appStates.Get(appId);
            var appPaths = _appPathsLazy.Value.Init(_site, appState);
            var physicalPath = Path.Combine(appPaths.PhysicalPath, DataSourcesFolder);
            var relativePath = Path.Combine(appPaths.RelativePath, DataSourcesFolder);
            return (physicalPath, relativePath);
        }

        private IEnumerable<(string ClassName, Type Type, DataSourceInfoError Error)> LoadAppDataSources(
            int appId,
            string physicalPath, 
            string relativePath
        ) => Log.Func($"a:{appId}", l =>
        {
            if (!Directory.Exists(physicalPath))
                return (null, $"no folder {physicalPath}");

            var compiler = _codeCompilerLazy.Value;

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
                            throwOnError: false);

                        if (!errorMessages.HasValue()) 
                            return (className, type, null);
                        l.E(errorMessages);
                        return(className, type, new DataSourceInfoError("Error Compiling", errorMessages));
                    }
                    catch (Exception ex)
                    {
                        l.Ex(ex);
                        return (className, null, new DataSourceInfoError("Unknown Exception", ex.Message));
                    }
                })
                .ToList();

            return types.Any() 
                ? (types2: types, $"OK, DataSources:{types.Count} ({string.Join(";", types.Select(t => t.className))}), path:{relativePath}")
                : (types2: types, $"OK, no working DataSources found, path:{relativePath}") ;
        });
    }
}
