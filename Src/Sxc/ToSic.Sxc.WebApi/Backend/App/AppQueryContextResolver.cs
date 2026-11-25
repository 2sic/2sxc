using ToSic.Eav.Context.Sys;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Eav.WebApi.App.Sys;

namespace ToSic.Sxc.Backend.App;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class AppQueryContextResolver(IContextCompleteService ctxService, IContextOfAppFactory appCtxSvc, IWebApiAppContextBuilderService ctxBuilder, LazySvc<ILookUpEngineResolver> lookupResolver)
    : ServiceBase("App.QryRes", connect: [lookupResolver, ctxService, ctxBuilder])
{
    public const string LogSuffix = "AppQry";


    public AppWebApiQueryRunSvc.AppCtxAndLookup GetCtxAndLookupForAppPath(string appPathRequired)
    {
        // 2026-10-20 2dm - previously used ctxService.SetAppFromPathOrGetExistingBlock
        var appCtx = ctxBuilder.TryFindAppFromPath(appPathRequired)
                     ?? throw new ArgumentException($"App not found for folder '{appPathRequired}'. Can't run query.");

        var lookups = lookupResolver.Value.GetLookUpEngine(0);
        return new(appCtx, lookups);
    }

    public AppWebApiQueryRunSvc.AppCtxAndLookup GetCtxAndLookupForAppIdOrContext(int? appId)
    {
        var appCtx = appId != null
            ? appCtxSvc.Create(appId.Value)
            : ctxService.BlockContextRequired();

        // If the appId wasn't specified or == to the Block-AppId, then also include block info to enable more data-sources like CmsBlock
        var maybeBlock = appId == null || appId == appCtx.AppReaderRequired.AppId
            ? ctxService.BlockOrNull
            : null;

        // If no app available from context, check if an app-id was supplied in url
        // Note that it may only be an app from the current portal
        // and security checks will run internally
        var blockLookupOrNull = maybeBlock is { DataIsReady: true }
            ? maybeBlock.Data.Configuration.LookUpEngine
            : null;

        var lookups = blockLookupOrNull ??
                      lookupResolver.Value.GetLookUpEngine((appCtx as IContextOfModule)?.Module.Id ?? -1);
        return new(appCtx, lookups);
    }
}
