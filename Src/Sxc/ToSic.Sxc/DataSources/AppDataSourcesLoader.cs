using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.DataSources;
using ToSic.Eav.Apps.Paths;
using ToSic.Eav.Context;
using ToSic.Eav.DataSources.Catalog;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.DataSources
{
    public class AppDataSourcesLoader : ServiceBase, IAppDataSourcesLoader
    {
        private readonly ISite _site;
        private readonly LazySvc<AppPaths> _appPathsLazy;
        private readonly LazySvc<CodeCompiler> _codeCompilerLazy;
        private readonly LazySvc<DataSourceCatalog> _dataSourceCatalogLazy;

        public AppDataSourcesLoader(ISite site, LazySvc<AppPaths> appPathsLazy, LazySvc<CodeCompiler> codeCompilerLazy, LazySvc<DataSourceCatalog> dataSourceCatalogLazy) : base("Eav.AppDtaSrcLoad")
        {
            ConnectServices(
                _site = site,
                _appPathsLazy = appPathsLazy,
                _codeCompilerLazy = codeCompilerLazy,
                _dataSourceCatalogLazy = dataSourceCatalogLazy
            );
        }

        public void Register(AppState appState) => Log.Do($"a:{appState.AppId}", l =>
        {
            var dataSourceCatalog = _dataSourceCatalogLazy.Value;
            var dataSourceInfos = dataSourceCatalog.Get(appState.AppId);
            if (dataSourceInfos != null) return;

            var appPaths = _appPathsLazy.Value.Init(_site, appState);
            var dataSourcesFolder = Path.Combine(appPaths.PhysicalPath, "DataSources");

            if (!Directory.Exists(dataSourcesFolder))
            {
                l.A($"no folder {dataSourcesFolder}");
                return;
            }

            var compiler = _codeCompilerLazy.Value;
            var types = new List<Type>();
            var errors = new List<string>();

            foreach (var dataSourceFile in Directory.GetFiles(dataSourcesFolder, "*.cs",
                         SearchOption.TopDirectoryOnly))
            {
                var virtualPath = Path.Combine(appPaths.Path, "DataSources", Path.GetFileName(dataSourceFile));
                var className = Path.GetFileNameWithoutExtension(virtualPath);
                try
                {
                    var rez = compiler.GetTypeOrErrorMessages(virtualPath, className, false);
                    if (rez.Item1 != null) types.Add(rez.Item1);
                    if (!string.IsNullOrEmpty(rez.Item2)) errors.Add(rez.Item2);
                }
                catch (Exception ex)
                {
                    errors.Add(ex.Message);
                    l.Ex(ex);
                }
            }

            if (errors.Any()) l.A($"errors: {string.Join(",", errors)}");

            if (types.Any())
            {
                l.A($"data sources: {string.Join(",", types.Select(t => t.FullName))}");


                dataSourceCatalog.UpdateAppCache(appState.AppId, types, dataSourcesFolder);
                l.A($"OK, DataSourceCatalog updated appId:{appState.AppId}, types:{types.Count}, path:{dataSourcesFolder}");
            }
            else
                l.A("OK, no types found, so no update of DataSourceCatalog");
        });
    }
}
