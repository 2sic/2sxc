using ToSic.Eav.Code;

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
public interface IExecutionContext: ICanGetService, IHasLog
{
    /// <summary>
    /// Get the current state of the execution context. Only works for a few types.
    /// </summary>
    /// <typeparam name="TState">The data type of known states: `IApp`, `ICmsContext` or `IDataSource` and a few more.
    /// Any other type will throw an error.</typeparam>
    /// <returns></returns>
    public TState GetState<TState>() where TState : class;

    /// <summary>
    /// Special accessor - currently only for `IDynamicStack` and name "Settings"
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <param name="name"></param>
    /// <returns></returns>
    public TState GetState<TState>(string name) where TState : class;

    /// <summary>
    /// Special GetService which can cache the found service so any other use could get the same instance.
    /// This should ensure that an Edit service requested through Kit14 and Kit16 are both the same, etc.
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <param name="protector"></param>
    /// <param name="reuse">if true, then a service requested multiple times will return the same instance</param>
    /// <returns></returns>
    [PrivateApi]
    TService GetService<TService>(NoParamOrder protector = default, bool reuse = false, Type type = default) where TService : class;

    /// <summary>
    /// Get special services which data need to initialize.
    /// This is a select list, ATM just the `AdamManager`
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <returns></returns>
    TService GetServiceForData<TService>() where TService : class;
}

