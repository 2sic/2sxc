using ToSic.Eav.Internal.Features;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Services;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class FeaturesService(IEavFeaturesService root)
    : ServiceBase($"{SxcLogName}.FeatSv"), IFeaturesService, ICanDebug
{
    public bool IsEnabled(params string[] nameIds)
    {
        var result = root.IsEnabled(nameIds);
        if (!Debug) return result;
        var wrapLog = Log.Fn<bool>(string.Join(",", nameIds ?? Array.Empty<string>()));
        return wrapLog.Return(result, $"{result}");
    }

    public bool Debug { get; set; }
}