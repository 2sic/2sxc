using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace ToSic.Sxc.Oqt.Server.InspectMiddleware
{
    // https://andrewlock.net/inserting-middleware-between-userouting-and-useendpoints-as-a-library-author-part-2/
    public static class ConditionalMiddlewareExtensions
    {
        public static IServiceCollection AddConditionalMiddleware(this IServiceCollection services, string afterMiddleware)
        {
            return services.AddTransient<IStartupFilter>(_ => new ConditionalMiddlewareStartupFilter(afterMiddleware));
        }

        public static IServiceCollection AddConditionalMiddlewareAfterRouting(this IServiceCollection services)
        {
            return services.AddConditionalMiddleware("Microsoft.AspNetCore.Routing.EndpointMiddleware");
        }
    }
}
