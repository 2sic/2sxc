using System.Text;
using ToSic.Eav.Apps.ImportExport.Sys;
using ToSic.Eav.Persistence.Sys.Logging;
using ToSic.Eav.Sys;
using ToSic.Sxc.Context;
using ToSic.Sxc.Sys.Integration.Installation;

namespace ToSic.Sxc.Backend.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class InstallControllerReal(
    LazySvc<IEnvironmentInstaller> envInstallerLazy,
    LazySvc<ImportFromRemote> impFromRemoteLazy)
    : ServiceBase($"{EavLogs.WebApi}.{LogSuffix}Rl", connect: [envInstallerLazy, impFromRemoteLazy])
{
    public const string LogSuffix = "Install";

    #region System Installation

    /// <summary>
    /// Finish system installation which had somehow been interrupted
    /// </summary>
    /// <returns></returns>
    public bool Resume() => envInstallerLazy.Value.ResumeAbortedUpgrade();

    #endregion

    /// <summary>
    /// Before this was GET Installer/InstallPackage
    /// </summary>
    /// <param name="packageUrl"></param>
    /// <param name="container"></param>
    /// <param name="newName"></param>
    /// <returns></returns>
    public string? RemotePackage(string packageUrl, IModule container, string? newName = null)
    {
        var l = Log.Fn<string>();

        var isApp = !container.IsContent;

        l.A($"install package:'{packageUrl}', {nameof(newName)}:'{newName}'");

        var block = container.BlockIdentifier;
        var (success, messages) = impFromRemoteLazy.Value
            .InstallPackage(block.ZoneId, block.AppId, isApp, packageUrl, newName);

        l.A($"install completed with success:{success}");

        return success 
            ? l.ReturnAsOk(null) 
            : l.Return(MessageBuilder(messages));
    }

    private static string MessageBuilder(List<Message> messages)
    {
        var err = new StringBuilder();
        foreach (var m in messages)
            err.AppendLine(m.Text);
        return err.ToString();
    }

}