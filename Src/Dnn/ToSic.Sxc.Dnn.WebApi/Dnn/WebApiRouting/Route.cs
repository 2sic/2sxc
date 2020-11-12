using System.Web.Http.Routing;

namespace ToSic.Sxc.Dnn.WebApiRouting
{
    internal class Route
    {
        public static string AppPathOrNull(IHttpRouteData route) => route.Values[Names.AppPath]?.ToString();
    }
}
