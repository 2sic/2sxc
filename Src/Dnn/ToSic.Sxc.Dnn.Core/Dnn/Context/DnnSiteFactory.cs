using DotNetNuke.Entities.Modules;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Abstractions.Portals;
using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys.ZoneMapper;
using ToSic.Eav.Sys;
using ToSic.Sxc.Sys.Integration.Paths;
using ToSic.Sxc.Web.Sys.Url;

namespace ToSic.Sxc.Dnn.Context;

/// <summary>
/// This is a DNN implementation of a Tenant-object. 
/// </summary>
internal sealed class DnnSiteFactory(LazySvc<IZoneMapper> zoneMapperLazy, LazySvc<ILinkPaths> linkPathsLazy, DnnCultureResolver cultureResolver)
    : ServiceBase(DnnConstants.LogName, connect: [zoneMapperLazy, linkPathsLazy, cultureResolver])
{
    #region Separating factory from shared global state

    /// <summary>
    /// The shared state, not ideal, but working for now.
    /// In the future, we must find a better way to clearly broadcast the shared site.
    /// </summary>
    public ISite TempSite => TempSiteContents;

    private DnnSite TempSiteContents
    {
        get => field ??= CreateFromPortal(null);
        set;
    }

    #endregion

    #region Swap new Portal Settings into this object

    /// <summary>
    /// Create a brand-new instance of a DNN site object based on PortalSettings.
    /// </summary>
    /// <remarks>
    /// This will NOT update shared state.
    /// Note that it previously did - but I guess all code paths never noticed.
    /// </remarks>
    /// <param name="settingsOrNull">The portal settings to use for creating the DNN site.</param>
    /// <returns>A new instance of a DNN site.</returns>
    internal DnnSite CreateFromPortal(PortalSettings? settingsOrNull)
    {
        var l = Log.Fn<DnnSite>();
        var portalSettings = KeepBestPortalSettings(settingsOrNull);

        var zoneId = portalSettings?.PortalId ?? EavConstants.NullId;
        var urlRoot = (portalSettings?.PortalAlias as IPortalAliasInfo)?.HttpAlias
                      ?? (PortalSettings.Current?.PortalAlias as IPortalAliasInfo)?.HttpAlias
                      ?? "err-portal-alias-not-loaded";
        var url = $"{new UrlParts(linkPathsLazy.Value.GetCurrentRequestUrl()).Protocol}{urlRoot}";
        var currentCultureCode = cultureResolver.GetCurrentCultureCode(portalSettings);
        var defaultCultureCode = portalSettings?.DefaultLanguage?.ToLowerInvariant() ?? string.Empty;
        var cultureCodesWithFallbacks = cultureResolver.GetCultureCodesWithFallbacks(zoneId, currentCultureCode, defaultCultureCode);

        var result = new DnnSite
        {
            Id = zoneId,
            ZoneId = zoneId < 0 ? EavConstants.NullId : zoneMapperLazy.Value.GetZoneId(zoneId),
            Url = url,
            UrlRoot = urlRoot,
            CurrentCultureCode = currentCultureCode, // note 2026-10-18 2dm null warning, not sure yet how to proceed; was always null, but now null-checks are kicking in
            DefaultCultureCode = defaultCultureCode,
            CultureCodesWithFallbacks = cultureCodesWithFallbacks,
            PortalSettings = portalSettings,
        };

        return l.Return(result, $"Site Id {zoneId}");
    }

    internal DnnSite CreateFromModuleAndShare(ModuleInfo module, ILog extLog)
    {
        var l = extLog.Fn<DnnSite>($"Owner Site: {module?.OwnerPortalID}, Current Site: {module?.PortalID}");

        if (module == null)
            return l.Return(TempSiteContents, "no module");
        if (module.OwnerPortalID < 0)
            return l.Return(TempSiteContents, "no change, owner < 0");
        if (module.OwnerPortalID == TempSiteContents.ZoneId)
            return l.Return(TempSiteContents, "no change, owner = temp site");

        // Note: this does look a bit fishy, since creating the object will probably be missing
        // some properties; should probably go through some kind of portal-settings manager or something?
        var modulePortalSettings = new PortalSettings(module.OwnerPortalID);
        var result = CreateFromPortal(modulePortalSettings);

        // Share/Broadcast the new site object, as it will apply to everything now
        // This is how it used to work before v22
        // It's not a good implementation as of now, but the complexity to change this is huge.
        TempSiteContents = result;

        return l.Return(result, "updated with portal settings of module");
    }

    /// <summary>
    /// Very special helper to work around a DNN issue
    /// Reason is that PortalSettings.Current is always "perfect" and also contains root URLs and current Page
    /// Other PortalSettings may not contain this (partially populated objects)
    /// In case we're requesting a DnnTenant with incomplete PortalSettings
    /// we want to correct this here
    /// </summary>
    /// <returns></returns>
    private PortalSettings? KeepBestPortalSettings(PortalSettings? settingsOrNull)
    {
        var l = Log.Fn<PortalSettings>();
        // in case we don't have an HTTP Context with current portal settings, don't try anything
        var current = PortalSettings.Current;
        if (current == null)
            return l.Return(settingsOrNull, "null, use given");

        // If we don't have settings, or they point to the same portal, then use that
        var msgKeepCurrent = settingsOrNull switch
        {
            null => "null, use current",
            _ when settingsOrNull == current => "is current, use current",
            _ when settingsOrNull.PortalId == current.PortalId => "id=current, use current",
            _ => null,
        };

        // fallback: use supplied settings
        return l.Return(msgKeepCurrent != null ? current : settingsOrNull, msgKeepCurrent ?? "use new settings");
    }

    #endregion
}