using Connect.Koi.Detectors;
using ToSic.Eav.Run.Unknown;

namespace ToSic.Sxc.Polymorphism
{
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class CssFrameworkDetectorUnknown: ICssFrameworkDetector
    {
        public CssFrameworkDetectorUnknown(WarnUseOfUnknown<CssFrameworkDetectorUnknown> _) { }

        public string AutoDetect() => Connect.Koi.CssFrameworks.Unknown;
    }
}
