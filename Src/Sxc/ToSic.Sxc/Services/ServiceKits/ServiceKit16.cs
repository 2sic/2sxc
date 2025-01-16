using Connect.Koi;
using ToSic.Razor.Blade;
using ToSic.Sxc.Code;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Services;

/// <summary>
/// Default ServiceKit for 2sxc v14.
/// </summary>
/// <remarks>
/// * History: Added v14.04
/// </remarks>
[PublicApi]
[method: PrivateApi("Public constructor for DI")]
public class ServiceKit16() : ServiceKit("Sxc.Kit16")
{
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

    // Removed for v16
    //public new ISystemLogService Log => SystemLog;


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

    /// <inheritdoc cref="ServiceKit14.User"/>
    [PrivateApi("Experimental in v15.03")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public IUserService User => field ??= GetKitService<IUserService>();

    /// <summary>
    /// Key service.
    /// Rarely used, as the RazorTyped has a UniqueKey property which comes from this service.
    /// You only need this service, if you need to create combined keys (like with an entity)
    /// </summary>
    /// <remarks>
    /// * New in v16.04
    /// </remarks>
    public IKeyService Key => field ??= new KeyService();

    /// <summary>
    /// Templates service, which can parse strings containing placeholders.
    /// </summary>
    /// <remarks>
    /// Released in v18.00
    /// </remarks>
    public ITemplateService Template => field ??= GetKitService<ITemplateService>();

    [InternalApi_DoNotUse_MayChangeWithoutNotice("Still Beta in v19.00")]
    public ICacheService Cache => field ??= GetKitService<ICacheService>();
}