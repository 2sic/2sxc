using System;
using Connect.Koi.Detectors;

namespace ToSic.Sxc.Oqt.Server.Polymorphism;

internal class OqtKoiCssFrameworkDetector : ICssFrameworkDetector
{
    private string _bootstrapVersion;

    public string AutoDetect()
    {
        return _bootstrapVersion ??= GetBootstrapVersion();
    }

    private static string GetBootstrapVersion()
    {
        var oqtaneVersion = GetOqtaneVersion();

        // bs5 for 2.2
        if (oqtaneVersion >= new Version(2, 2))
            return Connect.Koi.CssFrameworks.Bootstrap5;

        // bs4 for < 2.2
        return Connect.Koi.CssFrameworks.Bootstrap4;
    }

    private static Version GetOqtaneVersion()
    {
        return Version.TryParse(Oqtane.Shared.Constants.Version, out var ver) ? ver : new(1, 0);
    }
}