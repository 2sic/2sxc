using ToSic.Sxc.Data;
using ToSic.Sys.Caching.PiggyBack;

namespace ToSic.Sxc.Sys.ExecutionContext;

/// <summary>
/// Internal context information about the current execution.
/// </summary>
/// <remarks>
/// It contains:
/// 1. state
/// 2. services
/// 3. ...and more
///
/// The initial interface has none or few properties, so that we can pass it around without all projects being tied to each other.
/// </remarks>
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IExecutionContext: IContextServiceProvider, IHasPiggyBack, IHasLog
{
    ///// <summary>
    ///// Get special services which data need to initialize.
    ///// This is a select list, ATM just the `AdamManager`
    ///// </summary>
    ///// <typeparam name="TService"></typeparam>
    ///// <returns></returns>
    //TService GetServiceForData<TService>() where TService : class;

    ITypedStack AllSettings { get; }
    ITypedStack AllResources { get; }
}
