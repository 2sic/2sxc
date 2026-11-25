// #OutputCacheControllerSeemsUnused2026-11-21 Probably delete ca. 2027-Q1

//using ToSic.Eav.Context.Sys;
//using ToSic.Sxc.Services.OutputCache;

//namespace ToSic.Sxc.Backend.OutputCache;

//[ShowApiWhenReleased(ShowApiMode.Never)]
//public class CacheControllerReal(
//    IContextOfAppService ctxService,
//    IWebApiAppContextBuilderService ctxBuilder,
//    LazySvc<IOutputCacheManagementService> outputCacheManagement)
//    : ServiceBase("Sxc.ApiApCac", connect: [ctxService, ctxBuilder, outputCacheManagement])
//{
//    public const string LogSuffix = "AppCac";

//    // 2026-10-20 2dm - previously used ctxService.SetAppFromPathOrGetExistingBlock
//    public bool Flush(string appPath, AppCacheFlushSpecs? specs)
//    {
//        var appCtx = ctxBuilder.TryFindAppFromPath(appPath)
//                     ?? throw new ArgumentException($"App not found for folder '{appPath}'. Can't flush cache.");
//        return FlushInternal(appCtx.AppReaderRequired.AppId, specs);
//    }

//    public bool FlushAuto(int? appId, AppCacheFlushSpecs? specs)
//        => FlushInternal(
//            appId ?? ctxService.AppContextRequired().AppReaderRequired.AppId,
//            specs
//        );

//    private bool FlushInternal(int appId, AppCacheFlushSpecs? specs)
//    {
//        var l = Log.Fn<bool>($"app:{appId}");

//        //if (!context.User.IsContentAdmin)
//        //{
//        //    const string message = "Request not allowed. Cache flush requires admin permissions.";
//        //    throw l.Done(new HttpExceptionAbstraction(HttpStatusCode.Unauthorized, message, "Request not allowed"));
//        //}

//        var touched = outputCacheManagement.Value.Flush(appId, dependencies: specs?.Dependencies);
//        return touched == 0
//            ? l.ReturnTrue("app-wide cache dependency touched")
//            : l.ReturnTrue($"{touched} dependencies touched");
//    }
//}
