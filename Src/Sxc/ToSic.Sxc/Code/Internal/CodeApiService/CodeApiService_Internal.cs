using ToSic.Eav.Apps;
using ToSic.Lib.DI;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks.Internal;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Code.Internal;

public partial class CodeApiService : ICodeApiServiceInternal
{
    [PrivateApi]
    public void AttachApp(IApp app)
    {
        if (app is App typedApp)
            typedApp.SetupAsConverter(Cdf);

        App = app;

        _edition = Services.Polymorphism.UseViewEditionOrGet(_Block?.View, ((IAppWithInternal)App).AppReader);
    }

    private string _edition;

    [PrivateApi]
    [Obsolete("Warning - avoid using this on the DynamicCode Root - always use the one on the AsC")]
    public int CompatibilityLevel => Cdf.CompatibilityLevel;

    [PrivateApi] public IBlock _Block { get; private set; }

    [PrivateApi]
    public IAppTyped AppTyped => _appTyped ??= new Func<IAppTyped>(() => GetService<IAppTyped>(reuse: true))();
    private IAppTyped _appTyped;
    
    #region Kit Handling

    public TService GetService<TService>(NoParamOrder protector = default, bool reuse = false, Type type = default) where TService : class
    {
        if (!reuse) return BuildWithOptionalType();

        var typeInCache = type ?? typeof(TService);
        if (_reusableServices.TryGetValue(typeInCache, out var service))
            return (TService)service;
        
        var generated = BuildWithOptionalType();
        _reusableServices[typeInCache] = generated;
        return generated;

        TService BuildWithOptionalType()
        {
            var newService = type == null 
                ? Services.ServiceProvider.Build<TService>(Log)
                : Services.ServiceProvider.Build<TService>(type, Log);

            if (newService is INeedsCodeApiService newWithNeeds)
                newWithNeeds.ConnectToRoot(this);
            return newService;
        }
    }

    /// <summary>
    /// A kind of cache for:
    /// - all kinds of kits by version, like Kit14, Kit16
    /// - all services used inside these kits, as they should share the state (like the Edit kit)
    /// ...so we don't have to create them over and over again.
    /// This allows us to just get an object, kit, and if it's already created, we get the same instance.
    /// </summary>
    private readonly Dictionary<Type, object> _reusableServices = [];

    /// <summary>
    /// Get or Create a Kit by type
    /// </summary>
    /// <typeparam name="TKit"></typeparam>
    /// <returns></returns>
    TKit ICodeApiServiceInternal.GetKit<TKit>() => GetService<TKit>(reuse: true);

    #endregion
}