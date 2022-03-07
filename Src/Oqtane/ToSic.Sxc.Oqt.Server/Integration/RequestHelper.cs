using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Context;
using ToSic.Sxc.Oqt.Server.WebApi;

namespace ToSic.Sxc.Oqt.Server.Integration
{
    public class RequestHelper
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public RequestHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        
        public T GetTypedHeader<T>(string headerName, T fallback)
        {
            var valueString = _httpContextAccessor.HttpContext?.Request.Headers[headerName] ?? StringValues.Empty;
            return valueString.ConvertOrFallback(fallback, numeric: true);
        }

        public T GetQueryString<T>(string key, T fallback)
        {
            var valueString = _httpContextAccessor.HttpContext?.Request.Query[key] ?? StringValues.Empty;
            return valueString.ConvertOrFallback(fallback, numeric: true);
        }

        public T GetRouteValuesString<T>(string key, T fallback)
        {
            // TODO: stv - this looks wrong, don't think valueString is of this type
            var valueString = $"{_httpContextAccessor.HttpContext?.Request.RouteValues[key]}";
            return valueString.ConvertOrFallback(fallback, numeric: true);
        }

        public int TryGetPageId() =>
            GetTypedHeader(ContextConstants.PageIdKey,
                GetQueryString(WebApiConstants.PageId,
                    GetRouteValuesString(WebApiConstants.PageId, Eav.Constants.NullId)));

        public int TryGetModuleId() =>
            GetTypedHeader(Sxc.WebApi.WebApiConstants.HeaderInstanceId,
                GetQueryString(WebApiConstants.ModuleId,
                    GetRouteValuesString(WebApiConstants.ModuleId, Eav.Constants.NullId)));
    }
}
