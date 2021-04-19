using Connect.Koi.Detectors;

namespace ToSic.Sxc.Oqt.Server.Extensions.Koi
{
    class OqtKoiCssFrameworkDetector: ICssFrameworkDetector
    {
        public string AutoDetect() => Connect.Koi.CssFrameworks.Bootstrap4;
    }
}
