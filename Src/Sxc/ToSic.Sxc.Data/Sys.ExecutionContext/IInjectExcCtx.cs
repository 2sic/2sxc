namespace ToSic.Sxc.Sys.ExecutionContext;

/// <summary>
/// Special interface to use only when injecting the <see cref="IExecutionContext"/>.
/// </summary>
[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IInjectExcCtx : IExecutionContext;
