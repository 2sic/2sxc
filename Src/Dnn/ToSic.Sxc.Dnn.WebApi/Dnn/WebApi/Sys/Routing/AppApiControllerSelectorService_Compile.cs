using System.Reflection;
using System.Web.Compilation;
using System.Web.Hosting;
using System.Web.Http.Controllers;
using ToSic.Eav.Apps.Sys.AppJson;
using ToSic.Eav.Context;
using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sxc.Code.Sys.SourceCode;
using ToSic.Sxc.Dnn.Compile;
using ToSic.Sxc.Dnn.Compile.Sys;
using ToSic.Sxc.Dnn.Context;

namespace ToSic.Sxc.Dnn.WebApi.Sys;

/// <summary>
/// It's in a separate class, so DI etc. works properly
/// </summary>
internal class AppApiControllerCompiler(
    ISite site,
    LazySvc<AppCodeLoader> appCodeLoader,
    LazySvc<SourceAnalyzer> analyzerLazy,
    LazySvc<IRoslynBuildManager> roslynLazy,
    LazySvc<IAppJsonConfigurationService> appJson)
    : ServiceBase("Api.Comp", connect: [site, appCodeLoader, analyzerLazy, roslynLazy, appJson])
{

    internal (HttpControllerDescriptorWithPaths descriptor, IEnumerable<string> cacheKeys, IList<string> filePaths)
        BuildDescriptorIfExists(HttpConfiguration configuration, string appFolder, string edition, string controllerTypeName, bool shared, HotBuildSpec spec)
    {
        var l = Log.Fn<(HttpControllerDescriptorWithPaths descriptor, IEnumerable<string> cacheKeys, IList<string> filePaths)>(
            $"{nameof(appFolder)}:'{appFolder}'; {nameof(edition)}:'{edition}'; {nameof(controllerTypeName)}:'{controllerTypeName}'; {nameof(shared)}:{shared}; {spec}",
            timer: true
        );

        // 0. Prepare folders which will be used
        var controllerFolder = Path.Combine(
            shared ? site.SharedAppsRootRelative() : site.AppsRootPhysical,
            appFolder,
            edition + "api/"
        ).ForwardSlash();

        // 1. Check AppCode Dll
        if (spec != null)
        {
            var appCodeAssemblyResult = appCodeLoader.Value.GetAppCode(spec).AssemblyResult;

            // If assembly found, check if this controller exists in that dll - if yes, return it
            var type = appCodeAssemblyResult?.Assembly?.FindControllerTypeByName(controllerTypeName);
            if (type != null)
            {
                // cache dependency on existing cache item with AppCode assembly 
                var appCodeDescriptor = new HttpControllerDescriptor(configuration, type.Name, type);
                var fakeFolder = controllerFolder.Replace("/api/", "/AppCode/");
                return l.Return((descriptor: new(appCodeDescriptor, fakeFolder, fakeFolder + "AppCode-auto-compiled.dll"),
                                cacheKeys: [appCodeAssemblyResult.CacheDependencyId],
                                filePaths: null), 
                        "Api controller from AppCode");
            }
        }

        // 2. Normal Api, compiled on the fly
        var controllerPath = Path.Combine(controllerFolder, $"{controllerTypeName}.cs");
        l.A($"Controller Folder: '{controllerFolder}' Path: '{controllerPath}'");

        // note: this may look like something you could optimize/cache the result, but that's a bad idea
        // because when the file changes, the type-object will be different, so please don't optimize :)
        var (descriptor, cacheDependencyKeys) = BuildDescriptorOrThrow(configuration, controllerPath, controllerTypeName, spec);
        var hasCacheKeys = cacheDependencyKeys?.Count > 0;
        return l.Return((descriptor: new(descriptor, controllerFolder, controllerPath), 
                        cacheKeys: hasCacheKeys ? cacheDependencyKeys : null, // cache dependency on existing cache item;
                        filePaths: hasCacheKeys ? null: [HostingEnvironment.MapPath(controllerPath)]), // cache dependency on existing api file
            $"normal Api controller from '{controllerPath}'"); 
    }

    private (HttpControllerDescriptor HttpControllerDescriptor, List<string> CacheDependecyKeys)
        BuildDescriptorOrThrow(HttpConfiguration configuration, string fullPath, string typeName, HotBuildSpec spec)
    {
        var l = Log.Fn<(HttpControllerDescriptor, List<string>)>($"{nameof(fullPath)}:'{fullPath}'; {nameof(typeName)}:'{typeName}'; {spec}", timer: true);
        Assembly assembly;
        List<string> cacheDependencyKeys = null;
        var codeFileInfo = analyzerLazy.Value.TypeOfVirtualPath(fullPath);
        var alwaysUseRoslyn = appJson.Value.DnnCompilerAlwaysUseRoslyn(spec.AppId);
        if (alwaysUseRoslyn || codeFileInfo.AppCode)
        {
            l.A("AppCode - use Roslyn");
            var result = roslynLazy.Value.GetCompiledAssembly(codeFileInfo, typeName, spec);
            assembly = result?.Assembly;

            // build list of cache dependencies keys
            if (!string.IsNullOrEmpty(result?.CacheDependencyId))
            {
                cacheDependencyKeys = [result.CacheDependencyId];
                if (alwaysUseRoslyn) 
                    cacheDependencyKeys.Add(appJson.Value.AppJsonCacheKey(spec.AppId));
            }
            l.A($"cache dependency keys: {cacheDependencyKeys?.Count ?? 0}");
        }
        else
        {
            l.A("no AppCode - use BuildManager");
            assembly = BuildManager.GetCompiledAssembly(fullPath);
        }

        if (assembly == null)
            throw l.Ex(new Exception("Assembly not found or compiled to null (error)."));

        l.A($"FindControllerTypeByName: '{typeName}'");
        var type = assembly.FindControllerTypeByName(typeName)
            ?? throw l.Ex(new Exception($"Type '{typeName}' not found in assembly. Could be a compile error or name mismatch."));

        l.A($"Type found: '{type.Name}'");
        return l.Return((new (configuration, type.Name, type), cacheDependencyKeys));
    }

}