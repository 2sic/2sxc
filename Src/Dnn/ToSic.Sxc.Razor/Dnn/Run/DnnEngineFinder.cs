using System;
using ToSic.Sxc.Engines;

namespace ToSic.Sxc.Dnn.Run
{
    public class DnnEngineFinder: IEngineFinder
    {
        public Type RazorEngineType() => typeof(RazorEngine);
    }
}
