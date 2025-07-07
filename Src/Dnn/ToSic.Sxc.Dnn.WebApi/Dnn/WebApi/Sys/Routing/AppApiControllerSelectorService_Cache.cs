using System.Web.Http.Controllers;
using ToSic.Sxc.Code.Sys.HotBuild;

namespace ToSic.Sxc.Dnn.WebApi.Sys;

internal partial class AppApiControllerSelectorService
{
    private static string CacheKey(string appFolder, string controllerTypeName, bool shared, HotBuildSpec spec)
        => $"AppApiCtrl:Descriptor:{appFolder}:{controllerTypeName}:{shared}:{spec.CacheKey()}";

    private HttpControllerDescriptor Get(string appFolder, string editionPath, string controllerTypeName,
        bool shared, HotBuildSpec spec)
    {

        var descriptorCacheKey = CacheKey(appFolder, controllerTypeName, shared, spec);
        if (memoryCacheService.TryGet<HttpControllerDescriptorWithPaths>(descriptorCacheKey, out var dataWithPaths))
        {
            PreservePathForGetCodeInController(dataWithPaths.Folder, dataWithPaths.FullPath);
            return dataWithPaths.Descriptor;
        }

        Log.A($"Descriptor not found in cache, will try to build it ({nameof(descriptorCacheKey)}:'{descriptorCacheKey}')");

        var (data, cacheKeys, filePaths) = BuildDescriptorIfExists(appFolder, editionPath, controllerTypeName, shared, spec);

        memoryCacheService.Set(key: descriptorCacheKey, value: data, func: p => p
            .SetSlidingExpiration(new TimeSpan(1, 0, 0))
            .WatchCacheKeys(cacheKeys)
            .WatchFiles(filePaths)
        );

        PreservePathForGetCodeInController(data.Folder, data.FullPath);

        return data.Descriptor;
    }
}