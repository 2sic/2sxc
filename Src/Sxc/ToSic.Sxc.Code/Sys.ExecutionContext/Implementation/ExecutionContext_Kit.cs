using ToSic.Sxc.Adam.Sys.Manager;
using ToSic.Sxc.Data.Sys.CodeDataFactory;
using ToSic.Sxc.Services.Sys;

namespace ToSic.Sxc.Sys.ExecutionContext;

public partial class ExecutionContext: ICanGetService
{
    /// <inheritdoc cref="ICanGetService.GetService{TService}"/>
    public TService GetService<TService>() where TService : class
    {
        var newService = Services.ServiceProvider.Build<TService>(Log);
        if (newService is INeedsExecutionContext newWithNeeds)
            newWithNeeds.ConnectToRoot(this);
        return newService;
    }


    #region Kit Handling

    // ReSharper disable once MethodOverloadWithOptionalParameter
    public TService GetService<TService>(NoParamOrder npo = default, bool reuse = false, Type? type = default) where TService : class
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

}