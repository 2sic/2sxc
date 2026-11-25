using System.Runtime.CompilerServices;

namespace ToSic.Sxc.Dnn.Install;

internal class DnnInstallLoggerForVersion(DnnInstallLogger logger, string version)
{
    public void LogAuto(string message, [CallerMemberName] string cname = null)
        => logger.LogStep(version, message, true);

    public void LogUnimportant(string message, [CallerMemberName] string cname = null)
        => logger.LogStep(version, $"{cname} - {message}", false);
}
