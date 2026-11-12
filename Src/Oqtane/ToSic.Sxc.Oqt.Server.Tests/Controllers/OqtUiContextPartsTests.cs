using Microsoft.Extensions.DependencyInjection;
using Oqtane.Shared;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Run.Startup;
using ToSic.Eav.WebApi.Sys.Context;
using ToSic.Sxc.Backend.Context;
using ToSic.Sxc.Oqt.Server.Controllers;
using ToSic.Sxc.Run.Startup;
using ToSic.Sys.DI;
using static Xunit.Assert;

namespace ToSic.Sxc.Oqt.Controllers;

public class OqtUiContextPartsTests
{
    [Fact]
    public void Get_OmitsApp_WhenNoAppIsSelected()
    {
        var services = new ServiceCollection();
        services.AddTransient<IUiContextParts, OqtUiContextParts>();
        services.AddSingleton<SiteState>();
        services.AddSxcCoreFallbacks().AddSxcWebApi().AddEavAll().AddEavAllFallbacks();
        using var provider = services.BuildServiceProvider();
        var builder = provider.GetRequiredService<Generator<IUiContextBuilder, IAppReader?>>()
            .New(null);

        var context = builder.Get(Ctx.AppBasic, CtxEnable.None);

        Null(context.App);
    }
}
