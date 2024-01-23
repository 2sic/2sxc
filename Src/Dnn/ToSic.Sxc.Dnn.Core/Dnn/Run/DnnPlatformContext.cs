using DotNetNuke.Application;
using ToSic.Eav.Context;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Internal;

namespace ToSic.Sxc.Dnn.Run;

internal class DnnPlatformContext: Platform, IPlatformInfo
{
    public override PlatformType Type => PlatformType.Dnn;

    public override Version Version => DotNetNukeContext.Current.Application.Version;

    string IPlatformInfo.Identity => DotNetNuke.Entities.Host.Host.GUID;
}