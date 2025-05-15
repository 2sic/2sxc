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
public interface IExecutionContext
{
    /// <summary>
    /// Get the current state of the execution context. Only works for a few types.
    /// </summary>
    /// <typeparam name="TState">The data type of known states: `IApp`, `ICmsContext` or `IDataSource`. Any other type will throw an error.</typeparam>
    /// <returns></returns>
    public TState GetState<TState>();
}

