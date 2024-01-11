using System;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Lib.Coding;
using ToSic.Lib.Logging;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Services.Internal;

/// <summary>
/// WIP - goal is to have a DI factory which creates DynamicCode objects for use in Skins and other external controls
/// Not sure how to get this to work, since normally we always start with a code-file, and here we don't have one!
/// </summary>
public partial class DynamicCodeService
{
    /// <inheritdoc />
    public IApp App(NoParamOrder noParamOrder = default, int? zoneId = null, int? appId = null, ISite site = null, bool? withUnpublished = null)
    {
        MakeSureLogIsInHistory();

        // Ensure AppId is provided
        var realAppId = appId ?? throw new ArgumentException($"At least the {nameof(appId)} is required and must be a valid AppId", nameof(appId));

        // lookup zoneId if not provided
        var realZoneId = zoneId ?? Services.AppStates.Value.IdentityOfApp(realAppId).ZoneId;
        return App(new AppIdentityPure(realZoneId, realAppId), site, withUnpublished: withUnpublished);
    }

    public IApp AppOfSite() => AppOfSite(siteId: null);

    // ReSharper disable once MethodOverloadWithOptionalParameter
    public IApp AppOfSite(NoParamOrder noParamOrder = default, int? siteId = null, ISite site = null, bool? withUnpublished = null)
        => App(GetPrimaryAppIdentity(siteId, site), site, withUnpublished);

    private IAppIdentityPure GetPrimaryAppIdentity(int? siteId, ISite site)
    {
        siteId ??= site?.Id ?? Services.Site.Value.Id;
        var zoneId = Services.ZoneMapper.Value.GetZoneId(siteId.Value);
        var primaryApp = Services.AppStates.Value.IdentityOfPrimary(zoneId);
        return primaryApp;
    }


    private IApp App(IAppIdentityPure appIdentity, ISite site, bool? withUnpublished = null)
    {
        var wrapLog = Log.Fn<IApp>($"{appIdentity.Show()}, site:{site != null}, showDrafts: {withUnpublished}");
        var app = _myScopedServices.AppGenerator.New();
        if (site != null) app.PreInit(site);
        var appStuff = app.Init(appIdentity, _myScopedServices.AppConfigDelegateGenerator.New().Build(withUnpublished));
        return wrapLog.Return(appStuff);
    }

}