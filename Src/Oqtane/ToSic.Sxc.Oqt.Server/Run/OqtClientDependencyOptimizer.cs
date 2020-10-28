using System;
using Microsoft.AspNetCore.Http;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtClientDependencyOptimizer: ClientDependencyOptimizer
    {
        private readonly IHttpContextAccessor _httpContext;

        public OqtClientDependencyOptimizer(IHttpContextAccessor httpContext)
        {
            _httpContext = httpContext;
        }
        public override Tuple<string, bool> Process(string renderedTemplate)
        {
            if (_httpContext.HttpContext == null)
                return new Tuple<string, bool>(renderedTemplate, false);

            //JsDefaultPriority = (int)FileOrder.Js.DefaultPriority;
            //CssDefaultPriority = (int)FileOrder.Css.DefaultPriority;

            var include2SxcJs = false;

            // Handle Client Dependency injection
            renderedTemplate = ExtractScripts(renderedTemplate, ref include2SxcJs);

            // Handle Scripts
            renderedTemplate = ExtractStyles(renderedTemplate);

            // Add to Oqtane -#todo

            return new Tuple<string, bool>(renderedTemplate, include2SxcJs);
        }


    }

}
