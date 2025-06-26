using ToSic.Lib.Services;
using ToSic.Sys.Capabilities.Features;

namespace ToSic.Sxc.Services;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class FeaturesService(ISysFeaturesService sysFeaturesSvc)
    : ServiceBase($"{SxcLogName}.FeatSv"), IFeaturesService, ICanDebug
{
    public bool IsEnabled(params string[] nameIds)
    {
        var result = sysFeaturesSvc.IsEnabled(nameIds);
        if (!Debug) return result;
        var l = Log.Fn<bool>(string.Join(",", nameIds ?? []));
        return l.Return(result, $"{result}");
    }

    public bool Debug { get; set; }
}