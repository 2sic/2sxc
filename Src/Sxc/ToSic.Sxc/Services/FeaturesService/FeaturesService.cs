using ToSic.Eav.Internal.Features;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Services;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class FeaturesService(IEavFeaturesService eavFeaturesSvc)
    : ServiceBase($"{SxcLogName}.FeatSv"), IFeaturesService, ICanDebug
{
    public bool IsEnabled(params string[] nameIds)
    {
        var result = eavFeaturesSvc.IsEnabled(nameIds);
        if (!Debug) return result;
        var l = Log.Fn<bool>(string.Join(",", nameIds ?? []));
        return l.Return(result, $"{result}");
    }

    public bool Debug { get; set; }
}