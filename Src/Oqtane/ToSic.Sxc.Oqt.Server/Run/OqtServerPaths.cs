using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using ToSic.Eav.Helpers;
using ToSic.Eav.Run;
using ToSic.Sxc.Oqt.Shared;

namespace ToSic.Sxc.Oqt.Server.Run
{
    public class OqtServerPaths : IServerPaths
    {
        public OqtServerPaths(IWebHostEnvironment hostingEnvironment) => _hostingEnvironment = hostingEnvironment;
        private readonly IWebHostEnvironment _hostingEnvironment;


        public string FullAppPath(string virtualPath) => FullContentPath(virtualPath);


        public string FullContentPath(string virtualPath)
        {
            var path = virtualPath.Backslash().TrimPrefixSlash();
            return Path.Combine(_hostingEnvironment.ContentRootPath, path);
        }

        public static string GetAppRootWithSiteId(int siteId)
        {
            return string.Format(OqtConstants.AppRootPublicBase, siteId);
        }

        public static string GetAppPath(int siteId, string appFolder)
        {
            return Path.Combine(GetAppRootWithSiteId(siteId), appFolder);
        }

        public static string GetAppApiPath(int siteId, string appFolder, string apiPath)
        {
            return Path.Combine(GetAppPath(siteId, appFolder), apiPath);
        }
    }

}
