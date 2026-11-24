namespace ToSic.Sxc.Code;

/// <summary>
/// Interface to mark objects which can get a service.
/// Mainly used for code context, code helpers and public APIs which support `GetService{T}()`
/// </summary>
[PrivateApi, ShowApiWhenReleased(ShowApiMode.Never)]
public interface IGetService
{
    /// <summary>
    /// Get a service from the Dependency Injection.
    /// The service can come from 2sxc, EAV or the underlying platform (Dnn, Oqtane).
    /// </summary>
    /// <typeparam name="TService">Interface (preferred) or Class which is needed</typeparam>
    /// <returns>An object of the type or interface requested, or null if not found in the DI.</returns>
    TService GetService<TService>() where TService : class;
}
