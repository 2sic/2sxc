using System.IO;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Hosting;
using ToSic.Eav.Context;
using ToSic.Eav.WebApi.ApiExplorer;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code.Internal.HotBuild;
using ToSic.Sxc.Code.Internal.SourceCode;
using ToSic.Sxc.Dnn.Compile;
using ToSic.Sxc.Dnn.Integration;
using ToSic.Sxc.Polymorphism.Internal;
using RealController = ToSic.Eav.WebApi.ApiExplorer.ApiExplorerControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Admin;

[ValidateAntiForgeryToken]
[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
                SysHlp.GetService<DnnAppFolderUtilities>().GetAppFolderVirtualPath(Request, SysHlp.GetService<ISite>()), 
                path);

        Log.A($"Controller Virtual Path: {controllerVirtualPath}");

        if (!File.Exists(HostingEnvironment.MapPath(controllerVirtualPath)))
            throw new($"Error: can't find controller file: {controllerVirtualPath}");

        Assembly assembly;
        var codeFileInfo = SysHlp.GetService<SourceAnalyzer>().TypeOfVirtualPath(controllerVirtualPath);
        if (codeFileInfo.ThisApp)
        {
            Log.A("has ThisApp");
            // Figure edition
            HotBuildSpec spec = null;
            var block = SysHlp.GetService<DnnGetBlock>().GetCmsBlock(Request).LoadBlock();
            if (block != null)
                spec = new HotBuildSpec(block.AppId,
                    edition: PolymorphConfigReader.UseViewEditionLazyGetEdition(block.View, () => SysHlp.GetService<PolymorphConfigReader>().Init(block.Context.AppState.List)));
            assembly = SysHlp.GetService<IRoslynBuildManager>().GetCompiledAssembly(codeFileInfo, className, spec)?.Assembly;
        }
        else
        {
            assembly = BuildManager.GetCompiledAssembly(controllerVirtualPath);
        }

        if (assembly == null) throw new("Assembly not found or compiled to null (error).");

        return assembly;
    }

}