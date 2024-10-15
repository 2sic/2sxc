using Imazen.Common.Storage;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ToSic.Eav.Helpers;
using ToSic.Sxc.Oqt.Server.Plumbing;

namespace ToSic.Sxc.Oqt.Server.Adam.Imageflow;

internal class OqtaneBlobService(IServiceProvider serviceProvider) : IBlobProvider
{
    private const string Prefix = "/";
    private const string AdamPath = "/adam/";
    private const string SxcPath = "/assets/";
    private const string SharedPath = "/shared/";

    // TODO: Why do we need the IServiceProvider? this should probably get Generators or something
    // service provider is need to create new scope on each image request

    public IEnumerable<string> GetPrefixes() 
        => Enumerable.Repeat(Prefix, 1);

    public bool SupportsPath(string virtualPath)
        => ContainsSharedPath(virtualPath) || ContainsAdamPath(virtualPath) || ContainsSxcPath(virtualPath);

    public async Task<IBlobData> Fetch(string virtualPath) 
        => await Task.Run(() =>
        {
            if (!SupportsPath(virtualPath)) return null;

            // Get appName and filePath.
            if (!GetAppNameAndFilePath(virtualPath, out var appName, out var filePath))
                return null;

            // Get route.
            var route = GetRoute(virtualPath);

            // We need new scope on each request to avoid sharing the same instance of services.
            using var scope = serviceProvider.CreateScope();

            var webHostEnvironment = scope.ServiceProvider.GetRequiredService<IWebHostEnvironment>();
            if (ExistUnderWebRootPath(webHostEnvironment, virtualPath, out var webRootFilePath)) BlobData(webRootFilePath);

            // Get alias.
            var aliasResolver = scope.ServiceProvider.GetRequiredService<AliasResolver>();
            var alias = aliasResolver.Alias;

            // Build physicalPath.
            var fileHelper = scope.ServiceProvider.GetService<OqtAssetsFileHelper>(); // this service sometimes was not working when was lazy and not scoped
            var physicalPath = fileHelper.GetFilePath(webHostEnvironment.ContentRootPath, alias, route, appName, filePath);
            if (string.IsNullOrEmpty(physicalPath)) throw new BlobMissingException($"Oqtane blob \"{virtualPath}\" not found.");

            return BlobData(physicalPath);
        });

    private static IBlobData BlobData(string physicalPath) 
        => new BlobProviderFile
        {
            Path = physicalPath,
            Exists = true,
            LastModifiedDateUtc = File.GetLastWriteTimeUtc(physicalPath)
        };

    public static bool GetAppNameAndFilePath(string virtualPath, out string appName, out string filePath)
    {
        // setup
        var temp = (ContainsSharedPath(virtualPath)) ? "shared/" : "app/";
        appName = string.Empty;
        filePath = string.Empty;
        // get appName
        var prefixStart = virtualPath.IndexOf(temp, StringComparison.OrdinalIgnoreCase);
        appName = virtualPath.Substring(prefixStart + temp.Length).TrimStart('/');
        var indexOfSlash = appName.IndexOf('/');
        if (indexOfSlash < 1) return false;
        appName = appName.Substring(0, indexOfSlash);

        // get filePath
        if (ContainsAdamPath(virtualPath)) temp = AdamPath;
        if (ContainsSxcPath(virtualPath)) temp = SxcPath;
        if (ContainsSharedPath(virtualPath)) temp = $"{SharedPath}{appName}/";

        var find = virtualPath.IndexOf(temp, StringComparison.OrdinalIgnoreCase);
        if (find < 1) return false;
        filePath = virtualPath.Substring(find + temp.Length).TrimStart('/');

        return true;
    }

    private static bool ContainsSharedPath(string virtualPath)
        => virtualPath.Contains(SharedPath, StringComparison.OrdinalIgnoreCase);

    private static bool ContainsAdamPath(string virtualPath)
        => Regex.IsMatch(virtualPath, @"^.*app/[a-zA-Z \d-_]+/adam/.*$");

    public static bool ContainsSxcPath(string virtualPath)
        => Regex.IsMatch(virtualPath, @"^.*app/[a-zA-Z \d-_]+/assets/.*$");

    private static string GetRoute(string virtualPath)
        => ContainsSharedPath(virtualPath) ? OqtAssetsFileHelper.RouteShared :
            ContainsAdamPath(virtualPath) ? OqtAssetsFileHelper.RouteAdam :
            ContainsSxcPath(virtualPath) ? OqtAssetsFileHelper.RouteAssets : string.Empty;

    private static bool ExistUnderWebRootPath(IWebHostEnvironment webHostEnvironment, string virtualPath, out string filePath)
    {
        var path = virtualPath.Backslash().TrimPrefixSlash();
        filePath = Path.Combine(webHostEnvironment.WebRootPath, path);
        return File.Exists(filePath);
    }
}