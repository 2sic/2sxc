using System.Web.Http.Controllers;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code.Internal.HotBuild;

namespace ToSic.Sxc.Dnn.WebApi.Internal;

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

        memoryCacheService.SetNew(key: descriptorCacheKey, value: data, func: p => p
            .SetSlidingExpiration(new TimeSpan(1, 0, 0))
            .WatchCacheKeys(cacheKeys)
            .WatchFiles(filePaths)
        );
        // Ported 2024-10-22 - remove old code ca. 2024-12 #MemoryCacheApiCleanUp
        //memoryCacheService.Set(key: descriptorCacheKey, 
        //    value: data,
        //    slidingExpiration: new TimeSpan(1, 0, 0),
        //    cacheKeys: cacheKeys,
        //    filePaths: filePaths);

        PreservePathForGetCodeInController(data.Folder, data.FullPath);

        return data.Descriptor;
    }
}