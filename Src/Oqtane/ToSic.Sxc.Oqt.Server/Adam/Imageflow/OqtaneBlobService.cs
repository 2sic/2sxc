using Imazen.Common.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Oqtane.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ToSic.Oqt.Helpers;
using ToSic.Sxc.Oqt.Server.Plumbing;
using File = System.IO.File;

namespace ToSic.Sxc.Oqt.Server.Adam.Imageflow
{
    public class OqtaneBlobService : IBlobProvider
    {
        private const string Prefix = "/";
        private const string Adam = "adam";
        private const string AdamPath = "/adam/";
        private const string Sxc = "sxc";
        private const string SxcPath = "/assets/";

        private readonly IServiceProvider _serviceProvider;

        public OqtaneBlobService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public IEnumerable<string> GetPrefixes()
        {
            return Enumerable.Repeat(Prefix, 1);
        }

        public bool SupportsPath(string virtualPath)
            => ContainsAdamPath(virtualPath) || ContainsSxcPath(virtualPath);

        public async Task<IBlobData> Fetch(string virtualPath)
        {
            if (!SupportsPath(virtualPath))
            {
                return null;
            }

            // Get appName and filePath.
            if (GetAppNameAndFilePath(virtualPath, out var appName, out var filePath)) return null;

            // Get route.
            var route = GetRoute(virtualPath);

            // Get alias.
            using var scope = _serviceProvider.CreateScope();
            var siteStateInitializer = scope.ServiceProvider.GetRequiredService<SiteStateInitializer>();
            siteStateInitializer.InitIfEmpty();
            var alias = siteStateInitializer.SiteState.Alias;

            var hostingEnvironment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();

            // Build physicalPath.
            var physicalPath = ContentFileHelper.GetFilePath(hostingEnvironment.ContentRootPath, alias, route, appName, filePath);
            if (string.IsNullOrEmpty(physicalPath)) throw new BlobMissingException($"Oqtane blob \"{filePath}\" not found.");

            return new BlobProviderFile()
            {
                Path = physicalPath,
                Exists = true,
                LastModifiedDateUtc = File.GetLastWriteTimeUtc(physicalPath)
            } as IBlobData;
        }

        private static bool GetAppNameAndFilePath(string virtualPath, out string appName, out string filePath)
        {
            // setup
            var temp = "app/";
            appName = string.Empty;
            filePath = string.Empty;

            // get appName
            var prefixStart = virtualPath.IndexOf(temp, StringComparison.OrdinalIgnoreCase);
            appName = virtualPath.Substring(prefixStart + temp.Length).TrimStart('/');
            var indexOfSlash = appName.IndexOf('/');
            if (indexOfSlash < 1) return true;
            appName = appName.Substring(0, indexOfSlash);

            // get filePath
            if (ContainsAdamPath(virtualPath)) temp = AdamPath;
            if (ContainsSxcPath(virtualPath)) temp = SxcPath;
            var find = virtualPath.IndexOf(temp);
            if (find < 1) return true;
            filePath = virtualPath.Substring(find + temp.Length).TrimStart('/');

            return false;
        }

        private static bool ContainsAdamPath(string virtualPath)
            => virtualPath.Contains(AdamPath, StringComparison.OrdinalIgnoreCase);

        private static bool ContainsSxcPath(string virtualPath)
            => virtualPath.Contains(SxcPath, StringComparison.OrdinalIgnoreCase);

        private static string GetRoute(string virtualPath) => ContainsAdamPath(virtualPath) ? Adam :
            ContainsSxcPath(virtualPath) ? Sxc : string.Empty;
    }
}