using Microsoft.Extensions.DependencyInjection;
using ToSic.Eav.Apps;
using ToSic.Eav.Context;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Code;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Code.Sys.CodeApi;
using ToSic.Sxc.Services.Sys.CodeApiServiceHelpers;
using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Services.Sys.TypedApiService;

/// <summary>
/// WIP - goal is to have a DI factory which creates DynamicCode objects for use in Skins and other external controls
/// Not sure how to get this to work, since normally we always start with a code-file, and here we don't have one!
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class TypedApiService(CodeApiServiceBase.Dependencies services, string? logName = null /* must be nullable for DI */)
    : CodeApiServiceBase(services, logName ?? $"{SxcLogName}.TypeCS"),
        ITypedApiService
{
    /// <summary>
    /// This is for all the services used here, or also for services needed in inherited classes which will need the same scoped objects.
    /// It's important to understand that everything in here will use the scoped service provider.
    /// </summary>
    [field: AllowNull, MaybeNull]
    protected IServiceProvider ScopedServiceProvider => field
        ??= Services.ServiceProvider.CreateScope().ServiceProvider;

    [field: AllowNull, MaybeNull]
    private ScopedDependencies ServicesScoped => field
        ??= ScopedServiceProvider.Build<ScopedDependencies>().ConnectServices(Log);


    protected void ActivateEditUi() => EditUiRequired = true;

    protected bool EditUiRequired;

    #region App

    public IAppTyped App(NoParamOrder npo = default, int? zoneId = null, int? appId = null, ISite? site = null, bool? withUnpublished = null)
    {
        var l = Log.Fn<IAppTyped>();
        MakeSureLogIsInHistory();

        var app = GetApp(ServicesScoped.AppGenerator, zoneId: zoneId, appId: appId, site: site, withUnpublished: withUnpublished);
        return l.ReturnAsOk(GetNewCodeRoot(app).GetState<IAppTyped>());
    }

    /// <inheritdoc />
    public IAppTyped AppOfSite()
    {
        var l = Log.Fn<IAppTyped>();

        MakeSureLogIsInHistory();
        var app = GetAndInitApp(ServicesScoped.AppGenerator.New(), GetPrimaryAppIdentity(null), null);
        return l.ReturnAsOk(GetNewCodeRoot(app).GetState<IAppTyped>());
    }

    #endregion


    #region Of App / Site / Module etc.

    /// <inheritdoc />
    public ITypedApi ApiOfApp(int appId) => OfAppOrSiteInternal(appId: appId);

    /// <inheritdoc />
    public ITypedApi ApiOfApp(int zoneId, int appId) => OfAppOrSiteInternal(zoneId: zoneId, appId: appId);

    /// <inheritdoc />
    public ITypedApi ApiOfModule(int pageId, int moduleId)
    {
        var l = Log.Fn<ITypedApi>($"{pageId}, {moduleId}");
        MakeSureLogIsInHistory();
        ActivateEditUi();
        var cmsBlock = ServicesScoped.ModAndBlockBuilder.Value.BuildBlock(pageId, moduleId);
        var codeRoot = ServicesScoped.ExCtxGenerator.New().New(new()
        {
            OwnerOrNull = null,
            BlockOrNull = cmsBlock,
            ParentLog = Log,
            CompatibilityFallback = CompatibilityLevels.CompatibilityLevel16,
        });

        var code12 = new TypedApiStandalone(codeRoot, codeRoot.GetTypedApi());
        return l.ReturnAsOk(code12);
    }

    /// <inheritdoc />
    public ITypedApi ApiOfSite() =>
        ApiOfAppOrSite(GetPrimaryAppIdentity(null));

    /// <inheritdoc />
    public ITypedApi ApiOfSite(int siteId) =>
        ApiOfAppOrSite(GetPrimaryAppIdentity(siteId));

    private ITypedApi ApiOfAppOrSite(IAppIdentity appIdentity) =>
        OfAppOrSiteInternal(zoneId: appIdentity.ZoneId, appId: appIdentity.AppId);


    private ITypedApi OfAppOrSiteInternal(int? zoneId = null, int? appId = null)
    {
        var l = Log.Fn<ITypedApi>();
        MakeSureLogIsInHistory();
        ActivateEditUi();
        var app = GetApp(ServicesScoped.AppGenerator, zoneId: zoneId, appId: appId);
        var exCtx = GetNewCodeRoot(app);
        var code12 = new TypedApiStandalone(exCtx, exCtx.GetTypedApi());
        return l.ReturnAsOk(code12);
    }

    private IExecutionContext GetNewCodeRoot(IApp? appToAttach = default)
    {
        var exCtx = ServicesScoped.ExCtxGenerator.New().New(new()
        {
            OwnerOrNull = null,
            BlockOrNull = null,
            ParentLog = Log,
            CompatibilityFallback = CompatibilityLevels.CompatibilityLevel16,
        });
        if (appToAttach != null)
            ((IExCtxAttachApp)exCtx).AttachApp(appToAttach);
        return exCtx;
    }

    #endregion

}