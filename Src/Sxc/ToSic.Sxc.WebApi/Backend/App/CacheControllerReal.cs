using ToSic.Sxc.Services.Cache;

namespace ToSic.Sxc.Backend.App;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class CacheControllerReal(
    ISxcCurrentContextService ctxService,
    LazySvc<INamedCacheDependencyService> namedDependencies)
    : ServiceBase("Sxc.ApiApCac", connect: [ctxService, namedDependencies])
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

        var normalized = NamedDependencies.NormalizeNames(request?.Dependencies);
        if (normalized.Count == 0)
        {
            // Empty input means "flush all output-cache entries for this app" by touching the
            // app-wide dependency key, without purging the full app cache.
            NamedDependencies.TouchApp(CacheDependencyScopes.OutputCache, appId);
            return l.ReturnTrue("app-wide cache dependency touched");
        }

        NamedDependencies.Touch(CacheDependencyScopes.OutputCache, appId, normalized);
        return l.ReturnTrue($"{normalized.Count} dependencies touched");
    }

    private INamedCacheDependencyService NamedDependencies => namedDependencies.Value;
}
