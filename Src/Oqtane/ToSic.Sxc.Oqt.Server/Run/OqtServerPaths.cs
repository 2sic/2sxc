using Microsoft.AspNetCore.Hosting;
using Oqtane.Infrastructure;
using Oqtane.Repository;
using ToSic.Eav.Environment.Sys.ServerPaths;
using ToSic.Sxc.Oqt.Server.Adam;
using ToSic.Sxc.Oqt.Shared;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Oqt.Server.Run;

internal class OqtServerPaths(IWebHostEnvironment hostingEnvironment, LazySvc<IFileRepository> fileRepository, LazySvc<ITenantManager> tenantManager)
    : IServerPaths
{
    public string FullAppPath(string virtualPath) => FullContentPath(virtualPath);


    public string FullContentPath(string virtualPath)
    {
        var path = virtualPath.TrimPrefixSlash().ToSystemPath();
        return Path.Combine(hostingEnvironment.ContentRootPath, path);
    }

    public string FullPathOfReference(string fileReference) => ServerPathsShared.FullPathOfReference(fileReference, FullPathOfReference);


    private string FullPathOfReference(int id)
    {
        // Oqtane 10.3.0 removed GetFilePath; only local storage providers have physical paths.
        var file = fileRepository.Value.GetFile(id);
        if (file?.Folder == null)
            return null;

        var root = OqtFolderStorage.GetStorageProvider(file.Folder) switch
        {
            OqtFolderStorage.PrivateStorageProvider => hostingEnvironment.ContentRootPath,
            OqtFolderStorage.PublicStorageProvider => hostingEnvironment.WebRootPath,
            _ => null
        };
        if (root == null)
            return null;

        var sitePath = string.Format(OqtConstants.ContentRootPublicBase, tenantManager.Value.GetTenant().TenantId, file.Folder.SiteId);
        return Path.Combine(root, sitePath, file.Folder.Path, file.Name).ToSystemPath();
    }

    public static string GetAppRootWithTenantAndSiteId(int tenantId, int siteId)
        => string.Format(OqtConstants.AppRootTenantSiteBase, tenantId, siteId);

    public static string GetAppPath(int tenantId, int siteId, string appFolder)
        => Path.Combine(GetAppRootWithTenantAndSiteId(tenantId, siteId), appFolder);

    public static string GetAppApiPath(int tenantId, int siteId, string appFolder, string apiPath)
        => Path.Combine(GetAppPath(tenantId, siteId, appFolder), apiPath);
}
