using ToSic.Sxc.Services.OutputCache;

namespace ToSic.Sxc.Backend.App;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class CacheControllerReal(
    ISxcCurrentContextService ctxService,
    LazySvc<IOutputCacheManagementService> outputCacheManagement)
    : ServiceBase("Sxc.ApiApCac", connect: [ctxService, outputCacheManagement])
{
    public const string LogSuffix = "AppCac";

    public bool Flush(string appPath, AppCacheFlushRequest? request)
        => FlushInternal(ctxService.SetAppOrGetBlock(appPath), request);

    public bool FlushAuto(AppCacheFlushRequest? request, int? appId = null)
        => FlushInternal(
            appId != null
                ? ctxService.GetExistingAppOrSet(appId.Value)
                : ctxService.BlockContextRequired(),
            request
        );

    private bool FlushInternal(IContextOfApp context, AppCacheFlushRequest? request)
    {
        var l = Log.Fn<bool>($"app:{context.AppReaderRequired.AppId}");
        var appId = context.AppReaderRequired.AppId;

        //if (!context.User.IsContentAdmin)
        //{
        //    const string message = "Request not allowed. Cache flush requires admin permissions.";
        //    throw l.Done(new HttpExceptionAbstraction(HttpStatusCode.Unauthorized, message, "Request not allowed"));
        //}

        var touched = OutputCacheManagement.Flush(appId, request?.Dependencies);
        return touched == 0
            ? l.ReturnTrue("app-wide cache dependency touched")
            : l.ReturnTrue($"{touched} dependencies touched");
    }

    private IOutputCacheManagementService OutputCacheManagement => outputCacheManagement.Value;
}
