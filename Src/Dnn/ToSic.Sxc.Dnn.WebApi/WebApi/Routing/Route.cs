using System.Web.Http.Routing;

namespace ToSic.Sxc.WebApi
{
    internal class Route
    {
        public const string AppPathKey = "apppath";

        public static string AppPathOrNull(IHttpRouteData route) => route.Values[AppPathKey]?.ToString();
    }
}
