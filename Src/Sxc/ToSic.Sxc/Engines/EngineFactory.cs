using ToSic.Eav.DI;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Engines
{
    public class EngineFactory
    {

        public EngineFactory(Generator<IRazorEngine> razorEngineGen, Generator<TokenEngine> tokenEngineGen)
        {
            _razorEngineGen = razorEngineGen;
            _tokenEngineGen = tokenEngineGen;
        }
        private readonly Generator<IRazorEngine> _razorEngineGen;
        private readonly Generator<TokenEngine> _tokenEngineGen;

        public IEngine CreateEngine(IView view) => view.IsRazor
            ? (IEngine)_razorEngineGen.New
            : _tokenEngineGen.New;
    }
}