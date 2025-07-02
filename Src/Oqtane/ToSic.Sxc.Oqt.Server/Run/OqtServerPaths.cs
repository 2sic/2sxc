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

    public static string GetAppRootWithSiteId(int siteId)
        => string.Format(OqtConstants.AppRootPublicBase, siteId);

    public static string GetAppPath(int siteId, string appFolder)
        => Path.Combine(GetAppRootWithSiteId(siteId), appFolder);

    public static string GetAppApiPath(int siteId, string appFolder, string apiPath)
        => Path.Combine(GetAppPath(siteId, appFolder), apiPath);
}