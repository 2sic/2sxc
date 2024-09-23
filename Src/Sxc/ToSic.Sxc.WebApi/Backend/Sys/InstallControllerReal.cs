using System.Text;
using ToSic.Eav.Apps.Services;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Internal.Features;
using ToSic.Eav.Persistence.Logging;
using ToSic.Eav.WebApi.ImportExport;
using ToSic.Eav.WebApi.Infrastructure;
using ToSic.Eav.WebApi.Sys;
using ToSic.Sxc.Backend.App;
using ToSic.Sxc.Context;
using ToSic.Sxc.Integration.Installation;
using IFeaturesService = ToSic.Sxc.Services.IFeaturesService;
using ServiceBase = ToSic.Lib.Services.ServiceBase;
#if NETFRAMEWORK
using THttpResponseType = System.Net.Http.HttpResponseMessage;
#else
using THttpResponseType = Microsoft.AspNetCore.Mvc.IActionResult;
#endif

namespace ToSic.Sxc.Backend.Sys;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class InstallControllerReal(
    LazySvc<IContextOfSite> context,
    LazySvc<IEnvironmentInstaller> envInstallerLazy,
    LazySvc<IPlatformAppInstaller> platformAppInstaller,
    LazySvc<ImportFromRemote> impFromRemoteLazy,
    IResponseMaker responseMaker,
    LazySvc<IFeaturesService> featureService,
    LazySvc<AppsBackend> appsBackend,
    LazySvc<AppDataStackService> appSettingsStack)
    : ServiceBase($"{Eav.EavLogs.WebApi}.{LogSuffix}Rl",
        connect:
        [
            context, envInstallerLazy, platformAppInstaller, impFromRemoteLazy, responseMaker, featureService,
            appSettingsStack, appsBackend
        ])
{
    public const string LogSuffix = "Install";

    #region System Installation

    /// <summary>
    /// Finish system installation which had somehow been interrupted
    /// </summary>
    /// <returns></returns>
    public bool Resume() => envInstallerLazy.Value.ResumeAbortedUpgrade();

    #endregion

    #region App / Content Package Installation

    public InstallAppsDto InstallSettings(bool isContentApp, IModule module)
    {
        // Get Remote Install URL
        var site = context.Value.Site;
        var url = platformAppInstaller.Value
            .GetAutoInstallPackagesUiUrl(site, module, isContentApp);

        // Get list of already installed Apps
        var appsOfThisSite = appsBackend.Value.Apps()
            .Select(a => new AppDtoLight
            {
                name = a.Name,
                guid = a.Guid,
                version = a.Version,
            })
            .ToList();

        // Get list of allow/forbid rules for the App installer
        var settingsSources = appSettingsStack.Value
            .InitForPrimaryAppOfZone(site.ZoneId)
            .GetStack(AppStackConstants.Settings);
        var stack = new PropertyStack().Init(AppStackConstants.RootNameSettings, settingsSources);

        var rules = stack.InternalGetPath(new PropReqSpecs("SiteSetup.AutoInstallApps", PropReqSpecs.EmptyDimensions, true, Log), null);
        var ruleEntities = rules.Result as IEnumerable<IEntity>;    // note: Result is null if nothing found...
        var rulesFinal = ruleEntities?
            .Select(e => new SiteSetupAutoInstallAppsRule(e).GetRuleDto())
            .ToList();

        if (!featureService.Value.IsEnabled(BuiltInFeatures.AppAutoInstallerConfigurable.NameId))
        {
            Log.A("will not add installer rules as the feature is not enabled");
            rulesFinal = [];
        }

        return new()
        {
            remoteUrl = url,
            installedApps = appsOfThisSite,
            rules = rulesFinal
        };
    }

    /// <summary>
    /// Before this was GET Installer/InstallPackage
    /// </summary>
    /// <param name="packageUrl"></param>
    /// <param name="container"></param>
    /// <returns></returns>
    public THttpResponseType RemotePackage(string packageUrl, IModule container)
    {
        var l = Log.Fn<THttpResponseType>();

        var isApp = !container.IsContent;

        Log.A("install package:" + packageUrl);

        var block = container.BlockIdentifier;
        var (success, messages) = impFromRemoteLazy.Value
            .InstallPackage(block.ZoneId, block.AppId, isApp, packageUrl);

        Log.A("install completed with success:" + success);

        return success 
            ? l.ReturnAsOk(responseMaker.Ok()) 
            : l.Return(responseMaker.InternalServerError(MessageBuilder(messages)), "error");
    }

    private static string MessageBuilder(List<Message> messages)
    {
        var err = new StringBuilder();
        foreach (var m in messages) err.AppendFormat("{0}", m.Text);
        return err.ToString();
    }

    #endregion
}