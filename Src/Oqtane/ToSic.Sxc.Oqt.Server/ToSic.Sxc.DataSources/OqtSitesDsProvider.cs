using Oqtane.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Helpers;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;
using ToSic.Sxc.Oqt.Server.Context;

// ReSharper disable once CheckNamespace
namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// Deliver a list of sites from the Oqtane
    /// </summary>
    [PrivateApi]
    public class OqtSitesDsProvider : SitesDataSourceProvider
    {
        private readonly IAliasRepository _aliases;
        private readonly ISiteRepository _sites;
        private readonly LazySvc<OqtCulture> _oqtCulture;

        #region Constructor / DI

        public OqtSitesDsProvider(MyServices services, IAliasRepository aliases, ISiteRepository sites, LazySvc<OqtCulture> oqtCulture)
        :base(services, "Oqt.Sites")
        {
            ConnectServices(
                _aliases = aliases,
                _sites = sites,
                _oqtCulture = oqtCulture
            );
        }

        #endregion

        public override List<CmsSiteInfo> GetSitesInternal() => Log.Func(l =>
        {
            var sites = _sites.GetSites().ToList();
            return sites.Select(s => new CmsSiteInfo
            {
                Id = s.SiteId,
                Guid = new(s.SiteGuid),
                Name = s.Name,
                Url = GetUrl(s.SiteId),
                Languages = GetLanguages(s.SiteId),
                DefaultLanguage = _oqtCulture.Value.DefaultLanguageCode(s.SiteId),
                Created = s.CreatedOn,
                Modified = s.ModifiedOn,
                ZoneId = GetZoneId(s.SiteId),
                ContentAppId = GetDefaultAppId(s.SiteId),
                PrimaryAppId = GetPrimaryAppId(s.SiteId)
            }).ToList();
        });

        private string GetUrl(int siteId)
        {
            var alias = _aliases.GetAliases().FirstOrDefault(a => a.SiteId == siteId && a.IsDefault);
                        //?? _aliases.GetAliases().FirstOrDefault(a => a.SiteId == siteId);
            return $"{alias?.Protocol}{alias?.Name}/{alias?.Path}".TrimLastSlash();
        }
    }
}
