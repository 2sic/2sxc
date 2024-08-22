using System;
using System.Collections.Generic;
using System.Linq;
using Oqtane.Repository;
using Oqtane.Shared;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Internal.Work;
using ToSic.Eav.Cms.Internal.Languages;
using ToSic.Eav.Context;
using ToSic.Eav.Integration;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Sxc.Oqt.Server.Context;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Run;

internal class OqtZoneMapper : ZoneMapperBase
{
    /// <inheritdoc />
    public OqtZoneMapper(ISiteRepository siteRepository, 
        ISettingRepository settingRepository,
        Generator<ISite> site,
        LazySvc<ZoneCreator> zoneCreatorLazy,
        OqtCulture oqtCulture, 
        IAppStates appStates) : base(appStates, $"{OqtConstants.OqtLogPrefix}.ZoneMp")
    {
        ConnectLogs([
            _siteRepository = siteRepository,
            _settingRepository = settingRepository,
            _site = site,
            _zoneCreatorLazy = zoneCreatorLazy,
            _oqtCulture = oqtCulture
        ]);
    }
    private readonly ISiteRepository _siteRepository;
    private readonly ISettingRepository _settingRepository;
    private readonly Generator<ISite> _site;
    private readonly LazySvc<ZoneCreator> _zoneCreatorLazy;
    private readonly OqtCulture _oqtCulture;


    public override int GetZoneId(int tenantId)
    {
        // additional protection against invalid portalId which may come from bad configs and execute in search-index mode
        // see https://github.com/2sic/2sxc/issues/1054
        if (tenantId < 0)
            throw new("Can't get zone for invalid portal ID: " + tenantId);

        if (HasZoneId(tenantId, out var i)) return i;

        // Create new zone automatically
        var portalSettings = _siteRepository.GetSite(tenantId);
        var zoneId = _zoneCreatorLazy.Value.Create(portalSettings.Name + " (Site " + tenantId + ")");
        _settingRepository.AddSetting(new()
        {
            CreatedBy = "2sxc", 
            CreatedOn = DateTime.Now, 
            EntityId = tenantId, 
            EntityName = EntityNames.Site,
            ModifiedBy = "2sxc",
            ModifiedOn = DateTime.Now,
            SettingName = OqtConstants.SiteKeyForZoneId,
            SettingValue = zoneId.ToString()
        });
        return zoneId;
    }

    private bool HasZoneId(int tenantId, out int i)
    {
        var c = _settingRepository.GetSettings(EntityNames.Site, tenantId).ToList();

        var zoneSetting = c.FirstOrDefault(s => s.SettingName == OqtConstants.SiteKeyForZoneId);
        if (zoneSetting != null)
        {
            if (!int.TryParse(zoneSetting.SettingValue, out var zId))
            {
                var msg = $"Got value '{zoneSetting.SettingValue}' for ZoneId but can't convert to int";
                Log.A(msg);
                throw new(msg);
            }
            i = zId;
            return true;
        }

        i = 0;
        return false;
    }

    public override ISite SiteOfZone(int zoneId)
    {
        var sites = _siteRepository.GetSites().ToList();
        var found = sites.FirstOrDefault(p => HasZoneId(p.SiteId, out var zoneOfSite) && zoneOfSite == zoneId);
        return found != null ? ((OqtSite)_site.New()).Init(found) : null;
    }

    public override List<ISiteLanguageState> CulturesWithState(ISite site)
    {
        if (_supportedCultures != null) return _supportedCultures;
        var availableEavLanguages = AppStates.AppsCatalog.Zone(site.ZoneId).Languages;
        _supportedCultures = _oqtCulture.GetSupportedCultures(site.Id, availableEavLanguages);
        return _supportedCultures;
    }
    private List<ISiteLanguageState> _supportedCultures;
}