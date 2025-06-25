using ToSic.Sxc.Blocks.Sys.Views;

namespace ToSic.Sxc.Engines;

public interface IEngineFactory
{
    IEngine CreateEngine(IView view);
}