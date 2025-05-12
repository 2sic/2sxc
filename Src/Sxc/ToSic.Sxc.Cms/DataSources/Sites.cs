﻿using ToSic.Eav.DataSource;
using ToSic.Eav.DataSource.Internal;
using ToSic.Eav.DataSource.VisualQuery;
using ToSic.Sxc.Cms.Sites;
using ToSic.Sxc.Cms.Sites.Internal;
using ToSic.Sxc.DataSources.Internal;

// Important Info to people working with this
// It depends on abstract provider, that must be overriden in each platform
// In addition, each platform must make sure to register a TryAddTransient with the platform specific provider implementation
// This is because any constructor DI should be able to target this type, and get the real provider implementation

namespace ToSic.Sxc.DataSources;

/// <summary>
/// Deliver a list of sites from the Oqtane
/// </summary>
/// <remarks>
/// You can cast the result to <see cref="ISiteModel"/> for typed use in your code.
/// To figure out the returned properties, best also consult the <see cref="ISiteModel"/>.
///
/// As of now there are no parameters to set.
/// 
/// History
///
/// * Not sure when it was first created, probably early 2023 with the name `Roles`, and not officially communicated.
/// * Model <see cref="ISiteModel"/> created in v19.01 and officially released
/// </remarks>

[PublicApi]
[VisualQuery(
    ConfigurationType = "",
    NameId = "a11c28fb-7d8d-40a2-a22c-50beaa019e41",
    HelpLink = "https://go.2sxc.org/ds-sites",
    Icon = DataSourceIcons.Globe,
    NiceName = "Sites",
    Type = DataSourceType.Source,
    UiHint = "Sites in this system")]
public class Sites: CustomDataSource
{
    [PrivateApi]
    public Sites(MyServices services, SitesDataSourceProvider sitesProvider) : base(services, logName: "CDS.Sites")
    {
        ConnectLogs([sitesProvider]);
        ProvideOutRaw(sitesProvider.GetSitesInternal, options: () => SiteModel.Options);
    }
}