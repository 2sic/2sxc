using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks.Internal;

namespace ToSic.Sxc.Engines;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class EngineFactory(Generator<IRazorEngine> razorEngineGen, Generator<TokenEngine> tokenEngineGen)
    : ServiceBase($"{SxcLogName}.EngFct", connect: [razorEngineGen, tokenEngineGen]), IEngineFactory
{
    public IEngine CreateEngine(IView view) => view.IsRazor
        ? razorEngineGen.New()
        : tokenEngineGen.New();
}