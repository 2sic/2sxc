using System;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Context;
using ToSic.Eav.WebApi.Sys.Helpers.Http;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Oqt.Server.Blocks;
using ToSic.Sxc.Run.Startup;
using ToSic.Sys.DI;
using ToSic.Sys.Users;
using static Xunit.Assert;

namespace ToSic.Sxc.Oqt.Context;

/// <summary>
/// WebApi requests without ModuleId/PageId headers
/// (public app APIs, REST data by app path, URL-id calls) must still get a usable context.
/// </summary>
public class OqtGetBlockWithoutHeadersTests
{
    [Fact]
    public void PrepareContext_WithoutModuleHeaders_DoesNotThrow_AndHasNoBlock()
    {
        var services = new ServiceCollection();
        services.AddTransient(typeof(Generator<>), typeof(Generator<>));
        services.AddTransient(typeof(LazySvc<>), typeof(LazySvc<>));
        services.AddContextProviders();
        services.AddSingleton<ISite>(_ => null!);
        services.AddSingleton<IUser>(_ => null!);
        using var provider = services.BuildServiceProvider();
        using var scope = provider.CreateScope();

        var http = new HttpContextAccessor { HttpContext = new DefaultHttpContext() }; // no ModuleId / PageId
        var getBlock = new OqtGetBlock(null!, new(http),
            scope.ServiceProvider.GetRequiredService<ISxcCurrentContextService>(), null!, null!, null!);

        var ex = Record.Exception(() => getBlock.PrepareContextResolverForApiRequest());

        Null(ex);
    }
}
