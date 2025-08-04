using ToSic.Eav.Context;
using ToSic.Eav.Context.Sys.ZoneCulture;
using ToSic.Eav.Context.Sys.ZoneMapper;
using ToSic.Eav.Data.Build;
using ToSic.Sxc.Adam.Sys.Manager;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Code.Sys;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Sys.CmsContext;
using ToSic.Sxc.Data.Sys.DynamicJacket;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Data.Sys.Wrappers;
using ToSic.Sxc.Services.Sys;
using ToSic.Sys.Code.InfoSystem;
using ToSic.Sys.Users;

namespace ToSic.Sxc.Data.Sys.CodeDataFactory;

// todo: make internal once we have an interface
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public partial class CodeDataFactory(
    IServiceProvider serviceProvider,
    LazySvc<CodeDataServices> codeDataServices,
    LazySvc<AdamManager> adamManager,
    LazySvc<IContextOfApp> contextOfAppLazy,
    LazySvc<DataBuilder> dataBuilderLazy,
    LazySvc<ICodeDataPoCoWrapperService> codeDataWrapper,
    Generator<CodeJsonWrapper> wrapJsonGenerator,
    LazySvc<CodeInfoService> codeInfoSvc,
    LazySvc<IZoneMapper> zoneMapper)
    : ServiceWithContext("Sxc.AsConv",
        connect: [/* never: serviceProvider */codeDataServices, adamManager, contextOfAppLazy, dataBuilderLazy, codeDataWrapper, wrapJsonGenerator, codeInfoSvc, zoneMapper]),
        ICodeDataFactory
{
    public CodeInfoService CodeInfo => codeInfoSvc.Value;

    public void SetFallbacks(ISite site, int? compatibility = default, object? adamManagerPrepared = default)
    {
        _siteOrNull = site;
        _compatibilityLevel = compatibility ?? _compatibilityLevel;

        // Handle Adam Manager - passed in as an object so that the Type doesn't have to exist at top level definition
        // but if we get it, we must really make sure it's the correct type
        if (adamManagerPrepared is null)
            return;
        if (adamManagerPrepared is AdamManager adamManagerTyped)
            AdamManager = adamManagerTyped;
        else
            throw new($"The {nameof(adamManager)} must be of type {nameof(AdamManager)}");
    }
    private ISite? _siteOrNull;

    [field: AllowNull, MaybeNull]
    private ISite SiteFromContextOrFallback => field 
        ??= (ExCtxOrNull?.GetState<ICmsContext>() as CmsContext)?.CtxSite.Site
            ?? _siteOrNull
            ?? throw new("Tried getting site from context or fallback, neither returned anything useful. ");

    #region Compatibility Level

    public void SetCompatibilityLevel(int compatibilityLevel)
        => _compatibilityLevelOverride = compatibilityLevel;

    public int CompatibilityLevel => _compatibilityLevelOverride ?? _compatibilityLevel;
    private int? _compatibilityLevelOverride;
    private int _compatibilityLevel = CompatibilityLevels.CompatibilityLevel10;

    #endregion

    #region CodeDataServices

    public CodeDataServices Services => _services.Get(() => 
    {
        var cds = codeDataServices.Value;
        // if the render service is ever needed, it should be connected to the root
        //cds.RenderServiceGenerator.SetInit(nowRs => (nowRs as INeedsCodeApiService)?.ConnectToRoot(_CodeApiSvc));
        return cds;
    })!;
    private readonly GetOnce<CodeDataServices> _services = new();

    /// <summary>
    /// List of dimensions for value lookup, incl. priorities etc. and null-trailing.
    /// lower case safe guaranteed. 
    /// </summary>
    // If we don't have a DynCodeRoot, try to generate the language codes and compatibility
    // There are cases where these were supplied using SetFallbacks, but in some cases none of this is known
    [field: AllowNull, MaybeNull]
    public string?[] Dimensions => field ??=
        // note: can't use SiteFromContextOrFallback.SafeLanguagePriorityCodes() because it will error during testing
        (ExCtxOrNull?.GetState<ICmsContext>() as CmsContext)?.CtxSite.Site.SafeLanguagePriorityCodes()
        ?? _siteOrNull.SafeLanguagePriorityCodes();


    public IBlock? BlockOrNull => ExCtxOrNull?.GetState<IBlock>();

    #endregion

    public object? Json2Jacket(string? json, string? fallback = default)
        => wrapJsonGenerator.New().Setup(WrapperSettings.Dyn(true, true))
            .Json2Jacket(json, fallback: fallback);

    /// <summary>
    /// WIP, we need this in the GetAndConvertHelper, and want to make sure it's not executed on every entity used,
    /// so for now we're doing this once only here.
    /// </summary>
    /// <remarks>
    /// IMPORTANT: LOWER-CASE guaranteed.
    /// </remarks>
    [field: AllowNull, MaybeNull]
    public List<string> SiteCultures => field
        ??= zoneMapper.Value
                .CulturesEnabledWithState(SiteFromContextOrFallback)?
                .Select(c => c.Code.ToLowerInvariant())
                .ToList()
            ?? [];

    /// <summary>
    /// Special service provider for Data objects. Use with caution and as little as possible!
    ///
    /// Important: this will auto-attach to the root CodeApiSvc and get the context as well, which is important for render services etc.!
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <returns></returns>
    public TService GetService<TService>() where TService : class
        => ExCtxOrNull?.GetService<TService>() ?? serviceProvider.Build<TService>();
    
}