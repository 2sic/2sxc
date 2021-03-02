//using Microsoft.AspNetCore.Hosting;
//using Microsoft.Extensions.DependencyInjection;

//namespace ToSic.Sxc.Oqt.Server.InspectMiddleware
//{
//    // https://andrewlock.net/inserting-middleware-between-userouting-and-useendpoints-as-a-library-author-part-2/
//    public static class ConditionalMiddlewareExtensions
//    {
//        public static IServiceCollection AddConditionalMiddleware(this IServiceCollection services, string beforeMiddleware)
//        {
//            return services.AddTransient<IStartupFilter>(_ => new ConditionalMiddlewareStartupFilter(beforeMiddleware));
//        }

//        public static IServiceCollection AddConditionalMiddlewareBeforeEndpoints(this IServiceCollection services)
//        {
//            return services.AddConditionalMiddleware("Microsoft.AspNetCore.Routing.EndpointMiddleware");
//        }
//    }
//}
