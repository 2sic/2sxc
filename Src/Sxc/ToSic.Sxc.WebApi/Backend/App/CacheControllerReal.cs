using System.Net;
using ToSic.Eav.Context;
using ToSic.Sxc.Context.Sys;
using ToSic.Sxc.Web.Sys.LightSpeed;

namespace ToSic.Sxc.Backend.App;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class CacheControllerReal(
    ISxcCurrentContextService ctxService,
    LazySvc<LightSpeedExternalDependencies> externalDependencies)
    : ServiceBase("Sxc.ApiApCac", connect: [ctxService, externalDependencies])
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

        var normalized = ExternalDependencies.NormalizeDependencies(request?.Dependencies);
        if (normalized.Count == 0)
        {
            // Empty input means "flush all LightSpeed entries for this app" by touching the
            // app-wide dependency key, without purging the full app cache.
            ExternalDependencies.TouchApp(appId);
            return l.ReturnTrue("app-wide cache dependency touched");
        }

        ExternalDependencies.Touch(appId, normalized);
        return l.ReturnTrue($"{normalized.Count} dependencies touched");
    }

    private LightSpeedExternalDependencies ExternalDependencies => externalDependencies.Value;
}
