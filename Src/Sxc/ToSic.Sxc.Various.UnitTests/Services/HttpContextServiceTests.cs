#if NETCOREAPP
using Microsoft.AspNetCore.Http;
using ToSic.Sxc.Services.HttpCtx;

namespace ToSic.Sxc.Tests.Services;

public class HttpContextServiceTests
{
    [Fact]
    public void Redirect301_SetsStatusCodeAndLocationHeader()
    {
        var httpContext = new DefaultHttpContext();
        var sut = new HttpContextService(new HttpContextAccessor { HttpContext = httpContext });

        sut.Redirect301("/target-page");

        Equal(301, httpContext.Response.StatusCode);
        Equal("/target-page", httpContext.Response.Headers.Location.ToString());
    }

    [Fact]
    public void Redirect_WithCustomRedirectCode_SetsStatusCodeAndLocationHeader()
    {
        var httpContext = new DefaultHttpContext();
        var sut = new HttpContextService(new HttpContextAccessor { HttpContext = httpContext });

        sut.Redirect("/other-target", 308);

        Equal(308, httpContext.Response.StatusCode);
        Equal("/other-target", httpContext.Response.Headers.Location.ToString());
    }
}
#endif
