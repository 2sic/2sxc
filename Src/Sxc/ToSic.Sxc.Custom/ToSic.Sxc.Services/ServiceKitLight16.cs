using ToSic.Eav.Apps;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Razor.Blade;
using ToSic.Sxc.Services.Data.Sys;

namespace ToSic.Sxc.Services;

/// <summary>
/// Lightweight ServiceKit for 2sxc v16.
/// Provided in custom data sources as `Kit`.
/// </summary>
/// <remarks>
/// It's primarily used in dynamic code which runs standalone, without a module context.
///
/// Example: Custom DataSources can run anywhere without actually being inside a module or content-block.
/// In such scenarios, certain services like the <see cref="IPageService"/> would not be able to perform any real work.
/// 
/// History: Added v15.06 - still WIP
/// </remarks>
[PublicApi]
public class ServiceKitLight16(IServiceProvider serviceProvider) : ServiceBase("Sxc.Kit15", connect: [/* never! serviceProvider */ ])
{
    private TService GetService<TService>() => serviceProvider.Build<TService>(Log);

    internal ServiceKitLight16 Setup(IAppIdentity appIdentity, Func<ILookUpEngine> getLookup)
    {
        _appIdentity = appIdentity;
        _getLookup = getLookup;
        return this;
    }

    private IAppIdentity _appIdentity = null!;
    private Func<ILookUpEngine> _getLookup = null!;

    /// <inheritdoc cref="ServiceKit14.Convert"/>
    public IConvertService Convert => field ??= GetService<IConvertService>();

    /// <inheritdoc cref="ServiceKit14.Data"/>
    public IDataService Data => field ??= new Func<IDataService>(() =>
    {
        var dss = GetService<IDataService>();
        (dss as DataService)?.Setup(new(_appIdentity, _getLookup));
        return dss;
    })();


    /// <inheritdoc cref="ServiceKit14.Feature"/>
    public IFeaturesService Feature => field ??= GetService<IFeaturesService>();

    /// <inheritdoc cref="ServiceKit14.HtmlTags"/>
    public IHtmlTagsService HtmlTags => field ??= GetService<IHtmlTagsService>();

    /// <inheritdoc cref="ServiceKit14.Json"/>
    public IJsonService Json => field ??= GetService<IJsonService>();

    /// <inheritdoc cref="ServiceKit14.SystemLog"/>
    public ISystemLogService SystemLog => field ??= GetService<ISystemLogService>();

    /// <inheritdoc cref="ServiceKit14.SecureData"/>
    public ISecureDataService SecureData => field ??= GetService<ISecureDataService>();

    /// <inheritdoc cref="ServiceKit14.Scrub"/>
    public IScrub Scrub => field ??= GetService<IScrub>();


    //[PrivateApi("Experimental in v15.03")]
    //public IUsersService Users => _users.Get(GetService<IUsersService>);
    //private readonly GetOnce<IUsersService> _users = new GetOnce<IUsersService>();
}