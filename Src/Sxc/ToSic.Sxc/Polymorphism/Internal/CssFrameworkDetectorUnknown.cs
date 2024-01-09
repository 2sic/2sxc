using Connect.Koi.Detectors;
using ToSic.Eav.Internal.Unknown;

namespace ToSic.Sxc.Polymorphism.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class CssFrameworkDetectorUnknown(WarnUseOfUnknown<CssFrameworkDetectorUnknown> _) : ICssFrameworkDetector
{
    public string AutoDetect() => Connect.Koi.CssFrameworks.Unknown;
}