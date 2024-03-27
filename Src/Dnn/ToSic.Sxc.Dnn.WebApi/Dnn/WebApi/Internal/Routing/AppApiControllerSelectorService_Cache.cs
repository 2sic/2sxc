using System.Web.Http.Controllers;
using ToSic.Lib.Logging;
using ToSic.Sxc.Code.Internal.HotBuild;

namespace ToSic.Sxc.Dnn.WebApi.Internal;

internal partial class AppApiControllerSelectorService
{
    private string CacheKey(string appFolder, string edition, string controllerTypeName, bool shared, HotBuildSpec spec)
        => $"AppApiCtrl:Descriptor:{appFolder}:{controllerTypeName}:{shared}:{spec.CacheKey()}";

    private HttpControllerDescriptor Get(string appFolder, string edition, string controllerTypeName,
        bool shared, HotBuildSpec spec)
    {
        HelperWithPathResolutionForCompilersInsideController(appFolder, edition, controllerTypeName, shared);

        var descriptorCacheKey = CacheKey(appFolder, edition, controllerTypeName, shared, spec);
        if (memoryCacheService.Get(descriptorCacheKey) is HttpControllerDescriptor dataFromCache) return dataFromCache;

        Log.A($"Descriptor not found in cache, will try to build it ({nameof(descriptorCacheKey)}:'{descriptorCacheKey}')");

        var (data, policy) = BuildDescriptorIfExists(appFolder, edition, controllerTypeName, shared, spec);
        memoryCacheService.Set(new(descriptorCacheKey, data), policy);
        return data;
    }
}