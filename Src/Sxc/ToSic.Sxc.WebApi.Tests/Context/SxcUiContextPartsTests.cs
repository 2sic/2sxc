using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav;
using ToSic.Eav.Run.Startup;
using ToSic.Eav.WebApi.Sys.Context;
using ToSic.Sxc.Run.Startup;
using static Xunit.Assert;

namespace ToSic.Sxc.WebApi.Tests.Context;

public class SxcUiContextPartsTests
{
    [Fact]
    public void Get_OmitsApp_WhenNoAppIsSelected()
    {
        var services = new ServiceCollection();
        services.AddSxcWebApi().AddEavAll().AddEavAllFallbacks();
        using var provider = services.BuildServiceProvider();
        var builder = provider.GetRequiredService<Generator<IUiContextBuilder, IAppReader?>>()
            .New(null);

        var context = builder.Get(Ctx.AppBasic, CtxEnable.None);

        Null(context.App);
    }
}
