using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.LookUp.Sys.Engines;
using ToSic.Sxc.Adam.Sys.Manager;
using ToSic.Sxc.Apps.Sys;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Data.Sys.CodeDataFactory;
using ToSic.Sxc.Data.Sys.Factory;
using ToSic.Sxc.Services.Sys;

namespace ToSic.Sxc.Code.Context.Sys;

public partial class CodeContext: IGetService
{
    private ISysServiceProvider SpWithOverrides => field ??= GetSpWithOverrides();

    private ISysServiceProvider GetSpWithOverrides()
    {
        // Some services such as the Cdf cannot be created at first,
        // as they also use the GetService to be created; so they must be delayed

        // 0. AppIdentity - must be delayed, as it may not exist the first time this is called
        using (Ctx<IAppIdentity>.Use(() => App?.PureIdentity() ?? throw new ArgumentNullException(nameof(App))))
        using (Ctx<IAppForCode>.Use(() => App ?? throw new ArgumentNullException(nameof(App))))
        // 0. Settings, Block
        using (Ctx<ICodeContextSettings>.Use(this))
        using (Ctx<IBlock>.Use(() => Block!))
        // 1. Main / full context
        using (Ctx<ICodeContext>.Use(this))
        // 2. Cdf service, must be delayed
        using (Ctx<ICodeDataFactory>.Use(() => Cdf))
        // ILookUpEngine
        using (Ctx<ILookUpEngine>.Use(() => LookUpForDataSources))
        // 3. AdamManager service for AdamService (WIP)
        using (Ctx<AdamManager>.Use(() => ((CodeDataFactory)Cdf).AdamManager))
        // 4. Service provider with overrides and `reuse:`
        using (Ctx<ICodeContextGetService>.Use(this))
            return Services.ServiceProvider.GetSysServiceProvider();
    }

    /// <inheritdoc cref="IGetService.GetService{TService}"/>
    public TService GetService<TService>() where TService : class
    {
        var newService = SpWithOverrides.Build<TService>(Log);
        if (newService is ICodeContextRequired newWithNeeds)
            newWithNeeds.SetupContext(this);
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
                ? SpWithOverrides.Build<TService>(Log)
                : SpWithOverrides.Build<TService>(type, Log);

            if (newService is ICodeContextRequired newWithNeeds)
                newWithNeeds.SetupContext(this);
            return newService;
        }
    }

    //public TService GetServiceForData<TService>() where TService : class
    //{
    //    if (typeof(TService) == typeof(AdamManager))
    //        return ((CodeDataFactory)Cdf).AdamManager as TService
    //            ?? throw new InvalidOperationException("Can't get AdamManager from CodeContext - it is not initialized.");

    //    throw new NotImplementedException();
    //}

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

    #endregion

}
