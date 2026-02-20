using ToSic.Eav.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Code.Sys.CodeApiService;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Services.Sys.DynamicCodeService;

partial class DynamicCodeService
{
    /// <inheritdoc />
    public IDynamicCode12 OfApp(int appId) => OfAppInternal(appId: appId);

    /// <inheritdoc />
    public IDynamicCode12 OfApp(int zoneId, int appId) => OfAppInternal(zoneId: zoneId, appId: appId);

    /// <inheritdoc />
    public IDynamicCode12 OfApp(IAppIdentity appIdentity) => OfAppInternal(zoneId: appIdentity.ZoneId, appId: appIdentity.AppId);

    /// <inheritdoc />
    public IDynamicCode12 OfModule(int pageId, int moduleId)
    {
        var l = Log.Fn<IDynamicCode12>($"{pageId}, {moduleId}");
        MakeSureLogIsInHistory();
        ActivateEditUi();
        var cmsBlock = ServicesScoped.ModAndBlockBuilder.Value.BuildBlock(pageId, moduleId);
        var exCtx = ServicesScoped.ExCtxGenerator.New().New(new()
        {
            OwnerOrNull = null,
            BlockOrNull = cmsBlock,
            ParentLog = Log,
            CompatibilityFallback = CompatibilityLevels.CompatibilityLevel12,
        });

        var code12 = new DynamicCodeStandalone(exCtx, ((ExecutionContext)exCtx).DynamicApi);
        return l.ReturnAsOk(code12);
    }

    /// <inheritdoc />
    public IDynamicCode12 OfSite() => OfApp(GetPrimaryAppIdentity(null));

    /// <inheritdoc />
    public IDynamicCode12 OfSite(int siteId) => OfApp(GetPrimaryAppIdentity(siteId));

    private IDynamicCode12 OfAppInternal(int? zoneId = null, int? appId = null)
    {
        var l = Log.Fn<IDynamicCode12>();
        MakeSureLogIsInHistory();
        ActivateEditUi();
        var exCtx = ServicesScoped.ExCtxGenerator.New().New(new()
        {
            OwnerOrNull = null,
            BlockOrNull = null,
            ParentLog = Log,
            CompatibilityFallback = CompatibilityLevels.CompatibilityLevel12,
        });
        var app = App(zoneId: zoneId, appId: appId);
        ((IExCtxAttachApp)exCtx).AttachApp(app);
        var code12 = new DynamicCodeStandalone(exCtx, ((ExecutionContext)exCtx).DynamicApi);
        return l.ReturnAsOk(code12);
    }
}