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
using ToSic.Eav.WebApi.ApiExplorer;
using ToSic.Eav.WebApi.Plumbing;

namespace ToSic.Sxc.Dnn.WebApi.Admin
{
    [ValidateAntiForgeryToken]
    [DnnModuleAuthorize(AccessLevel = SecurityAccessLevel.Admin)]
    public class ApiExplorerController : DnnApiControllerWithFixes<ApiExplorerControllerReal<HttpResponseMessage>>, IApiExplorerController<HttpResponseMessage>
    {
        public ApiExplorerController() : base(ApiExplorerControllerReal<HttpResponseMessage>.LogSuffix) { }

        [HttpGet]
        public HttpResponseMessage Inspect(string path)
        {
            // Make sure the Scoped ResponseMaker has this controller context
            var responseMaker = (ResponseMakerNetFramework)GetService<ResponseMaker<HttpResponseMessage>>();
            responseMaker.Init(this);

            return Real.Inspect(path, GetCompiledAssembly);
        }

        private Assembly GetCompiledAssembly(string path)
        {
            var controllerVirtualPath =
                Path.Combine(
                    AppFolderUtilities.GetAppFolderVirtualPath(GetService<IServiceProvider>(), Request, GetService<ISite>(), Log), 
                    path);

            Log.Add($"Controller Virtual Path: {controllerVirtualPath}");

            if (!File.Exists(HostingEnvironment.MapPath(controllerVirtualPath)))
                throw new Exception($"Error: can't find controller file: {controllerVirtualPath}");

            return BuildManager.GetCompiledAssembly(controllerVirtualPath);
        }

    }


}


