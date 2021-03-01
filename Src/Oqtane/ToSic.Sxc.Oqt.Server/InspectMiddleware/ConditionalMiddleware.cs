using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ToSic.Sxc.Oqt.Server.Controllers.WebApiRouting;

namespace ToSic.Sxc.Oqt.Server.InspectMiddleware
{
    // https://andrewlock.net/inserting-middleware-between-userouting-and-useendpoints-as-a-library-author-part-2/
    internal class ConditionalMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ConditionalMiddleware> _logger;
        private readonly string _runAfterMiddlewareTypeName;

        public ConditionalMiddleware(RequestDelegate next, ILogger<ConditionalMiddleware> logger, string runAfterMiddlewareName)
        {
            _runAfterMiddlewareTypeName = runAfterMiddlewareName;
            _next = next;
            _logger = logger;
        }

        public async Task Invoke(HttpContext httpContext)
        {
            if (IsCorrectMiddleware(httpContext, _runAfterMiddlewareTypeName))
            {
                _logger.LogInformation("Running conditional middleware after {PreviousMiddleware}", _runAfterMiddlewareTypeName);
            }

            await _next(httpContext);
        }

        static bool IsCorrectMiddleware(HttpContext httpContext, string requiredMiddleware)
        {
            return httpContext.Items.TryGetValue(NameCheckerMiddleware.WrappedMiddlewareKey, out var wrappedMiddlewareName)
                   && wrappedMiddlewareName is string name
                   && string.Equals(name, requiredMiddleware, StringComparison.Ordinal);
        }
    }
}
