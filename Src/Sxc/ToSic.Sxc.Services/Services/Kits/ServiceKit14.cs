using Connect.Koi;
using ToSic.Razor.Blade;

namespace ToSic.Sxc.Services;

/// <summary>
/// Default ServiceKit for 2sxc v14.
/// Provided in Razor and WebApi as `Kit`.
/// </summary>
/// <remarks>
/// * History: Added v14.04
/// </remarks>
[PublicApi]
[method: PrivateApi("Public constructor for DI")]
public class ServiceKit14() : ServiceKit("Sxc.Kit14") // , IServiceKitForTypedData /* probably not needed, since typed data is always newer base classes */
{
    /// <summary>
    /// The ADAM Service, used to retrieve files and maybe more. 
    /// </summary>
    [field: AllowNull, MaybeNull]
    public IAdamService Adam => field ??= GetKitService<IAdamService>();

    // 2025-05-11 2dm commented out, previous was internal / marked obsolete
    ///// <summary>
    ///// The CMS Service - not for use
    ///// </summary>
    //[PrivateApi("Was never public but could be in use")]
    //[Obsolete("This API was never published, do not use.")]
    //[ShowApiWhenReleased(ShowApiMode.Never)]
    //internal ICmsService Cms => field ??= GetKitService<ICmsService>();

    ///// <summary>
    ///// Access for TypedData when using this service kit with the interface.
    ///// New & internal v20.
    ///// </summary>
    //ICmsService IServiceKitForTypedData.Cms => Cms;

    /// <summary>
    /// The Convert Service, used to convert any kind of data type to another data type
    /// </summary>
    [field: AllowNull, MaybeNull]
    public IConvertService Convert => field ??= GetKitService<IConvertService>();

    /// <summary>
    /// The Koi CSS Service, used to detect the current CSS framework and other features.
    /// See [ICss](xref:Connect.Koi.ICss)
    /// </summary>
    [field: AllowNull, MaybeNull]
    public ICss Css => field ??= GetKitService<ICss>();


    /// <summary>
    /// The Data service to get DataSources and similar.
    /// </summary>
    /// <remarks>
    /// * added in v15.06
    /// </remarks>
    [field: AllowNull, MaybeNull]
    public IDataService Data => field ??= GetKitService<IDataService>();

    /// <summary>
    /// The Edit service, same as the main Edit service
    /// </summary>
    // Important: must share the Edit from the _DynCodeRoot for scenarios where Enable was set manually
    [field: AllowNull, MaybeNull]
    public IEditService Edit => field ??= GetKitService<IEditService>();


    /// <summary>
    /// The Features service, used to check if features are enabled
    /// </summary>
    [field: AllowNull, MaybeNull]
    public IFeaturesService Feature => field ??= GetKitService<IFeaturesService>();

    /// <summary>
    /// The Razor Blade 4 HtmlTags service, to fluidly create Tags.
    /// See [](xref:ToSic.Razor.Blade.IHtmlTagsService).
    ///
    /// > [!IMPORTANT]
    /// > This is _similar but different_ to the [Razor.Blade.Tag](https://razor-blade.net/api/ToSic.Razor.Blade.Tag.html).
    /// > The [](xref:ToSic.Razor.Blade.IHtmlTag) objects returned here are _immutable_.
    /// > This means that chained commands like `...HtmlTags.Div().Id(...).Class(...)`
    /// > all return new objects and don't modify the previous one.
    /// >
    /// > The older `Tag` helper created mutable objects where chaining always modified the original and returned it again.
    /// </summary>
    /// <remarks>Added in v15</remarks>
    [field: AllowNull, MaybeNull]
    public IHtmlTagsService HtmlTags => field ??= GetKitService<IHtmlTagsService>();

    /// <summary>
    /// The Images service, used to create `img` and `picture` tags
    /// </summary>
    [field: AllowNull, MaybeNull]
    public IImageService Image => field ??= GetKitService<IImageService>();


    /// <summary>
    /// The JSON service, used to convert data to-and-from JSON
    /// </summary>
    [field: AllowNull, MaybeNull]
    public IJsonService Json => field ??= GetKitService<IJsonService>();


    /// <inheritdoc cref="IDynamicCode.Link" />
    [field: AllowNull, MaybeNull]
    public ILinkService Link => field ??= GetKitService<ILinkService>();

    /// <summary>
    /// The System Log service, used to add log messages to the system (Dnn/Oqtane)
    /// </summary>
    [field: AllowNull, MaybeNull]
    public ISystemLogService SystemLog => field ??= GetKitService<ISystemLogService>();

    /// <summary>
    /// Note that this was used in Mobius / Events in a few releases, so we can't just change it.
    /// If we create a Kit15, this should be removed
    /// </summary>
    [PrivateApi("was the official name before v15.06, probably never used publicly, but should stay in for a while")]
    [ShowApiWhenReleased(ShowApiMode.Never)]
    public new ISystemLogService Log => SystemLog;


    /// <summary>
    /// The Mail service, used to send mails
    /// </summary>
    [field: AllowNull, MaybeNull]
    public IMailService Mail => field ??= GetKitService<IMailService>();


    /// <summary>
    /// The Page service, used to set headers, activate features etc.
    /// </summary>
    [field: AllowNull, MaybeNull]
    public IPageService Page => field ??= GetKitService<IPageService>();


    /// <summary>
    /// The Render service, used to render one or more dynamic content within other content
    /// </summary>
    [field: AllowNull, MaybeNull]
    public IRenderService Render => field ??= GetKitService<IRenderService>();

    /// <summary>
    /// The Secure Data service - mainly for reading / decrypting secrets. 
    /// </summary>
    [field: AllowNull, MaybeNull]
    public ISecureDataService SecureData => field ??= GetKitService<ISecureDataService>();

    /// <summary>
    /// The Razor-Blade Scrub service, used to clean up HTML.
    /// See [](xref:ToSic.Razor.Blade.IScrub)
    /// </summary>
    [field: AllowNull, MaybeNull]
    public IScrub Scrub => field ??= GetKitService<IScrub>();


    /// <summary>
    /// The toolbar service, used to generate advanced toolbars
    /// </summary>
    [field: AllowNull, MaybeNull]
    public IToolbarService Toolbar => field ??= GetKitService<IToolbarService>();
}