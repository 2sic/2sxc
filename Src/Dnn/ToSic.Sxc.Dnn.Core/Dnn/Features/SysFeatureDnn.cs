using ToSic.Eav.Internal.Features;
using ToSic.Eav.SysData;

namespace ToSic.Sxc.Dnn.Features;

// ReSharper disable once UnusedMember.Global - since it's used by reflection
internal class SysFeatureDnn() : SysFeatureDetector(DefStatic, true)
{
    private static readonly SysFeature DefStatic = new()
    {
        NameId = "Dnn",
        Guid = new("00cb8d98-bfac-4a18-b7de-b1237498f183"),
        Name = "Dnn",
        LicenseRules = BuiltInFeatures.SystemEnabled,
    };
}