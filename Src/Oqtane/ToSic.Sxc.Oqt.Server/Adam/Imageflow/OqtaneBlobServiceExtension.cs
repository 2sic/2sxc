using Imazen.Common.Storage;
using Microsoft.Extensions.DependencyInjection;

namespace ToSic.Sxc.Oqt.Server.Adam.Imageflow
{
    public static class OqtaneBlobServiceExtensions
    {
        public static IServiceCollection AddImageflowOqtaneBlobService(this IServiceCollection services)
        {
            services.AddSingleton<IBlobProvider>((container) => new OqtaneBlobService(container));

            return services;
        }
    }
}
