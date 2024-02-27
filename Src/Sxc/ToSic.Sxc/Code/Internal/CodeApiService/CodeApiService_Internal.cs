using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Polymorphism.Internal;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Code.Internal;

public partial class CodeApiService : ICodeApiServiceInternal
{
    [PrivateApi]
    public void AttachApp(IApp app)
    {
        if (app is App typedApp)
            typedApp.SetupAsConverter(Cdf);

        // WIP - enable app.Data to do GetOne<T>, GetMany<T> etc.
        // Note that app.Data is only the typed one, if app is first cast to IAppTyped
        if (app is IAppTyped { Data: AppDataTyped appDataTyped })
            appDataTyped.Setup(((ICodeApiServiceInternal)this).GetKit<ServiceKit16>());

        App = app;

        _edition = PolymorphConfigReader.UseViewEditionOrGetLazy(_Block?.View, () => Services.Polymorphism.Init(App.AppState.List));
    }

    private string _edition;

    [PrivateApi]
    [Obsolete("Warning - avoid using this on the DynamicCode Root - always use the one on the AsC")]
    public int CompatibilityLevel => Cdf.CompatibilityLevel;

    [PrivateApi] public IBlock _Block { get; private set; }

    #region Kit Handling

    public TService GetService<TService>(NoParamOrder protector = default, bool reuse = false) where TService : class
    {
        if (!reuse) return GetService<TService>();

        var type = typeof(TService);
        if (_reusableServices.TryGetValue(type, out var service))
            return (TService)service;
        var generated = GetService<TService>();
        _reusableServices[type] = generated;
        return generated;
    }
    
    /// <summary>
    /// A kind of cache for:
    /// - all kinds of kits by version, eg. Kit14, Kit16
    /// - all services used inside these kits, as they should share the state (eg. the Edit kit)
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