using Connect.Koi;
using ToSic.Razor.Blade;
using ToSic.Sxc.Code;
using ToSic.Sxc.Services.Internal;

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
    public IAdamService Adam => field ??= GetKitService<IAdamService>();

    /// <summary>
    /// The CMS Service - WIP
    /// </summary>
    [PrivateApi("Not yet for public use, as API is not yet public")]
    internal ICmsService Cms => field ??= GetKitService<ICmsService>();

    /// <inheritdoc cref="ServiceKit14.Convert"/>
    public IConvertService16 Convert => field ??= GetKitService<IConvertService16>();

    /// <inheritdoc cref="ServiceKit14.Css"/>
    public ICss Css => field ??= GetKitService<ICss>();


    /// <inheritdoc cref="ServiceKit14.Data"/>
    public IDataService Data => field ??= GetKitService<IDataService>();

    /// <inheritdoc cref="ServiceKit14.Edit"/>
    public IEditService Edit => field ??= GetKitService<IEditService>();

    /// <inheritdoc cref="ServiceKit14.Feature"/>
    public IFeaturesService Feature => field ??= GetKitService<IFeaturesService>();

    /// <inheritdoc cref="ServiceKit14.HtmlTags"/>
    public IHtmlTagsService HtmlTags => field ??= GetKitService<IHtmlTagsService>();

    /// <inheritdoc cref="ServiceKit14.Image"/>
    public IImageService Image => field ??= GetKitService<IImageService>();

    /// <inheritdoc cref="ServiceKit14.Json"/>
    public IJsonService Json => field ??= GetKitService<IJsonService>();

    /// <inheritdoc cref="IDynamicCode.Link" />
    public ILinkService Link => field ??= GetKitService<ILinkService>();

    /// <inheritdoc cref="ServiceKit14.SystemLog"/>
    public ISystemLogService SystemLog => field ??= GetKitService<ISystemLogService>();

    /// <inheritdoc cref="ServiceKit14.Mail"/>
    public IMailService Mail => field ??= GetKitService<IMailService>();

    /// <inheritdoc cref="ServiceKit14.Page"/>
    public IPageService Page => field ??= GetKitService<IPageService>();

    /// <inheritdoc cref="ServiceKit14.Render"/>
    public IRenderService Render => field ??= GetKitService<IRenderService>();

    /// <inheritdoc cref="ServiceKit14.SecureData"/>
    public ISecureDataService SecureData => field ??= GetKitService<ISecureDataService>();

    /// <inheritdoc cref="ServiceKit14.Scrub"/>
    public IScrub Scrub => field ??= GetKitService<IScrub>();


    /// <inheritdoc cref="ServiceKit14.Toolbar"/>
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
    public IUserService User => field ??= GetKitService<IUserService>();

    /// <summary>
    /// Key service.
    /// Rarely used, as the RazorTyped has a UniqueKey property which comes from this service.
    /// You only need this service, if you need to create combined keys (like with an entity)
    /// </summary>
    /// <remarks>
    /// * New in v16.04
    /// </remarks>
    public IKeyService Key => field ??= GetKitService<IKeyService>(); // new KeyService();

    /// <summary>
    /// Templates service, which can parse strings containing placeholders.
    /// </summary>
    /// <remarks>
    /// History: introduced in v18.00
    /// </remarks>
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
    public ICacheService Cache => field ??= GetKitService<ICacheService>();

    #endregion
}