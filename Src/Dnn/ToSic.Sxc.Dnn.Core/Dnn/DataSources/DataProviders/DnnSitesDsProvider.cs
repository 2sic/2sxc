using DotNetNuke.Entities.Portals;
using ToSic.Eav.Helpers;
using ToSic.Sxc.DataSources.Internal;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources;

[PrivateApi]
internal class DnnSitesDsProvider: SitesDataSourceProvider
{
    public DnnSitesDsProvider(MyServices services) : base(services, "Dnn.Sites")
    { }

    public override List<SiteDataRaw> GetSitesInternal()
    {
        var l = Log.Fn<List<SiteDataRaw>>($"PortalId: {PortalSettings.Current?.PortalId ?? -1}");
        var portals = PortalController.Instance.GetPortals().OfType<PortalInfo>().ToList();

        if (/*portals == null || */!portals.Any()) return l.Return([], "null/empty");

        var result = portals
            .Select(s => new SiteDataRaw
            {
                Id = s.PortalID,
                Guid = s.GUID,
                Name = s.PortalName,
                Url = GetUrl(s.PortalID, s.DefaultLanguage).TrimLastSlash(),
                DefaultLanguage = s.DefaultLanguage.ToLower() ?? "",
                Languages = GetLanguages(s.PortalID),
                Created = s.CreatedOnDate,
                Modified = s.LastModifiedOnDate,
                ZoneId = GetZoneId(s.PortalID),
                ContentAppId = GetDefaultAppId(s.PortalID),
                PrimaryAppId = GetPrimaryAppId(s.PortalID)
            })
            .ToList();
        return l.Return(result, $"found {result.Count}");

    }

    private string GetUrl(int portalId, string cultureCode)
    {
        var primaryPortalAlias = PortalAliasController.Instance.GetPortalAliasesByPortalId(portalId)
            .GetAliasByPortalIdAndSettings(portalId, result: null, cultureCode, settings: new(portalId));
        return primaryPortalAlias.HTTPAlias;
    }

    //private bool AllowRegistration(int userRegistration) =>
    //    userRegistration != (int)Globals.PortalRegistrationType.NoRegistration 
    //    && userRegistration != (int)Globals.PortalRegistrationType.PrivateRegistration;
}