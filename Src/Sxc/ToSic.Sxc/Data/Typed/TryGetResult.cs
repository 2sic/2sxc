using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data
{
    [PrivateApi]
    public class TryGetResult
    {
        public TryGetResult(object result, bool found)
        {
            Result = result;
            Found = found;
        }
        public bool Found;
        public object Result;
    }
}