using System;
using DotNetNuke.Common;
using ToSic.Eav.Context;
using ToSic.Sxc.Context;
using Assembly = System.Reflection.Assembly;

namespace ToSic.Sxc.Dnn.Run
{
    public class DnnPlatformContext: Platform
    {
        public override PlatformType Type => PlatformType.Dnn;

        public override Version Version => Assembly.GetAssembly(typeof(Globals)).GetName().Version;
    }
}
