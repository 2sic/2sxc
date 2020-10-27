using System;
using ToSic.Sxc.Engines;

namespace ToSic.Sxc.Razor.Engine
{
    public class RazorEngineFinder : IEngineFinder
    {
        public Type RazorEngineType() => typeof(RazorEngine);
    }
}
