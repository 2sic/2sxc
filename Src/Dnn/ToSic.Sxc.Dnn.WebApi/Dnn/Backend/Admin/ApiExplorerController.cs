using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Hosting;
using System.Web.Http;
using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using ToSic.Eav.Context;
using ToSic.Eav.WebApi.ApiExplorer;
using ToSic.Lib.Logging;
using ToSic.Sxc.Dnn.Integration;
using ToSic.Sxc.Dnn.WebApi;
using RealController = ToSic.Eav.WebApi.ApiExplorer.ApiExplorerControllerReal;

namespace ToSic.Sxc.Dnn.Backend.Admin;

[ValidateAntiForgeryToken]
[DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ApiExplorerController() : DnnApiControllerWithFixes(RealController.LogSuffix), IApiExplorerController
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
        var controllerVirtualPath =
            Path.Combine(
                SysHlp.GetService<DnnAppFolderUtilities>().GetAppFolderVirtualPath(Request, SysHlp.GetService<ISite>()), 
                path);

        Log.A($"Controller Virtual Path: {controllerVirtualPath}");

        if (!File.Exists(HostingEnvironment.MapPath(controllerVirtualPath)))
            throw new Exception($"Error: can't find controller file: {controllerVirtualPath}");

        return BuildManager.GetCompiledAssembly(controllerVirtualPath);
    }

}