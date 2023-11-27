using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Lib.Coding;
using ToSic.Lib.Data;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data.Wrapper;

[PrivateApi]
[JsonConverter(typeof(DynamicJsonConverter))]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public interface IPreWrap : IHasJsonSource, IPropertyLookup, IWrapper<object>
{
    TryGetResult TryGetWrap(string name, bool wrapDefault = true);

    object TryGetObject(string name, NoParamOrder noParamOrder, bool? required, [CallerMemberName] string cName = default);

    TValue TryGetTyped<TValue>(string name, NoParamOrder noParamOrder, TValue fallback, bool? required, [CallerMemberName] string cName = default);

    WrapperSettings Settings { get; }

    bool ContainsKey(string name);

    IEnumerable<string> Keys(NoParamOrder noParamOrder = default, IEnumerable<string> only = default);
}