using Connect.Koi.Detectors;
using ToSic.Eav.Internal.Unknown;
#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Polymorphism.Internal;

internal class CssFrameworkDetectorUnknown(WarnUseOfUnknown<CssFrameworkDetectorUnknown> _) : ICssFrameworkDetector
{
    public string AutoDetect() => Connect.Koi.CssFrameworks.Unknown;
}