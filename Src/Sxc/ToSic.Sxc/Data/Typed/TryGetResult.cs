using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class TryGetResult
{
    public TryGetResult(bool found, object result)
    {
        Found = found;
        Raw = result;
        Result = result;
    }

    public TryGetResult(bool found, object raw, object result = default)
    {
        Found = found;
        Raw = raw;
        Result = result;
    }
    public bool Found;
    public object Raw;
    public object Result;
}