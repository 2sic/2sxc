using System.Web.Http.Controllers;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Code.Sys.HotBuild;

namespace ToSic.Sxc.Dnn.WebApi.Custom.Sys;

partial class DnnAppApiControllerService
{
    /// <summary>
    /// Get the controller descriptor, if necessary by compiling the code or by finding it in the AppCode.
    /// </summary>
    /// <param name="configuration">Http config for creating the descriptor definition (in the compiler)</param>
    /// <param name="request">Http request for caching some values for later pickup</param>
    /// <param name="appFolder">Folder of the app, where to find the controller</param>
    /// <param name="editionPath">Optional edition (subfolder)</param>
    /// <param name="useSharedLocation">Check the shared apps folder (not the one of the current site)</param>
    /// <param name="controllerName">Name of the controller type</param>
    /// <param name="spec">Hot build specification</param>
    /// <returns>HttpControllerDescriptor</returns>
    private HttpControllerDescriptor GetControllerDescriptor(
        HttpConfiguration configuration,
        HttpRequestMessage request,
        string appFolder,
        string editionPath,
        bool useSharedLocation,
        string controllerName,
        HotBuildSpec spec)
    {
        // 1. Check if we can find the descriptor in the cache
        var descriptorCacheKey = $"AppApiCtrl:Descriptor:{appFolder}:{controllerName}:{useSharedLocation}:{spec.CacheKey()}";
        var l = Log.Fn<HttpControllerDescriptor>(descriptorCacheKey);
        if (cacheSvc.TryGet<HttpControllerDescriptorWithPaths>(descriptorCacheKey, out var dataWithPaths))
        {
            PreservePathForGetCodeInController(request, dataWithPaths.Folder, dataWithPaths.FullPath);
            return l.Return(dataWithPaths.Descriptor, "from cache");
        }

        // 2. Not yet found, try to build/compile it
        l.A($"Descriptor not in cache; try to build/compile");
        var (data, cacheKeys, filePaths)
            = apiCompilerLazy.Value.BuildDescriptorIfExists(configuration, appFolder, editionPath, controllerName, useSharedLocation, spec);

        // 3. No matter if found or not, cache the result, watching for file system changes
        cacheSvc.Set(
            key: descriptorCacheKey,
            value: data,
            func: p => p
                .SetSlidingExpiration(new TimeSpan(1, 0, 0))
                .WatchCacheKeys(cacheKeys)
                .WatchFiles(filePaths)
        );

        PreservePathForGetCodeInController(request, data.Folder, data.FullPath);

        return l.ReturnAsOk(data.Descriptor);
    }

    private static void PreservePathForGetCodeInController(HttpRequestMessage request, string controllerFolder, string controllerPath)
    {
        if (request == null)
            return;
        request.Properties.Add(SourceCodeConstants.SharedCodeRootPathKeyInCache, controllerFolder);
        request.Properties.Add(SourceCodeConstants.SharedCodeRootFullPathKeyInCache, controllerPath);
    }

}