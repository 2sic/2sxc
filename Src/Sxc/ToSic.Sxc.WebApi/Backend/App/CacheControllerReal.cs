using ToSic.Sxc.Services.OutputCache;

namespace ToSic.Sxc.Backend.App;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class CacheControllerReal(
    ISxcCurrentContextService ctxService,
    LazySvc<IOutputCacheManagementService> outputCacheManagement)
    : ServiceBase("Sxc.ApiApCac", connect: [ctxService, outputCacheManagement])
{
    public const string LogSuffix = "AppCac";

    public bool Flush(string appPath, AppCacheFlushSpecs? request)
        => FlushInternal(ctxService.SetAppOrGetBlock(appPath).AppReaderRequired.AppId, request);

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
