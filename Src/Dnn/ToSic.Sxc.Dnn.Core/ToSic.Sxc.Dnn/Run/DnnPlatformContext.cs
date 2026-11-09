using DotNetNuke.Application;
using ToSic.Sxc.Context;
using DotNetNuke.Abstractions.Application;
using ToSic.Sxc.Context.Sys.Platform;
using ToSic.Sys.Capabilities.Platform;

namespace ToSic.Sxc.Dnn.Run;

internal class DnnPlatformContext(IHostSettings hostSettings): Platform, IPlatformInfo
{
    public override PlatformType Type => PlatformType.Dnn;

    public override Version Version => DotNetNukeContext.Current.Application.Version;

    string IPlatformInfo.Identity => hostSettings.Guid;
}
