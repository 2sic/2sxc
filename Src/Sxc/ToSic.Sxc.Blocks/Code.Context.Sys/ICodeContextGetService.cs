using ToSic.Sxc.Sys.ExecutionContext;

namespace ToSic.Sxc.Code.Context.Sys;

[ShowApiWhenReleased(ShowApiMode.Never)]
public interface ICodeContextGetService : ICanGetService
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
