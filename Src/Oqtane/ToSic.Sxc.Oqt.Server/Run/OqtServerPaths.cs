using Microsoft.AspNetCore.Hosting;
using Oqtane.Repository;
using ToSic.Eav.Environment.Sys.ServerPaths;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Oqt.Server.Run;

internal class OqtServerPaths(IWebHostEnvironment hostingEnvironment, LazySvc<IFileRepository> fileRepository)
    : ServerPathsBase
{
    public override string FullAppPath(string virtualPath) => FullContentPath(virtualPath);


    public override string FullContentPath(string virtualPath)
    {
        var path = virtualPath.Backslash().TrimPrefixSlash();
        return Path.Combine(hostingEnvironment.ContentRootPath, path);
    }


    protected override string FullPathOfReference(int id)
        => fileRepository.Value.GetFilePath(id);

    public static string GetAppRoot(OqtTenantSiteIdentity identity)
        => identity.TenantId == OqtConstants.MasterTenantId
            ? string.Format(OqtConstants.AppRootPublicBase, identity.SiteId)
            : string.Format(OqtConstants.AppRootTenantSiteBase, identity.TenantId, identity.SiteId);

    public static string GetAppRoot(int tenantId, int siteId)
        => GetAppRoot(new OqtTenantSiteIdentity(tenantId, siteId));

    public static string GetAppRootWithSiteId(int siteId)
        => GetAppRoot(new OqtTenantSiteIdentity(OqtConstants.MasterTenantId, siteId));

    public static string GetAppPath(OqtTenantSiteIdentity identity, string appFolder)
        => Path.Combine(GetAppRoot(identity), appFolder);

    public static string GetAppPath(int tenantId, int siteId, string appFolder)
        => GetAppPath(new OqtTenantSiteIdentity(tenantId, siteId), appFolder);

    public static string GetAppPath(int siteId, string appFolder)
        => GetAppPath(OqtConstants.MasterTenantId, siteId, appFolder);

    public static string GetAppApiPath(OqtTenantSiteIdentity identity, string appFolder, string apiPath)
        => Path.Combine(GetAppPath(identity, appFolder), apiPath);

    public static string GetAppApiPath(int tenantId, int siteId, string appFolder, string apiPath)
        => GetAppApiPath(new OqtTenantSiteIdentity(tenantId, siteId), appFolder, apiPath);

    public static string GetAppApiPath(int siteId, string appFolder, string apiPath)
        => GetAppApiPath(OqtConstants.MasterTenantId, siteId, appFolder, apiPath);
}
