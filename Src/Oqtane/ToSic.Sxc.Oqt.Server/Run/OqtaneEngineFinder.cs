using System;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Oqt.Server.Engines;

namespace ToSic.Sxc.Oqt.Server.Run
{

    public class OqtaneEngineFinder : IEngineFinder
    {
        public Type RazorEngineType() => typeof(OqtaneRazorEngine);
    }
}
