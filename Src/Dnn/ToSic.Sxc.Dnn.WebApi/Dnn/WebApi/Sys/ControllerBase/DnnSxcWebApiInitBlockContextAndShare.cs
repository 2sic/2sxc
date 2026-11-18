using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Services.Sys;

namespace ToSic.Sxc.Dnn.WebApi.Sys;

internal class DnnSxcWebApiInitBlockContextAndShare()
    : ServiceWithContext("Dnn.CdHlp")
{
    /// <summary>
    /// This will make sure that any services requiring the context can get it.
    /// It must usually be called from the base class which expects to use this.
    /// </summary>
    public void InitBlockSpecs(DnnWebApiHelper sysHlp, HttpRequestMessage request, ILog log)
    {
        sysHlp = sysHlp ?? throw new InvalidOperationException("System helper is not initialized");

        this.LinkLog(log);

        // Prevent duplicate execution, as both these call this
        // - DnnSxcControllerBase (foundational base class) - directly in Initialize
        // - DnnSxcCustomControllerBase (base class for custom code) - Indirectly, through Initialize below.
        if (_blockContextInitialized)
            return;
        _blockContextInitialized = true;
        var block = sysHlp.GetBlockAndContext(request);
        if (block != null)
            sysHlp.GetService<ISxcCurrentContextService>().ShareBlockSpecs(block);
    }
    private bool _blockContextInitialized;

    public static void InitAppSpecs(DnnWebApiHelper sysHlp, int appId)
    {
        sysHlp.GetService<ISxcCurrentContextService>().ShareAppStateOptimized(appId);
    }
}