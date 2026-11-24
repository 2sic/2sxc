namespace ToSic.Sxc.Sys.ExecutionContext;

/// <summary>
/// Interface to mark objects which can get a service.
/// </summary>
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface ICanGetService
{
    /// <summary>
    /// Get a service from the Dependency Injection.
    /// The service can come from 2sxc, EAV or the underlying platform (Dnn, Oqtane).
    /// </summary>
    /// <typeparam name="TService">Interface (preferred) or Class which is needed</typeparam>
    /// <returns>An object of the type or interface requested, or null if not found in the DI.</returns>
    TService GetService<TService>() where TService : class;
}

public interface IContextServiceProvider : ICanGetService
{
    /// <summary>
    /// Special GetService which can cache the found service so any other use could get the same instance.
    /// This should ensure that an Edit service requested through Kit14 and Kit16 are both the same, etc.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="npo">see [](xref:NetCode.Conventions.NamedParameters)</param>
    /// <param name="reuse">if true, then a service requested multiple times will return the same instance</param>
    /// <param name="type">rare option: type name of the service to create; probably not relevant any more today</param>
    /// <returns></returns>
    [PrivateApi]
    TService GetService<TService>(NoParamOrder npo = default, bool reuse = false, Type? type = default) where TService : class;
}
