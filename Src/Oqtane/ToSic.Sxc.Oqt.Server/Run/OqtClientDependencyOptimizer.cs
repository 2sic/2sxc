using System;
using Microsoft.AspNetCore.Http;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtClientDependencyOptimizer: ClientDependencyOptimizer
    {
        //private readonly IHttpContextAccessor _httpContext;

        //public OqtClientDependencyOptimizer(IHttpContextAccessor httpContext)
        //{
        //    _httpContext = httpContext;
        //}

        public override Tuple<string, bool> Process(string renderedTemplate)
        {
            //if (_httpContext.HttpContext == null)
            //    return new Tuple<string, bool>(renderedTemplate, false);

            var include2SxcJs = false;

            ExtractOnlyEnableOptimization = false;

            // Handle Client Dependency injection
            renderedTemplate = ExtractExternalScripts(renderedTemplate, ref include2SxcJs);

            // Handle inline JS
            renderedTemplate = ExtractInlineScripts(renderedTemplate);

            // Handle Styles
            renderedTemplate = ExtractStyles(renderedTemplate);

            Assets.ForEach(a => a.PosInPage = OqtPositionName(a.PosInPage));

            return new Tuple<string, bool>(renderedTemplate, include2SxcJs);
        }



        private string OqtPositionName(string position)
        {
            position = position.ToLowerInvariant();

            return position switch
            {
                "body" => position,
                "head" => position,
                _ => "body"
            };
        }

    }

}
