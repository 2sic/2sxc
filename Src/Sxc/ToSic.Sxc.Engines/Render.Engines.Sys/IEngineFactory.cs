using ToSic.Sxc.Blocks.Sys.Views;

namespace ToSic.Sxc.Render.Engines.Sys;

[InternalApi_DoNotUse_MayChangeWithoutNotice]
public interface IEngineFactory
{
    IEngine CreateEngine(IView view);
}