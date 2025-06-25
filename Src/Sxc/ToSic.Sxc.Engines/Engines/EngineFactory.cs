using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks.Sys.Views;

namespace ToSic.Sxc.Engines;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class EngineFactory(Generator<IRazorEngine> razorEngineGen, Generator<ITokenEngine> tokenEngineGen)
    : ServiceBase($"{SxcLogName}.EngFct", connect: [razorEngineGen, tokenEngineGen]), IEngineFactory
{
    public IEngine CreateEngine(IView view) => view.IsRazor
        ? razorEngineGen.New()
        : tokenEngineGen.New();
}