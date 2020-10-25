using System;
using ToSic.Sxc.Engines;

namespace ToSic.Sxc.Oqt.Server.Run
{

    public class OqtaneEngineFinder : IEngineFinder
    {
        public Type RazorEngineType() => typeof(RazorEngine);
    }
}
