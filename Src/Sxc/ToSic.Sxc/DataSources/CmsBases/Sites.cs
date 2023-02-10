using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Eav.DataSources.Queries;
using ToSic.Lib.Documentation;
using ToSic.Lib.Logging;

// Important Info to people working with this
// It depends on abstract provider, that must be overriden in each platform
// In addition, each platform must make sure to register a TryAddTransient with the platform specific provider implementation
// This is because any constructor DI should be able to target this type, and get the real provider implementation

namespace ToSic.Sxc.DataSources
{
    /// <summary>
    /// Deliver a list of sites from the Oqtane
    ///
    /// As of now there are no parameters to set.
    ///
    /// To figure out the properties returned and what they match up to, see <see cref="CmsSiteInfo"/>
    /// </summary>
    [PublicApi]
    [VisualQuery(
        ExpectsDataOfType = "",
        GlobalName = "a11c28fb-7d8d-40a2-a22c-50beaa019e41",
        HelpLink = "https://r.2sxc.org/ds-sites",
        Icon = Icons.Scopes,
        NiceName = "Sites",
        Type = DataSourceType.Source,
        UiHint = "Sites in this CMS")]
    public class Sites: ExternalData
    {
        private readonly SitesDataSourceProvider _provider;

        #region Constructor

        [PrivateApi]
        public Sites(Dependencies dependencies, SitesDataSourceProvider provider) : base(dependencies, "CDS.Sites")
        {
            ConnectServices(
                _provider = provider
            );
            Provide(GetSites);
        }
        #endregion

        [PrivateApi]
        public IImmutableList<IEntity> GetSites() => Log.Func(l =>
        {
            Configuration.Parse();

            // Get sites from underlying system/provider
            var sitesFromSystem = _provider.GetSitesInternal();
            if (sitesFromSystem == null || !sitesFromSystem.Any())
                return (new List<IEntity>().ToImmutableList()/*new ImmutableArray<IEntity>()*/, "null/empty");

            // Convert to Entity-Stream
            var builder = new DataBuilderQuickWIP(DataBuilder, typeName: "Sites", titleField: nameof(CmsSiteInfo.Name));
            var sites = sitesFromSystem
                .Select(s => builder.CreateWithEavNullId(s))
                .ToImmutableList();

            return (sites, $"found: {sites.Count}");
        });
    }
}
