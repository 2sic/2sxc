using ToSic.Eav.Caching;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Code.Internal.HotBuild;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class AssemblyCacheManager(MemoryCacheService memoryCacheService) : ServiceBase(SxcLogName + ".AssCMn")
{
    #region Static Calls for AppCode - to use before requiring DI
    public (AssemblyResult Result, string cacheKey) TryGetAppCode(HotBuildSpec spec)
    {
        var cacheKey = KeyAppCode(spec);
        return (memoryCacheService.Get<AssemblyResult>(cacheKey), cacheKey);
    }
    private static string KeyAppCode(HotBuildSpec spec) => $"{MemoryCacheService.AssemblyCacheKeyPrefix}a:{spec.AppId}.e:{spec.Edition}.AppCode";
    #endregion

    #region Static Calls for Dependecies - to use before requiring DI
    public (List<AssemblyResult> assemblyResults, string cacheKey) TryGetDependencies(HotBuildSpec spec)
    {
        var cacheKey = KeyDependency(spec);
        return (memoryCacheService.Get<List<AssemblyResult>>(cacheKey), cacheKey);
    }
    private static string KeyDependency(HotBuildSpec spec) => $"{MemoryCacheService.AssemblyCacheKeyPrefix}a:{spec.AppId}.e:{spec.Edition}.d:{DependenciesLoader.DependenciesFolder}";
    #endregion

    #region Static Calls Only - for use before the object is created using DI

    internal static string KeyTemplate(string templateFullPath) => $"{MemoryCacheService.AssemblyCacheKeyPrefix}v:{templateFullPath.ToLowerInvariant()}";

    public AssemblyResult TryGetTemplate(string templateFullPath) => memoryCacheService.Get<AssemblyResult>(KeyTemplate(templateFullPath));

    #endregion


    internal string Add(string cacheKey, object data, IList<string> folderPaths = null, IList<string> filePaths = null) 
        => memoryCacheService.Add(cacheKey, data, folderPaths, filePaths);

    internal string Add(string cacheKey, object data, IDictionary<string, bool> folderPaths = null, IList<string> filePaths = null)
        => memoryCacheService.Add(cacheKey, data, folderPaths, filePaths);
}