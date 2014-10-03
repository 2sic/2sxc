using DotNetNuke.Web.Api;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Web.Compilation;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Dispatcher;
using System.Web.Http.Routing;

namespace ToSic.SexyContent.WebApiExtensions
{
    public class TemplateControllerSelector : IHttpControllerSelector
    {
        private readonly HttpConfiguration _config;
        public IHttpControllerSelector PreviousSelector { get; set; }

        public TemplateControllerSelector(HttpConfiguration configuration)
        {
            _config = configuration;
        }

        public IDictionary<string, HttpControllerDescriptor> GetControllerMapping()
        {
            return PreviousSelector.GetControllerMapping();
        }

        public HttpControllerDescriptor SelectController(HttpRequestMessage request)
        {
            
            IHttpRouteData routeData = request.GetRouteData();
            if (routeData.Route.RouteTemplate.Contains("/DesktopModules/2sxc/API/App/"))
            {
                var moduleInfo = request.FindModuleInfo();
                var portalSettings = DotNetNuke.Entities.Portals.PortalSettings.Current;
                var sexy = request.FindSexy();

                var controllerPath = Path.Combine(SexyContent.AppBasePath(portalSettings), sexy.App.Folder, "WebApiController.cs");

                if(File.Exists(System.Web.Hosting.HostingEnvironment.MapPath(controllerPath)))
                {
                    var assembly = BuildManager.GetCompiledAssembly(controllerPath);
                    var type = assembly.GetType("WebApiController");
                    return new HttpControllerDescriptor(_config, "WebApiController", type);
                }
            }
            
            return PreviousSelector.SelectController(request);
        }
    }
}