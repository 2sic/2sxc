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
            try
            {
                var expiration = new TimeSpan(1, 0, 0);
                var policy = new CacheItemPolicy { SlidingExpiration = expiration };

                var (physicalPath, virtualPath) = GetAppDataSourceFolderPaths(appId);
                if (Directory.Exists(physicalPath))
                    policy.ChangeMonitors.Add(new FolderChangeMonitor(new List<string> { physicalPath }));

                var data = CreateDataSourceInfos(appId);

                return (data, policy);
            }
            catch
            {
                /* ignore for now */
            }
            return (null, null);
        }

        private List<DataSourceInfo> CreateDataSourceInfos(int appId)
        {
            // App state for automatic lookup of configuration content-types
            var appState = _appStates.Get(appId);

            var data = LoadAppDataSources(appId)
                .Select(pair =>
                {
                    var t = pair.Type;
                    // 1. Make sure we only keep DataSources and not other classes in the same folder
                    if (!typeof(IDataSource).IsAssignableFrom(t)) return null;

                    // 2. Get VisualQuery Attribute if available, or create new, since it's optional in DynamicCode
                    var vq = t?.GetDirectlyAttachedAttribute<VisualQueryAttribute>()
                             ?? new VisualQueryAttribute();

                    var typeName = t?.Name ?? pair.className;
                    

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
                    return new DataSourceInfo(t ?? typeof(Error), false, vq, pair.Error);
                })
                .Where(dsi => dsi != null)
                .ToList();

            return data;
        }

        private (string physicalPath, string virtualPath) GetAppDataSourceFolderPaths(int appId)
        {
            var appState = _appStates.Get(appId);
            var appPaths = _appPathsLazy.Value.Init(_site, appState);
            var physicalPath = Path.Combine(appPaths.PhysicalPath, DataSourcesFolder);
            var virtualPath = Path.Combine(appPaths.Path, DataSourcesFolder);
            return (physicalPath, virtualPath);
        }

        private IEnumerable<(Type Type, DataSourceInfoError Error, string className)> LoadAppDataSources(int appId) => Log.Func($"a:{appId}", l =>
        {
            _logStore.Add(EavLogs.LogStoreAppDataSourcesLoader, Log);

            var (physicalPath, virtualPath) = GetAppDataSourceFolderPaths(appId);

            if (!Directory.Exists(physicalPath))
                return (null, $"no folder {physicalPath}");

            var compiler = _codeCompilerLazy.Value;
            var errors = new List<string>();

            var types2 = Directory
                .GetFiles(physicalPath, "*.cs", SearchOption.TopDirectoryOnly)
                .Select(
                    dataSourceFile =>
                    {
                        var className = Path.GetFileNameWithoutExtension(dataSourceFile);
                        try
                        {
                            var (type, errorMessages) = compiler.GetTypeOrErrorMessages(
                                virtualPath: Path.Combine(virtualPath, Path.GetFileName(dataSourceFile)),
                                className: className,
                                throwOnError: false);

                            DataSourceInfoError err = null;
                            if (!string.IsNullOrEmpty(errorMessages))
                            {
                                errors.Add(errorMessages);
                                err = new DataSourceInfoError { Title = "Error Compiling", Message = errorMessages };
                            }

                            return (type, err, className);
                        }
                        catch (Exception ex)
                        {
                            errors.Add(ex.Message);
                            l.Ex(ex);
                            return (null,
                                new DataSourceInfoError { Title = "Unknown Exception", Message = ex.Message }, 
                                className);
                        }
                    })
                .ToList();

            if (errors.Any()) l.A($"Errors: {string.Join(",", errors)}");

            return types2.Any() 
                ? (types2, $"OK, DataSources:{types2.Count} ({string.Join(";", types2.Select(t => t.Item3))}), path:{virtualPath}")
                : (Enumerable.Empty<(Type, DataSourceInfoError, string)>(), $"OK, no working DataSources found, path:{virtualPath}") ;
        });
    }
}
