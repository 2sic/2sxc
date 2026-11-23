#if NETCOREAPP

using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace ToSic.Sxc.Web;

public static class HttpContextExtensions
{
    public static ISysServiceProvider GetSysRequestServices(this HttpContext context)
        => context.RequestServices.GetRequiredService<ISysServiceProvider>();
}

#endif