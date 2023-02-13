using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Urls;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Helpers;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources
{
    [PrivateApi]
    public class DnnSitesDsProvider: SitesDataSourceProvider
    {
        public DnnSitesDsProvider(Dependencies dependencies) : base(dependencies, "Dnn.Sites")
        { }

        public override List<CmsSiteInfo> GetSitesInternal(
        ) => Log.Func($"PortalId: {PortalSettings.Current?.PortalId ?? -1}", l =>
        {
            var portals = PortalController.Instance.GetPortals().OfType<PortalInfo>().ToList();

            if (/*portals == null || */!portals.Any()) return (new List<CmsSiteInfo>(), "null/empty");

            var result = portals
                .Select(s => new CmsSiteInfo
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
            return (result, $"found {result.Count}");

        });

        private string GetUrl(int portalId, string cultureCode)
        {
            var primaryPortalAlias = PortalAliasController.Instance.GetPortalAliasesByPortalId(portalId)
                .GetAliasByPortalIdAndSettings(portalId, result: null, cultureCode, settings: new FriendlyUrlSettings(portalId));
            return primaryPortalAlias.HTTPAlias;
        }

        //private bool AllowRegistration(int userRegistration) =>
        //    userRegistration != (int)Globals.PortalRegistrationType.NoRegistration 
        //    && userRegistration != (int)Globals.PortalRegistrationType.PrivateRegistration;
    }
}
