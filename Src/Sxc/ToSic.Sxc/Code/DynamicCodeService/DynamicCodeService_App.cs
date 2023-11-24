using System;
using ToSic.Eav;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Lib.Logging;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Code;

/// <summary>
/// WIP - goal is to have a DI factory which creates DynamicCode objects for use in Skins and other external controls
/// Not sure how to get this to work, since normally we always start with a code-file, and here we don't have one!
/// </summary>
public partial class DynamicCodeService
{
    /// <inheritdoc />
    public IApp App(
        string noParamOrder = Eav.Parameters.Protector,
        int? zoneId = null,
        int? appId = null,
        ISite site = null,
        bool? withUnpublished = null)
    {
        MakeSureLogIsInHistory();
        Eav.Parameters.ProtectAgainstMissingParameterNames(noParamOrder, nameof(App),
            $"{nameof(zoneId)}, {nameof(appId)} (required), {nameof(site)}, {nameof(withUnpublished)}");

        // Ensure AppId is provided
        var realAppId = appId ?? throw new ArgumentException($"At least the {nameof(appId)} is required and must be a valid AppId", nameof(appId));

        // lookup zoneId if not provided
        var realZoneId = zoneId ?? Services.AppStates.Value.IdentityOfApp(realAppId).ZoneId;
        return App(new AppIdentity(realZoneId, realAppId), site, withUnpublished: withUnpublished);
    }

    public IApp AppOfSite() => AppOfSite(siteId: null);

    // ReSharper disable once MethodOverloadWithOptionalParameter
    public IApp AppOfSite(string noParamOrder = Parameters.Protector, int? siteId = null, ISite site = null, bool? withUnpublished = null)
    {
        var primaryApp = GetPrimaryApp(siteId, site);
        return App(primaryApp, site, withUnpublished);
    }

    private AppState GetPrimaryApp(int? siteId, ISite site)
    {
        siteId = siteId ?? site?.Id ?? Services.Site.Value.Id;
        var zoneId = Services.ZoneMapper.Value.GetZoneId(siteId.Value);
        var primaryApp = Services.AppStates.Value.GetPrimaryApp(zoneId, Log);
        return primaryApp;
    }


    private IApp App(IAppIdentity appIdentity, ISite site, bool? withUnpublished = null)
    {
        var wrapLog = Log.Fn<IApp>($"{appIdentity.Show()}, site:{site != null}, showDrafts: {withUnpublished}");
        var app = _myScopedServices.AppGenerator.New();
        if (site != null) app.PreInit(site);
        var appStuff = app.Init(appIdentity, _myScopedServices.AppConfigDelegateGenerator.New().Build(withUnpublished));
        return wrapLog.Return(appStuff);
    }

}