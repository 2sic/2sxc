namespace ToSic.Sxc.Sys.ExecutionContext;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public interface IExCtxValue<out TValue>
{
    TValue Value { get; }
}