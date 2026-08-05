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

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class DnnAppWebApiControllerAssemblyLoader(
    AppFolderLookupForWebApi appFolderLookup,
    ISite site,
    ISxcCurrentContextService ctxService,
    IAppJsonConfigurationService appJson,
    SourceAnalyzer sourceAnalyzer,
    IRoslynBuildManager roslynBuildManager,
    PolymorphConfigReader polymorphism)
    : ServiceBase($"{DnnConstants.LogName}.ApiCtlAsm", connect: [appFolderLookup, site, ctxService, appJson, sourceAnalyzer, roslynBuildManager, polymorphism]),
        IAppWebApiControllerAssemblyLoader
{
    public Assembly GetAssembly(string path)
    {
        var className = Path.GetFileNameWithoutExtension(path);
        Log.A($"Class name: {className}");

        var controllerVirtualPath = Path.Combine(site.AppsRootPhysical, appFolderLookup.GetAppFolder(), path).ForwardSlash();
        Log.A($"Controller Virtual Path: {controllerVirtualPath}");

        if (!File.Exists(HostingEnvironment.MapPath(controllerVirtualPath)))
            throw new($"Error: can't find controller file: {controllerVirtualPath}");

        var block = ctxService.BlockOrNull();
        var appSpecs = block?.Context.AppReaderRequired.Specs ?? ctxService.AppOrNull()?.AppReaderRequired.Specs;
        var codeFileInfo = sourceAnalyzer.TypeOfVirtualPath(controllerVirtualPath);
        var useRoslyn = (block != null && appJson.DnnCompilerAlwaysUseRoslyn(block.AppId)) || codeFileInfo.AppCode || FileInAppCode(path);

        Assembly assembly;
        if (useRoslyn)
        {
            Log.A("has AppCode");
            var edition = block != null
                ? polymorphism.UseViewEditionOrGet(block)
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