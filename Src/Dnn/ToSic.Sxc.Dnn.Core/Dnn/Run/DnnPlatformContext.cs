using System;
using DotNetNuke.Application;
using ToSic.Eav.Run;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Dnn.Run
{
    public class DnnPlatformContext: Platform, IPlatformInfo
    {
        public override PlatformType Type => PlatformType.Dnn;

        public override Version Version => DotNetNukeContext.Current.Application.Version; // Assembly.GetAssembly(typeof(Globals)).GetName().Version;

        string IPlatformInfo.Identity => DotNetNuke.Entities.Host.Host.GUID;
    }
}
