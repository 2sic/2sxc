using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;

namespace ToSic.Sxc.Oqt.Server.InspectMiddleware
{
    // https://andrewlock.net/inserting-middleware-between-userouting-and-useendpoints-as-a-library-author-part-2/
    public class ConditionalMiddlewareStartupFilter : IStartupFilter
    {
        private readonly string _runAfterMiddlewareName;

        public ConditionalMiddlewareStartupFilter(string runAfterMiddlewareName)
        {
            _runAfterMiddlewareName = runAfterMiddlewareName;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                var wrappedBuilder = new ConditionalMiddlewareBuilder(builder, _runAfterMiddlewareName);
                next(wrappedBuilder);
            };
        }
    }
}
