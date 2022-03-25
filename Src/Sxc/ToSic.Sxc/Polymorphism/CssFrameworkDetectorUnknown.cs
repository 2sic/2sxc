using Connect.Koi.Detectors;
using ToSic.Eav.Run.Unknown;

namespace ToSic.Sxc.Polymorphism
{
    public class CssFrameworkDetectorUnknown: ICssFrameworkDetector
    {
        public CssFrameworkDetectorUnknown(WarnUseOfUnknown<CssFrameworkDetectorUnknown> warn) { }

        public string AutoDetect() => Connect.Koi.CssFrameworks.Unknown;
    }
}
