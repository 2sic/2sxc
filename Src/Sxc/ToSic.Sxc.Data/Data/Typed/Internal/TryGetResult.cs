namespace ToSic.Sxc.Data.Internal.Typed;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal class TryGetResult(bool found, object? raw, object? result = default)
{
    public TryGetResult(bool found, object? result): this(found, result, result)
    { }

    public bool Found = found;
    public object? Raw = raw;
    public object? Result = result;
}