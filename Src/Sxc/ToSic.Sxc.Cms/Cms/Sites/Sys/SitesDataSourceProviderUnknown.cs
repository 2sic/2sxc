#pragma warning disable CS9113 // Parameter is unread.

namespace ToSic.Sxc.Cms.Sites.Sys;

internal class SitesDataSourceProviderUnknown(SitesDataSourceProvider.MyServices services, WarnUseOfUnknown<SitesDataSourceProviderUnknown> _) : SitesDataSourceProvider(services, $"{SxcLogName}.{LogConstants.NameUnknown}")
{
    public override List<SiteModel> GetSitesInternal() => [];
}