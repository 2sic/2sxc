using ToSic.Eav.Data;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data
{
    [PrivateApi]
    public class TryGetResult
    {
        public TryGetResult(bool found, object result)//, PropReqResult propReqResult)
        {
            Found = found;
            Raw = result;
            Result = result;
            //PropReqResult = propReqResult;
        }

        public TryGetResult(bool found, object raw, object result = default, PropReqResult propReqResult = default)
        {
            Found = found;
            Raw = raw;
            Result = result;
            PropReqResult = propReqResult;
        }
        public bool Found;
        public object Raw;
        public object Result;
        public PropReqResult PropReqResult;
    }
}