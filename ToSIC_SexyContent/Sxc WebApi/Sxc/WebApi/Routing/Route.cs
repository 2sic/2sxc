using System.Web.Http.Routing;

namespace ToSic.SexyContent.WebApi.AutoDetectContext
{
    internal class Route
    {
        public const string AppPathKey = "apppath";
        public const string AppIdKey = "appId";
        public const string ZoneIdKey = "zoneId";

        public static string AppPathOrNull(IHttpRouteData route) => route.Values[AppPathKey]?.ToString();
    }
}
