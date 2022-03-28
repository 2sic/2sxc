using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Engines
{
    internal class EngineFactory
    {
        public static IEngine CreateEngine(IView view, Generator<IRazorEngine> razorEngineGen, Generator<TokenEngine> tokenEngineGen)
        {
            return view.IsRazor
                ? (IEngine)razorEngineGen.New
                : tokenEngineGen.New;
        }
    }
}