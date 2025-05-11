using ToSic.Eav.Apps;
using ToSic.Eav.Context;
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
        var realZoneId = zoneId ?? Services.AppsCatalog.Value.AppIdentity(realAppId).ZoneId;
        return App(new AppIdentityPure(realZoneId, realAppId), site, showDrafts: withUnpublished);
    }

    /// <inheritdoc />
    public IApp AppOfSite() => AppOfSite(noParamOrder: default);

    /// <inheritdoc />
    // ReSharper disable once MethodOverloadWithOptionalParameter
    public IApp AppOfSite(NoParamOrder noParamOrder = default, int? siteId = null, ISite site = null, bool? withUnpublished = null)
        => App(GetPrimaryAppIdentity(siteId, site), site, withUnpublished);

    private IAppIdentityPure GetPrimaryAppIdentity(int? siteId, ISite site = default)
    {
        siteId ??= site?.Id ?? Services.Site.Value.Id;
        var zoneId = Services.ZoneMapper.Value.GetZoneId(siteId.Value);
        var primaryApp = Services.AppsCatalog.Value.PrimaryAppIdentity(zoneId);
        return primaryApp;
    }


    private IApp App(IAppIdentityPure appIdentity, ISite site, bool? showDrafts = null)
    {
        var l = Log.Fn<IApp>($"{appIdentity.Show()}, site:{site != null}, showDrafts: {showDrafts}");
        var app = _myScopedServices.AppGenerator.New();
        // if (site != null) app.PreInit(site);
        app.Init(site, appIdentity, new() { ShowDrafts = showDrafts });
        return l.Return(app);
    }

}