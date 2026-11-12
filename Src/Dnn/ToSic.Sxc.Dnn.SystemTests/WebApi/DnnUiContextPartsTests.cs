using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Run.Startup;
using ToSic.Eav.WebApi.Sys.Context;
using ToSic.Sxc.Dnn.Integration;
using ToSic.Sxc.Run.Startup;
using ToSic.Sys.DI;

namespace ToSic.Sxc.Dnn.WebApi;

public class DnnUiContextPartsTests
{
    [Fact]
    public void Get_OmitsApp_WhenNoAppIsSelected()
    {
        var services = new ServiceCollection();
        services.AddDnnWebApi().AddSxcWebApi().AddEavAll().AddEavAllFallbacks();
        using var provider = services.BuildServiceProvider();
        var builder = provider.GetRequiredService<Generator<IUiContextBuilder, IAppReader?>>()
            .New(null);

        var context = builder.Get(Ctx.AppBasic, CtxEnable.None);

        Null(context.App);
    }
}
