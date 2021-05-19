using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;

namespace ToSic.Sxc.Oqt.Server.Run
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
            return ReturnTypedResultOrFallback(valueString, fallback);
        }

        public T GetQueryString<T>(string key, T fallback)
        {
            var valueString = _httpContextAccessor.HttpContext?.Request.Query[key] ?? StringValues.Empty;
            return ReturnTypedResultOrFallback(valueString, fallback);
        }

        public T GetRouteValuesString<T>(string key, T fallback)
        {
            // TODO: stv - this looks wrong, don't think valueString is of this type
            var valueString = $"{_httpContextAccessor.HttpContext?.Request.RouteValues[key]}";
            return ReturnTypedResultOrFallback(valueString, fallback);
        }

        private static T ReturnTypedResultOrFallback<T>(StringValues valueString, T fallback)
        {
            if (valueString == StringValues.Empty) return fallback;
            try
            {
                return (T)Convert.ChangeType(valueString.ToString(), typeof(T));
            }
            catch
            {
                return fallback;
            }
        }
    }
}
