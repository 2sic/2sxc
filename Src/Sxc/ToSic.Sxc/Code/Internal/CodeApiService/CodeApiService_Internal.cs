using ToSic.Sxc.Apps;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Polymorphism.Internal;

namespace ToSic.Sxc.Code.Internal;

public partial class CodeApiService : ICodeApiServiceInternal
{
    [PrivateApi]
    public void AttachApp(IApp app)
    {
        if (app is App typedApp) typedApp.SetupAsConverter(_Cdf);
        App = app;

        _edition = PolymorphConfigReader.UseViewEditionOrGetLazy(_Block?.View, () => Services.Polymorphism.Init(App.AppState.List));
        //_edition = _Block?.View?.Edition.NullIfNoValue() // if Block-View comes with a preset edition, it's an ajax-preview which should be respected
        //          ?? Services.Polymorphism.Init(App.AppState.List).Edition(); // Figure out edition using data
    }

    private string _edition;

    [PrivateApi]
    [Obsolete("Warning - avoid using this on the DynamicCode Root - always use the one on the AsC")]
    public int CompatibilityLevel => _Cdf.CompatibilityLevel;

    [PrivateApi] public IBlock _Block { get; private set; }

    #region Kit Handling

    TService ICodeApiServiceInternal.GetKitService<TService>()
    {
        var type = typeof(TService);
        if (_reusableServices.TryGetValue(type, out var service))
            return (TService)service;
        var generated = _CodeApiSvc.GetService<TService>();
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
    TKit ICodeApiServiceInternal.GetKit<TKit>() => ((ICodeApiServiceInternal)this).GetKitService<TKit>();

    #endregion
}