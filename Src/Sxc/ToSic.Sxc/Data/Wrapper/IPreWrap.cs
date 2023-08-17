using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Lib.Documentation;
using static ToSic.Eav.Parameters;

namespace ToSic.Sxc.Data.Wrapper
{
    [PrivateApi]
    [JsonConverter(typeof(DynamicJsonConverter))]
    internal interface IPreWrap : /*IHasKeys,*/ IHasJsonSource, IPropertyLookup
    {
        TryGetResult TryGetWrap(string name, bool wrapDefault = true);

        TValue TryGetTyped<TValue>(string name, string noParamOrder, TValue fallback, bool? required, [CallerMemberName] string cName = default);

        WrapperSettings Settings { get; }

        bool ContainsKey(string name);

        IEnumerable<string> Keys(string noParamOrder = Protector, IEnumerable<string> only = default);
    }
}
