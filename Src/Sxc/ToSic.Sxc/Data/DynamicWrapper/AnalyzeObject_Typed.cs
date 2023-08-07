using System.Collections.Generic;
using System.Runtime.CompilerServices;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Data.Typed;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data
{
    public partial class AnalyzeObject: IHasKeys, IGet4Typed //: ITyped, IHasKeys, IGet4Typed
    {
        [PrivateApi]
        public bool ContainsKey(string name) => _ignoreCaseLookup.ContainsKey(name);

        public IEnumerable<string> Keys(string noParamOrder, IEnumerable<string> only)
            => TypedHelpers.FilterKeysIfPossible(noParamOrder, only, _ignoreCaseLookup?.Keys);
        

        (bool Found, object Result) IGet4Typed.Get(string name)
        {
            var result = TryGet(name);
            return (result.Found, result.Result);
        }

        [PrivateApi]
        public TValue G4T<TValue>(string name, string noParamOrder, TValue fallback, [CallerMemberName] string cName = default)
        {
            Protect(noParamOrder, nameof(fallback), methodName: cName);
            return TryGet(name).Result.ConvertOrFallback(fallback);
        }
    }
}
