using ToSic.Eav.Apps;
using ToSic.Eav.LookUp;
using ToSic.Lib.DI;
using ToSic.Lib.Helpers;
using ToSic.Lib.Services;
using ToSic.Razor.Blade;
using ToSic.Sxc.Services.DataServices;

namespace ToSic.Sxc.Services;

/// <summary>
/// Lightweight ServiceKit for 2sxc v15.
/// It's primarily used in dynamic code which runs standalone, without a module context.
///
/// Example: Custom DataSources can run anywhere without actually being inside a module or content-block.
/// In such scenarios, certain services like the <see cref="IPageService"/> would not be able to perform any real work.
/// </summary>
/// <remarks>
/// * History: Added v15.06 - still WIP
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

    private IAppIdentity _appIdentity;
    private Func<ILookUpEngine> _getLookup;

    ///// <summary>
    ///// The ADAM Service, used to retrieve files and maybe more. 
    ///// </summary>
    //public IAdamService Adam => _adam.Get(GetService<IAdamService>);
    //private readonly GetOnce<IAdamService> _adam = new GetOnce<IAdamService>();

    /// <inheritdoc cref="ServiceKit14.Convert"/>
    public IConvertService Convert => _convert.Get(GetService<IConvertService>);
    private readonly GetOnce<IConvertService> _convert = new();

    /// <inheritdoc cref="ServiceKit14.Data"/>
    public IDataService Data => _data.Get(() =>
    {
        var dss = GetService<IDataService>();
        (dss as DataService)?.Setup(_appIdentity, _getLookup);
        return dss;
    });
    private readonly GetOnce<IDataService> _data = new();


    /// <inheritdoc cref="ServiceKit14.Feature"/>
    public IFeaturesService Feature => _features.Get(GetService<IFeaturesService>);
    private readonly GetOnce<IFeaturesService> _features = new();

    /// <inheritdoc cref="ServiceKit14.HtmlTags"/>
    public IHtmlTagsService HtmlTags => _ht.Get(GetService<IHtmlTagsService>);
    private readonly GetOnce<IHtmlTagsService> _ht = new();

    /// <inheritdoc cref="ServiceKit14.Json"/>
    public IJsonService Json => _json.Get(GetService<IJsonService>);
    private readonly GetOnce<IJsonService> _json = new();

    /// <inheritdoc cref="ServiceKit14.SystemLog"/>
    public ISystemLogService SystemLog => _sysLog.Get(GetService<ISystemLogService>);
    private readonly GetOnce<ISystemLogService> _sysLog = new();

    /// <inheritdoc cref="ServiceKit14.SecureData"/>
    public ISecureDataService SecureData => _secureData.Get(GetService<ISecureDataService>);
    private readonly GetOnce<ISecureDataService> _secureData = new();

    /// <inheritdoc cref="ServiceKit14.Scrub"/>
    public IScrub Scrub => _scrub.Get(GetService<IScrub>);
    private readonly GetOnce<IScrub> _scrub = new();


    //[PrivateApi("Experimental in v15.03")]
    //public IUsersService Users => _users.Get(GetService<IUsersService>);
    //private readonly GetOnce<IUsersService> _users = new GetOnce<IUsersService>();
}