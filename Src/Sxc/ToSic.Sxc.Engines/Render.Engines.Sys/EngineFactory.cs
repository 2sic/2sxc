using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sxc.Render.Engines.Razor;
using ToSic.Sxc.Render.Engines.Token;

namespace ToSic.Sxc.Render.Engines.Sys;

/// <summary>
/// Generate the appropriate engine for a specific view.
/// </summary>
/// <remarks>
/// As of now, only Razor and Token engines are supported, but in the future there may be more engines available.
///
/// The selection system is still trivial, in future, it should be made more generic.
/// </remarks>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class EngineFactory(Generator<IRazorEngine> razorEngineGen, Generator<ITokenEngine> tokenEngineGen)
    : ServiceBase($"{SxcLogName}.EngFct", connect: [razorEngineGen, tokenEngineGen]), IEngineFactory
{
    public IEngine CreateEngine(IView view) => view.IsRazor
        ? razorEngineGen.New()
        : tokenEngineGen.New();
}