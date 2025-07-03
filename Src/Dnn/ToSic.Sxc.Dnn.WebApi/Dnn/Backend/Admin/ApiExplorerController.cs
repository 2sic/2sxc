using System.Reflection;
using System.Web.Compilation;
using System.Web.Hosting;
using ToSic.Eav.Apps.Sys.AppJson;
using ToSic.Eav.Context;
using ToSic.Eav.WebApi.Sys.ApiExplorer;
using ToSic.Sxc.Code.Sys.HotBuild;
using ToSic.Sxc.Code.Sys.SourceCode;
using ToSic.Sxc.Dnn.Compile;
using ToSic.Sxc.Dnn.Compile.Sys;
using ToSic.Sxc.Dnn.Integration;
using ToSic.Sxc.Dnn.WebApi.Sys;
using ToSic.Sxc.Polymorphism.Sys;
using RealController = ToSic.Eav.WebApi.Sys.ApiExplorer.ApiExplorerControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Admin;

[ValidateAntiForgeryToken]
[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class ApiExplorerController() : DnnSxcControllerRoot(RealController.LogSuffix), IApiExplorerController
{
    private RealController Real => SysHlp.GetService<RealController>();

    [HttpGet]
    public HttpResponseMessage Inspect(string path)
    {
        // Make sure the Scoped ResponseMaker has this controller context
        SysHlp.SetupResponseMaker(this);
        return Real.Inspect(path, GetCompiledAssembly);
    }

    private Assembly GetCompiledAssembly(string path)
    {
        var className = Path.GetFileNameWithoutExtension(path);
        Log.A($"Class name: {className}");

        var controllerVirtualPath =
            Path.Combine(
                SysHlp.GetService<DnnAppFolderUtilities>().Setup(Request).GetAppFolderVirtualPath(SysHlp.GetService<ISite>()), 
                path);

        Log.A($"Controller Virtual Path: {controllerVirtualPath}");

        if (!File.Exists(HostingEnvironment.MapPath(controllerVirtualPath)))
            throw new($"Error: can't find controller file: {controllerVirtualPath}");

        Assembly assembly;
        var appJson = SysHlp.GetService<IAppJsonConfigurationService>();
        var block = SysHlp.GetService<DnnGetBlock>().GetCmsBlock(Request);
        var codeFileInfo = SysHlp.GetService<SourceAnalyzer>().TypeOfVirtualPath(controllerVirtualPath);
        if ((block != null && appJson.DnnCompilerAlwaysUseRoslyn(block.AppId)) || codeFileInfo.AppCode || FileInAppCode(path))
        {
            Log.A("has AppCode");
            // Figure edition
            HotBuildSpec spec = null;
            
            if (block != null)
            {
                var edition = SysHlp.GetService<PolymorphConfigReader>().UseViewEditionOrGet(block);
                spec = new(block.AppId, edition: edition, appName: block.App.Name);
            }
            assembly = SysHlp.GetService<IRoslynBuildManager>().GetCompiledAssembly(codeFileInfo, className, spec)?.Assembly;
        }
        else
        {
            assembly = BuildManager.GetCompiledAssembly(controllerVirtualPath);
        }

        if (assembly == null)
            throw new("Assembly not found or compiled to null (error).");

        return assembly;
    }

    [HttpGet]
    [JsonFormatter(Casing = Casing.Camel)]
    public AllApiFilesDto AppApiFiles(int appId) => Real.AppApiFiles(appId);

    private bool FileInAppCode(string path) => path.StartsWith("AppCode\\api\\", StringComparison.InvariantCultureIgnoreCase) || path.ContainsInsensitive("\\AppCode\\api\\");

}