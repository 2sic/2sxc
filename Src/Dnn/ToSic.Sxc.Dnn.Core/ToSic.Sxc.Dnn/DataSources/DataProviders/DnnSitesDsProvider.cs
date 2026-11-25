using DotNetNuke.Entities.Portals;
using DotNetNuke.Abstractions.Portals;
using ToSic.Sxc.Cms.Sites.Sys;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources;

[PrivateApi]
internal class DnnSitesDsProvider(
    SitesDataSourceProvider.Dependencies services,
    IPortalController portalController,
    IPortalAliasService portalAliasService)
    : SitesDataSourceProvider(services, "Dnn.Sites")
{
    public override List<SiteModelRaw> GetSitesInternal()
    {
        var l = Log.Fn<List<SiteModelRaw>>($"PortalId: {PortalSettings.Current?.PortalId ?? -1}");
        var portals = portalController
            .GetPortals()
            .OfType<PortalInfo>()
            .ToList();

        if (!portals.Any())
            return l.Return([], "null/empty");

        var result = portals
            .Select(s => new SiteModelRaw
            {
                Id = ((IPortalInfo)s).PortalId,
                Guid = s.GUID,
                Name = s.PortalName,
                Url = GetUrl(((IPortalInfo)s).PortalId, s.DefaultLanguage).TrimLastSlash(),
                DefaultLanguage = s.DefaultLanguage.ToLower() ?? "",
                Languages = GetLanguages(((IPortalInfo)s).PortalId),
                Created = s.CreatedOnDate,
                Modified = s.LastModifiedOnDate,
                ZoneId = GetZoneId(((IPortalInfo)s).PortalId),
                ContentAppId = GetDefaultAppId(((IPortalInfo)s).PortalId),
                PrimaryAppId = GetPrimaryAppId(((IPortalInfo)s).PortalId)
            })
            .ToList();
        return l.Return(result, $"found {result.Count}");

    }

    private string GetUrl(int portalId, string cultureCode)
    {
        var aliases = portalAliasService.GetPortalAliasesByPortalId(portalId).ToList();
        var primaryPortalAlias = aliases
            .Where(a => string.Compare(a.CultureCode, cultureCode, StringComparison.OrdinalIgnoreCase) == 0 || string.IsNullOrEmpty(a.CultureCode))
            .OrderByDescending(a => a.IsPrimary)
            .ThenByDescending(a => a.CultureCode)
            .FirstOrDefault()
            ?? aliases.FirstOrDefault(a => a.IsPrimary)
            ?? aliases.FirstOrDefault();
        return primaryPortalAlias?.HttpAlias ?? "";
    }

    //private bool AllowRegistration(int userRegistration) =>
    //    userRegistration != (int)Globals.PortalRegistrationType.NoRegistration 
    //    && userRegistration != (int)Globals.PortalRegistrationType.PrivateRegistration;
}
