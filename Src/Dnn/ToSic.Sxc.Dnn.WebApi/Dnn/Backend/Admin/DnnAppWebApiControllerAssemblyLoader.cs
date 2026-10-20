using System.Reflection;
using System.Web.Compilation;
using System.Web.Hosting;
using ToSic.Eav.Apps.Sys.AppJson;
using ToSic.Eav.Context;
using ToSic.Eav.Sys;
using ToSic.Eav.WebApi.Sys.ApiExplorer;
using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sxc.Code.Sys.SourceCode;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Dnn.Compile;
using ToSic.Sxc.Dnn.Compile.Sys;
using ToSic.Sxc.Render.Polymorphism.Sys;
using ToSic.Sxc.WebApi.Sys;

namespace ToSic.Sxc.Dnn.Backend.Admin;

/// <summary>
/// Assembly loader for the data source which lists all the ApiControllers to the admin ui.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class DnnAppWebApiControllerAssemblyLoader(
    ISite site,
    ISxcCurrentContextService ctxService,
    IAppJsonConfigurationService appJson,
    SourceAnalyzer sourceAnalyzer,
    IRoslynBuildManager roslynBuildManager,
    IEditionService editionSvc)
    : ServiceBase($"{DnnConstants.LogName}.ApiCtlAsm", connect: [site, ctxService, appJson, sourceAnalyzer, roslynBuildManager, editionSvc]),
        IAppWebApiControllerAssemblyLoader
{
    public Assembly GetAssembly(string path)
    {
        var className = Path.GetFileNameWithoutExtension(path);
        Log.A($"Class name: {className}");

        // Note 2026-10-18 2dm: #NoLookupSideEffects
        // the next line used to have a side effect of sharing the found app-context
        // This has been removed; it may break something, which would then need to be corrected "upstream", but not here
        var controllerVirtualPath = Path.Combine(site.AppsRootPhysical, ctxService.FindAppFolderFromScopeOrThrow(), path).ForwardSlash();
        Log.A($"Controller Virtual Path: {controllerVirtualPath}");

        if (!File.Exists(HostingEnvironment.MapPath(controllerVirtualPath)))
            throw new($"Error: can't find controller file: {controllerVirtualPath}");

        var block = ctxService.BlockOrNull();
        var appSpecs = block?.Context.AppReaderRequired.Specs ?? ctxService.AppContextOrNull()?.AppReaderRequired.Specs;
        var codeFileInfo = sourceAnalyzer.TypeOfVirtualPath(controllerVirtualPath);
        var useRoslyn = (block != null && appJson.DnnCompilerAlwaysUseRoslyn(block.AppId)) || codeFileInfo.AppCode || FileInAppCode(path);

        Assembly assembly;
        if (useRoslyn)
        {
            Log.A("has AppCode");
            var edition = block != null
                ? editionSvc.Edition(block)
                : GetEdition(path);
            var spec = appSpecs == null
                ? null
                : new HotBuildSpec(appSpecs.AppId, edition: edition, appName: appSpecs.Name);

            assembly = roslynBuildManager.GetCompiledAssembly(codeFileInfo, className, spec)?.Assembly;
        }
        else
        {
            assembly = BuildManager.GetCompiledAssembly(controllerVirtualPath);
        }

        if (assembly == null)
            throw new("Assembly not found or compiled to null (error).");

        return assembly;
    }

    private static bool FileInAppCode(string path) => path.StartsWith("AppCode\\api\\", StringComparison.InvariantCultureIgnoreCase) || path.ContainsInsensitive("\\AppCode\\api\\");

    private static string GetEdition(string path)
    {
        var edition = path.Split(['/'], StringSplitOptions.RemoveEmptyEntries)[0];
        return IsRootEdition(path, edition) ? string.Empty : edition;
    }

    private static bool IsRootEdition(string path, string edition)
        => edition.Equals(EavConstants.Api, StringComparison.OrdinalIgnoreCase)
           || edition.Equals(FolderConstants.AppCodeFolder, StringComparison.OrdinalIgnoreCase)
           || edition.Equals(FolderConstants.DataFolderProtected, StringComparison.OrdinalIgnoreCase)
           || edition.Equals(path, StringComparison.OrdinalIgnoreCase);
}