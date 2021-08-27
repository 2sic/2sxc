using System;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Engines
{
    internal class EngineFactory
    {

        public static IEngine CreateEngine(IServiceProvider serviceProvider, IView view)
        {
            return view.IsRazor
                ? (IEngine)serviceProvider.Build<IRazorEngine>()
                : serviceProvider.Build<TokenEngine>();
        }
    }
}