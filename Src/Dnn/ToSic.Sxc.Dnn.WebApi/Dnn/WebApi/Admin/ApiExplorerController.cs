using DotNetNuke.Security;
using DotNetNuke.Web.Api;
using System;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Web.Compilation;
using System.Web.Hosting;
using System.Web.Http;
using ToSic.Eav.Context;
using ToSic.Lib.Logging;
using ToSic.Eav.WebApi.ApiExplorer;
using RealController = ToSic.Eav.WebApi.ApiExplorer.ApiExplorerControllerReal<System.Net.Http.HttpResponseMessage>;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public class ApiExplorerController : DnnApiControllerWithFixes, IApiExplorerController<HttpResponseMessage>
    {
        public ApiExplorerController() : base(RealController.LogSuffix) { }

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


}


