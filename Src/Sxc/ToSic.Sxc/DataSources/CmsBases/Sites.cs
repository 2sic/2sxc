using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Eav.DataSources;
using ToSic.Lib.Documentation;

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
    /// To figure out the properties returned and what they match up to, see <see cref="SiteDataRaw"/>
    /// </summary>
    [PublicApi]
    [VisualQuery(
        ConfigurationType = "",
        NameId = "a11c28fb-7d8d-40a2-a22c-50beaa019e41",
        HelpLink = "https://r.2sxc.org/ds-sites",
        Icon = Icons.Globe,
        NiceName = "Sites",
        Type = DataSourceType.Source,
        UiHint = "Sites in this CMS")]
    public class Sites: CustomDataSource
    {
        [PrivateApi]
        public Sites(MyServices services, SitesDataSourceProvider sitesProvider) : base(services, logName: "CDS.Sites")
        {
            ConnectServices(sitesProvider);
            ProvideOutRaw(sitesProvider.GetSitesInternal, options: () => SiteDataRaw.Options);
        }
    }
}
