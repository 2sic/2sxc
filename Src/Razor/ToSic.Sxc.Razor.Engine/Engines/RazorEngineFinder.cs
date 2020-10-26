using System;
using ToSic.Sxc.Engines;

namespace ToSic.Sxc.Razor.Engines
{

    public class RazorEngineFinder : IEngineFinder
    {
        public Type RazorEngineType() => typeof(RazorEngine3);
    }
}
