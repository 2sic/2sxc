using ToSic.Sxc.Blocks.Internal;

namespace ToSic.Sxc.Engines;

public interface IEngineFactory
{
    IEngine CreateEngine(IView view);
}