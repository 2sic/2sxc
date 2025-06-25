using ToSic.Eav.Apps;
using ToSic.Eav.DataSource;
using ToSic.Lib.DI;
using ToSic.Lib.Wrappers;
using ToSic.Sxc.Adam.Sys.Manager;
using ToSic.Sxc.Apps;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Internal;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Services;
using ToSic.Sxc.Sys.ExecutionContext;
using ToSic.Sys.Users;
using IApp = ToSic.Sxc.Apps.IApp;

namespace ToSic.Sxc.Code.Internal;

public partial class ExecutionContext
    : IWrapper<ICodeDataFactory>,
        IExCtxAttachApp
{
    [PrivateApi]
    public void AttachApp(IApp app)
    {
        if (app is App typedApp)
            typedApp.SetupAsConverter(Cdf);

        App = app;

        _edition = Services.Polymorphism.UseViewEditionOrGet(Block?.View, ((IAppWithInternal)App).AppReader);
    }

    private string? _edition;

    [PrivateApi]
    [Obsolete("Warning - avoid using this on the DynamicCode Root - always use the one on the AsC")]
    public int CompatibilityLevel => Cdf.CompatibilityLevel;

    [PrivateApi] public IBlock Block { get; private set; } = null!;

    [PrivateApi]
    [field: AllowNull, MaybeNull]
    internal IAppTyped AppTyped => field ??= new Func<IAppTyped>(() => GetService<IAppTyped>(reuse: true))();

    #region Kit Handling

    // ReSharper disable once MethodOverloadWithOptionalParameter
    public TService GetService<TService>(NoParamOrder protector = default, bool reuse = false, Type? type = default) where TService : class
    {
        // No reuse - just build and return, but optionally with the type specified
        if (!reuse)
            return BuildWithOptionalType();

        // Reuse - check if we already have it, if not, build it and store it
        var typeInCache = type ?? typeof(TService);
        if (_reusableServices.TryGetValue(typeInCache, out var service))
            return (TService)service;

        // Not found, so build it and store it
        var generated = BuildWithOptionalType();
        _reusableServices[typeInCache] = generated;
        return generated;

        TService BuildWithOptionalType()
        {
            var newService = type == null 
                ? Services.ServiceProvider.Build<TService>(Log)
                : Services.ServiceProvider.Build<TService>(type, Log);

            if (newService is INeedsExecutionContext newWithNeeds)
                newWithNeeds.ConnectToRoot(this);
            return newService;
        }
    }

    public TService GetServiceForData<TService>() where TService : class
    {
        if (typeof(TService) == typeof(AdamManager))
            return ((CodeDataFactory)Cdf).AdamManager as TService
                ?? throw new InvalidOperationException("Can't get AdamManager from ExecutionContext - it is not initialized.");

        throw new NotImplementedException();
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
    /// WIP 19.03.xx - replace a service in the cache, mainly for unit testing
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="service"></param>
    internal void ReplaceServiceInCache<TService>(TService service)
        => _reusableServices[typeof(TService)] = service!;

    /// <summary>
    /// Get or Create a Kit by type
    /// </summary>
    /// <typeparam name="TKit"></typeparam>
    /// <returns></returns>
    public TKit GetKit<TKit>() where TKit : ServiceKit
        => GetService<TKit>(reuse: true);

    ///// <summary>
    ///// Special workaround so this can provide the data without
    ///// having to support an interface.
    ///// To call this, you must explicitly cast it to IWrapper&lt;IServiceKitForTypedData&gt;
    ///// </summary>
    ///// <returns></returns>
    //IExCtxServicesForTypedData IWrapper<IExCtxServicesForTypedData>.GetContents()
    //    => GetKit<ServiceKit16>();

    #endregion

    public TState GetState<TState>() where TState : class
    {
        if (typeof(TState) == typeof(IUser))
            return (TState)CmsContext.User;

        if (typeof(TState) == typeof(ICmsContext))
            return (TState)CmsContext;

        if (typeof(TState) == typeof(IApp))
            return (TState)App;

        if (typeof(TState) == typeof(IAppReader))
            return (TState)((IAppWithInternal)App)?.AppReader!;

        if (typeof(TState) == typeof(IDataSource))
            return (TState)Block.Data;

        if (typeof(TState) == typeof(IBlock))
            return (TState)Block;

        if (typeof(TState) == typeof(IContextOfBlock))
            return (TState)Block?.Context!;

        if (typeof(TState) == typeof(IAppTyped))
            return (TState)AppTyped;

        throw new InvalidOperationException(
            $"Can't get state of type {typeof(TState).Name} - only {nameof(IApp)}, {nameof(IDataSource)}, {nameof(IBlock)} and {nameof(IAppTyped)} are supported");
    }

    public TState GetState<TState>(string name) where TState : class
    {
        if (typeof(TState) == typeof(IDynamicStack) && name == ExecutionContextStateNames.Settings)
            return (TState)Settings;

        if (typeof(TState) == typeof(ITypedStack) && name == ExecutionContextStateNames.AllSettings)
            return (TState)AllSettings;

        if (typeof(TState) == typeof(ITypedStack) && name == ExecutionContextStateNames.AllResources)
            return (TState)AllResources;

        throw new InvalidOperationException("Can only retrieve 'Settings', 'AllSettings', 'AllResources'");
    }

    ICodeDataFactory IWrapper<ICodeDataFactory>.GetContents()
        => Cdf;
}