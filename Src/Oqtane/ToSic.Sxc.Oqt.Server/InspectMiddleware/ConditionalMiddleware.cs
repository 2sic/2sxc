//using System;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Routing;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;

//namespace ToSic.Sxc.Oqt.Server.InspectMiddleware
//{
//    // https://andrewlock.net/inserting-middleware-between-userouting-and-useendpoints-as-a-library-author-part-2/
//    internal class ConditionalMiddleware
//    {
//        private readonly RequestDelegate _next;
//        private readonly ILogger<ConditionalMiddleware> _logger;
//        private readonly string _runBefore;
//        private readonly bool _runMiddleware;

//        public ConditionalMiddleware(RequestDelegate next, ILogger<ConditionalMiddleware> logger, string runBefore)
//        {
//            _runMiddleware = next.Target.GetType().FullName == runBefore;

//            _next = next;
//            _logger = logger;
//            _runBefore = runBefore;
//        }

//        public async Task Invoke(HttpContext httpContext)
//        {
//            if (_runMiddleware)
//            {
//                _logger.LogInformation("Running conditional middleware before {NextMiddleware}", _runBefore);
//            }

//            await _next(httpContext);
//        }
//    }
//}
