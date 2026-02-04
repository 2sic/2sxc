using ToSic.Eav.Apps.AppReader.Sys;
using ToSic.Eav.Apps.Sys.Paths;
using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.Sys;
using ToSic.Eav.WebApi.Sys.Cms;
using ToSic.Eav.WebApi.Sys.Languages;
using ToSic.Sxc.Apps.Sys.Assets;
using ToSic.Sxc.Apps.Sys.Paths;
using ToSic.Sxc.Services;
using ToSic.Sys.Capabilities.Features;
using static ToSic.Sys.Capabilities.Features.BuiltInFeatures;

namespace ToSic.Sxc.Backend.Context;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class UiContextBuilderBase(UiContextBuilderBase.Dependencies services)
    : ServiceBase<UiContextBuilderBase.Dependencies>(services, SxcLogName + ".UiCtx"), IUiContextBuilder
{
    #region Dependencies 

    public record Dependencies(
        IContextOfSite SiteCtx,
        LazySvc<ISysFeaturesService> Features,
        LazySvc<IUiData> UiDataLazy,
        LazySvc<LanguagesBackend> LanguagesBackend,
        IAppPathsMicroSvc AppPaths,
        LazySvc<GlobalPaths> GlobalPaths,
        IAppsCatalog AppsCatalog,
        LazySvc<IUserService> UsersSvc
    ) : DependenciesRecord(connect: [SiteCtx, Features, UiDataLazy, AppPaths, LanguagesBackend, GlobalPaths, AppsCatalog, UsersSvc]);

    #endregion

    #region Constructor / DI

    protected int ZoneId => Services.SiteCtx.Site.ZoneId;
    protected IAppSpecs? AppSpecsOrNull;
    private IAppReader? _appReaderOrNull;

    #endregion

    public IUiContextBuilder InitApp(IAppReader? appReaderOrNull)
    {
        AppSpecsOrNull = appReaderOrNull?.Specs;
        _appReaderOrNull = appReaderOrNull;
        return this;
    }

    /// <inheritdoc />
    public ContextDto Get(Ctx flags, CtxEnable enableFlags)
    {
        var ctx = new ContextDto();
        // logic for activating each part
        // 1. either that switch is on
        // 2. or the null-check: all is on
        // 3. This also means if the switch is off, it's off
        if (flags.HasFlag(Ctx.AppBasic) | flags.HasFlag(Ctx.AppAdvanced))
            ctx.App = GetApp(flags);
        if (enableFlags != CtxEnable.None) ctx.Enable = GetEnable(enableFlags);
        if (flags.HasFlag(Ctx.Language)) ctx.Language = GetLanguage();
        if (flags.HasFlag(Ctx.Page)) ctx.Page = GetPage();
        if (flags.HasFlag(Ctx.Site)) ctx.Site = GetSite(flags);
        if (flags.HasFlag(Ctx.System)) ctx.System = GetSystem(flags);
        if (flags.HasFlag(Ctx.User)) ctx.User = GetUser(flags);
        if (flags.HasFlag(Ctx.Features) || flags.HasFlag(Ctx.FeaturesForSystemTypes))
            ctx.Features = GetFeatures(flags);
        return ctx;
    }

    protected virtual ContextLanguageDto? GetLanguage()
    {
        if (ZoneId == 0)
            return null;
        var site = Services.SiteCtx.Site;

        var converted = Services.LanguagesBackend.Value.GetLanguagesOfApp(_appReaderOrNull);

        return new()
        {
            Current = site.CurrentCultureCode,
            Primary = converted.Any() ? site.DefaultCultureCode : site.CurrentCultureCode, // in special case when no languages are available, use the current culture to fix translation issue in UI
            List = converted, 
        };
    }

    protected virtual ContextResourceWithApp GetSystem(Ctx flags)
    {
        var result = new ContextResourceWithApp
        {
            Url = "/"
        };
        // Stop now if we don't need advanced infos
        if (!flags.HasFlag(Ctx.AppAdvanced))
            return result;

        // Otherwise also add the global app id
        result.PrimaryApp = result.DefaultApp = new(1, 1);
        return result;
    }

    protected virtual ContextResourceWithApp GetSite(Ctx flags)
    {
        var result = new ContextResourceWithApp
        {
            Id = Services.SiteCtx.Site.Id,
            Url = "//" + Services.SiteCtx.Site.UrlRoot + "/",
        };
        // Stop now if we don't need advanced infos
        if (!flags.HasFlag(Ctx.AppAdvanced)) return result;

        // Otherwise also add the global appId
        var zoneId = Services.SiteCtx.Site.ZoneId;
        result.DefaultApp = (AppIdentity)Services.AppsCatalog.DefaultAppIdentity(zoneId);
        result.PrimaryApp = (AppIdentity)Services.AppsCatalog.PrimaryAppIdentity(zoneId);
        return result;
    }

    protected virtual WebResourceDto GetPage() =>
        new()
        {
            Id = EavConstants.NullId,
        };

    protected virtual ContextEnableDto GetEnable(CtxEnable ctx)
    {
        var isRealApp = AppSpecsOrNull != null && ! AppSpecsOrNull.IsContentApp();
        var user = Services.SiteCtx.User;
        var dto = new ContextEnableDto
        {
            DebugMode = user.IsSystemAdmin ||
                        Services.Features.Value.IsEnabled(EditUiAllowDebugModeForEditors)
        };
        if (ctx.HasFlag(CtxEnable.AppPermissions))
            dto.AppPermissions = isRealApp;
        if (ctx.HasFlag(CtxEnable.CodeEditor))
            dto.CodeEditor = user.IsSystemAdmin;
        if (ctx.HasFlag(CtxEnable.Query))
            dto.Query = isRealApp && user.IsSystemAdmin;
        if (ctx.HasFlag(CtxEnable.FormulaSave))
            dto.FormulaSave = user.IsSystemAdmin;
        if (ctx.HasFlag(CtxEnable.OverrideEditRestrictions))
            dto.OverrideEditRestrictions = user.IsSystemAdmin;
        return dto;
    }

    protected virtual string GetGettingStartedUrl() => EavConstants.UrlNotInitialized;

    protected virtual ContextAppDto? GetApp(Ctx flags)
    {
        if (_appReaderOrNull == null)
            return null;
        var appReader = _appReaderOrNull;
        var appSpecs = appReader.Specs;
        var paths = Services.AppPaths.Get(appReader, Services.SiteCtx.Site);
        var result = new ContextAppDto
        {
            Id = appSpecs.AppId,
            Name = appSpecs.Name,
            Folder = appSpecs.Folder,
            Url = paths?.Path,
            SharedUrl = paths?.PathShared
        };

        // Stop now if we don't need edit or advanced
        if (!flags.HasFlag(Ctx.AppEdit) && !flags.HasFlag(Ctx.AppAdvanced))
            return result;

        result.IsGlobalApp = appSpecs.IsGlobalSettingsApp();
        result.IsSiteApp = appSpecs.IsSiteSettingsApp();
        // only check content if not global, as that has the same id
        if (!result.IsGlobalApp)
            result.IsContentApp = appSpecs.IsContentApp();

        // Stop now if we don't need advanced infos
        if (!flags.HasFlag(Ctx.AppAdvanced))
            return result;

        result.GettingStartedUrl = GetGettingStartedUrl();
        result.Identifier = appSpecs.NameId;
            
        // #SiteApp v13
        result.SettingsScope = appSpecs.IsGlobalSettingsApp()
            ? "Global" 
            : appSpecs.IsSiteSettingsApp()
                ? "Site" 
                : "App";

        result.Permissions = new() { Count = appReader.Specs.Metadata.Permissions.Count() };

        result.IsShared = appReader.IsShared();
        result.IsInherited = appReader.IsInherited();

        result.Icon = AppAssetThumbnail.GetUrl(appReader, paths!, Services.GlobalPaths);
        return result;
    }

    protected virtual ContextUserDto GetUser(Ctx flags)
    {
        var user = Services.SiteCtx.User;
        var userDto = new ContextUserDto
        {
            Email = user.Email,
            Id = user.Id,
            Guid = user.Guid,
            IsSystemAdmin = user.IsSystemAdmin,
            IsAnonymous = user.IsAnonymous,
            IsSiteAdmin = user.IsSiteAdmin,
            IsContentEditor = user.IsContentEditor,
            IsContentAdmin = user.IsContentAdmin,
            Name = user.Name,
            Username = user.Username,
            Roles = flags.HasFlag(Ctx.UserRoles) ? Services.UsersSvc.Value.GetCurrentUser().Roles.Select(r => r.Name).ToListOpt() : [],
        };
        return userDto;
    }

    /// <summary>
    /// Features provided must be virtual, so that applications can override this.
    /// </summary>
    /// <returns></returns>
    protected virtual IList<FeatureDto> GetFeatures(Ctx flags)
        => Services.UiDataLazy.Value.FeaturesDto(Services.SiteCtx.Permissions.IsContentAdmin, flags.HasFlag(Ctx.FeaturesForSystemTypes));
}