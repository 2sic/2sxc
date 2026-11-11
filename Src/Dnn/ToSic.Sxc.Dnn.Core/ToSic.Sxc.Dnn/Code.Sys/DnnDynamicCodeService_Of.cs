using ToSic.Eav.Apps;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Code.Sys.CodeApi;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sys.Wrappers;

namespace ToSic.Sxc.Dnn.Code.Sys;

partial class DnnDynamicCodeService
{
    /// <inheritdoc />
    public IDynamicCode12 OfApp(int appId) => GetApi12(appId: appId);

    /// <inheritdoc />
    public IDynamicCode12 OfApp(int zoneId, int appId) => GetApi12(zoneId: zoneId, appId: appId);

    /// <inheritdoc />
    public IDynamicCode12 OfApp(IAppIdentity appIdentity) => GetApi12(zoneId: appIdentity.ZoneId, appId: appIdentity.AppId);

    /// <inheritdoc />
    public IDynamicCode12 OfModule(int pageId, int moduleId)
    {
        var l = Log.Fn<IDynamicCode12>($"{pageId}, {moduleId}");
        MakeSureLogIsInHistory();
        // #ActivateEditUi seems unused, it's never read; keep till ca. mid 2027
        //ActivateEditUi();
        var cmsBlock = ServicesScoped.ModAndBlockBuilder.Value.BuildBlock(pageId, moduleId);
        var exCtx = ServicesScoped.ExCtxGenerator.New().New(new()
        {
            OwnerOrNull = null,
            BlockOrNull = cmsBlock,
            ParentLog = Log,
            CompatibilityFallback = CompatibilityLevels.CompatibilityLevel12,
        });

        var code12 = new DynamicApiStandalone(exCtx, exCtx.GetDynamicApi());
        return l.ReturnAsOk(code12);
    }

    /// <inheritdoc />
    public IDynamicCode12 OfSite() => OfApp(GetPrimaryAppIdentity(null));

    /// <inheritdoc />
    public IDynamicCode12 OfSite(int siteId) => OfApp(GetPrimaryAppIdentity(siteId));

    private IDynamicCode12 GetApi12(int? zoneId = null, int? appId = null)
    {
        var l = Log.Fn<IDynamicCode12>();
        MakeSureLogIsInHistory();
        // #ActivateEditUi seems unused, it's never read; keep till ca. mid 2027
        //ActivateEditUi();
        var exCtx = ServicesScoped.ExCtxGenerator.New().New(new()
        {
            OwnerOrNull = null,
            BlockOrNull = null,
            ParentLog = Log,
            CompatibilityFallback = CompatibilityLevels.CompatibilityLevel12,
        });
        var app = GetAndInitApp(ToAppIdentityOrThrow(zoneId, appId), overrideSite: null);
        ((IExCtxAttachApp)exCtx).AttachApp(((IWrapper<IAppForCode>)app).GetContents());
        var code12 = new DynamicApiStandalone(exCtx, exCtx.GetDynamicApi());
        return l.ReturnAsOk(code12);
    }
}