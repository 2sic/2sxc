using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Routing;

namespace ToSic.Sxc.Oqt.Server.Controllers.AppApi
{
    public static class AppApiMiddlewareExtension
    {


        public static IApplicationBuilder UseAppApi(this IApplicationBuilder app)
        {
            // Branch execution
            app.MapWhen(context => MatchAppApi(context), appBuilder =>
            {
                appBuilder.UseMiddleware<AppApiMiddleware>();
            });
            return app;
        }

        /// <summary>
        /// Match 2sxc app api route path without edition: "{alias}/api/sxc/app/{appFolder}/api/{controller}/{action}",
        /// or with edition: "{alias}/api/sxc/app/{appFolder}/{edition}/api/{controller}/{action}".
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public static bool MatchAppApi(HttpContext context)
        {
            return MatchAppApi(context, out _);
        }

        private static bool MatchAppApi(HttpContext context, out RouteValueDictionary values)
        {
            values = new RouteValueDictionary();
            var routeValues = SplitPath(context.Request.Path.Value);

            // Check for number of route segments.
            if (routeValues.Count < 7) return false;

            values.Add("alias", routeValues[0]);

            // Check for 'api' in second segment.
            if (routeValues[1] != "api") return false;

            // Check for 'sxc' in third segment.
            if (routeValues[2] != "sxc") return false;

            // Check for 'app' in 4th segment.
            if (routeValues[3] != "app") return false;

            values.Add("appFolder", routeValues[4]);

            // Check for 'api' in 6th segment.
            if (routeValues[5] == "api")
            {
                // Match route path without edition.
                // "api/{controller}/{action}"
                values.Add("controller", routeValues[6]);
                values.Add("action", routeValues[7]);
                return true;
            }

            if (routeValues.Count < 8 || routeValues[6] != "api") return false;

            // Match route path with edition.
            // "{edition}/api/{controller}/{action}"
            values.Add("edition", routeValues[5]);
            values.Add("controller", routeValues[7]);
            values.Add("action", routeValues[8]);
            return true;
        }

        public static RouteValueDictionary GetValues(HttpContext context)
        {
            MatchAppApi(context, out var values);
            return values;
        }

        public static List<string> SplitPath(string path)
        {
            if (string.IsNullOrWhiteSpace(path)) return new List<string>();
            if (path.EndsWith("/")) path = path.Remove(path.Length - 1);
            if (path.StartsWith("/")) path = path.Substring(1);
            return path.Split('/').ToList();
        }
    }
}
