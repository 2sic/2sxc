using Connect.Koi;
using ToSic.Razor.Blade;
using ToSic.Sxc.Services.Sys;
using ToSic.Sxc.Services.Sys.Cms;

namespace ToSic.Sxc.Services;

/// <summary>
/// Default ServiceKit for 2sxc v16.
/// Provided in Razor and WebApi as `Kit`.
/// </summary>
/// <remarks>
/// This is the service kit you get on `Hybrid.RazorTyped`, `AppCode.Razor.AppRazor` or `Hybrid.ApiTyped`.
/// 
/// History
/// * New in v16 for Typed Razor / WebApi
/// * Added Key service in v16.04
/// * Added Template service in v18.00
/// * Added Cache service in v19.00
/// * Added User service in v19.02
/// </remarks>
[PublicApi]
[method: PrivateApi("Public constructor for DI")]
public class ServiceKit16() : ServiceKit("Sxc.Kit16")
{
    #region Same as v14

    /// <inheritdoc cref="ServiceKit14.Adam"/>
    [field: AllowNull, MaybeNull]
    public IAdamService Adam => field ??= GetKitService<IAdamService>();

    /// <summary>
    /// The CMS Service - WIP
    /// </summary>
    [PrivateApi("Not yet for public use, as API is not yet public")]
    [field: AllowNull, MaybeNull]
    private ICmsService Cms => field ??= GetKitService<ICmsService>();

    /// <inheritdoc cref="ServiceKit14.Convert"/>
    [field: AllowNull, MaybeNull]
    public IConvertService16 Convert => field ??= GetKitService<IConvertService16>();

    /// <inheritdoc cref="ServiceKit14.Css"/>
    [field: AllowNull, MaybeNull]
    public ICss Css => field ??= GetKitService<ICss>();


    /// <inheritdoc cref="ServiceKit14.Data"/>
    [field: AllowNull, MaybeNull]
    public IDataService Data => field ??= GetKitService<IDataService>();

    /// <inheritdoc cref="ServiceKit14.Edit"/>
    [field: AllowNull, MaybeNull]
    public IEditService Edit => field ??= GetKitService<IEditService>();

    /// <inheritdoc cref="ServiceKit14.Feature"/>
    [field: AllowNull, MaybeNull]
    public IFeaturesService Feature => field ??= GetKitService<IFeaturesService>();

    /// <inheritdoc cref="ServiceKit14.HtmlTags"/>
    [field: AllowNull, MaybeNull]
    public IHtmlTagsService HtmlTags => field ??= GetKitService<IHtmlTagsService>();

    /// <inheritdoc cref="ServiceKit14.Image"/>
    [field: AllowNull, MaybeNull]
    public IImageService Image => field ??= GetKitService<IImageService>();

    /// <inheritdoc cref="ServiceKit14.Json"/>
    [field: AllowNull, MaybeNull]
    public IJsonService Json => field ??= GetKitService<IJsonService>();

    /// <inheritdoc cref="IDynamicCodeDocs.Link" />
    [field: AllowNull, MaybeNull]
    public ILinkService Link => field ??= GetKitService<ILinkService>();

    /// <inheritdoc cref="ServiceKit14.SystemLog"/>
    [field: AllowNull, MaybeNull]
    public ISystemLogService SystemLog => field ??= GetKitService<ISystemLogService>();

    /// <inheritdoc cref="ServiceKit14.Mail"/>
    [field: AllowNull, MaybeNull]
    public IMailService Mail => field ??= GetKitService<IMailService>();

    /// <inheritdoc cref="ServiceKit14.Page"/>
    [field: AllowNull, MaybeNull]
    public IPageService Page => field ??= GetKitService<IPageService>();

    /// <inheritdoc cref="ServiceKit14.Render"/>
    [field: AllowNull, MaybeNull]
    public IRenderService Render => field ??= GetKitService<IRenderService>();

    /// <inheritdoc cref="ServiceKit14.SecureData"/>
    [field: AllowNull, MaybeNull]
    public ISecureDataService SecureData => field ??= GetKitService<ISecureDataService>();

    /// <inheritdoc cref="ServiceKit14.Scrub"/>
    [field: AllowNull, MaybeNull]
    public IScrub Scrub => field ??= GetKitService<IScrub>();


    /// <inheritdoc cref="ServiceKit14.Toolbar"/>
    [field: AllowNull, MaybeNull]
    public IToolbarService Toolbar => field ??= GetKitService<IToolbarService>();

    #endregion

    #region Existed in v14 but Removed in v16

    // Removed for v16
    //public new ISystemLogService Log => SystemLog;

    #endregion

    #region Added to v16 only

    /// <summary>
    /// The User service, used to get user and role information.
    /// </summary>
    /// <remarks>
    /// History: released in 19.02 (started in v15.03 but was never public)
    /// </remarks>
    [field: AllowNull, MaybeNull]
    public IUserService User => field ??= GetKitService<IUserService>();

    /// <summary>
    /// Key service.
    /// Rarely used, as the RazorTyped has a UniqueKey property which comes from this service.
    /// You only need this service, if you need to create combined keys (like with an entity)
    /// </summary>
    /// <remarks>
    /// * New in v16.04
    /// </remarks>
    [field: AllowNull, MaybeNull]
    public IKeyService Key => field ??= GetKitService<IKeyService>(); // new KeyService();

    /// <summary>
    /// Templates service, which can parse strings containing placeholders.
    /// </summary>
    /// <remarks>
    /// History: introduced in v18.00
    /// </remarks>
    [field: AllowNull, MaybeNull]
    public ITemplateService Template => field ??= GetKitService<ITemplateService>();

    /// <summary>
    /// Cache service, used to cache data.
    /// </summary>
    /// <remarks>
    /// Used to cache data, specifically to ensure it is refreshed when certain events happen,
    /// such as data in the App changes.
    ///
    /// History: introduced in v19.00
    /// </remarks>
    [field: AllowNull, MaybeNull]
    public ICacheService Cache => field ??= GetKitService<ICacheService>();

    #endregion
}