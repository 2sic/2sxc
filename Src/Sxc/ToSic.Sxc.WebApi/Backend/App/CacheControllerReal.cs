using ToSic.Sxc.Services.OutputCache;

namespace ToSic.Sxc.Backend.App;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class CacheControllerReal(
    IContextCompleteService ctxService,
    IContextBuilderService ctxBuilder,
    LazySvc<IOutputCacheManagementService> outputCacheManagement)
    : ServiceBase("Sxc.ApiApCac", connect: [ctxService, ctxBuilder, outputCacheManagement])
{
    public const string LogSuffix = "AppCac";

    // 2026-10-20 2dm - previously used ctxService.SetAppFromPathOrGetExistingBlock
    public bool Flush(string appPath, AppCacheFlushSpecs? request)
        => FlushInternal((ctxBuilder.TryFindAppFromPath(appPath) ?? throw new ArgumentException($"App not found for folder '{appPath}'. Can't flush cache.")).AppReaderRequired.AppId, request);

    public bool FlushAuto(int? appId, AppCacheFlushSpecs? request)
        => FlushInternal(
            appId ?? ctxService.BlockContextRequired().AppReaderRequired.AppId,
            request
        );

    private bool FlushInternal(int appId, AppCacheFlushSpecs? specs)
    {
        var l = Log.Fn<bool>($"app:{appId}");

        //if (!context.User.IsContentAdmin)
        //{
        //    const string message = "Request not allowed. Cache flush requires admin permissions.";
        //    throw l.Done(new HttpExceptionAbstraction(HttpStatusCode.Unauthorized, message, "Request not allowed"));
        //}

        var touched = OutputCacheManagement.Flush(appId, dependencies: specs?.Dependencies);
        return touched == 0
            ? l.ReturnTrue("app-wide cache dependency touched")
            : l.ReturnTrue($"{touched} dependencies touched");
    }

    private IOutputCacheManagementService OutputCacheManagement => outputCacheManagement.Value;
}
