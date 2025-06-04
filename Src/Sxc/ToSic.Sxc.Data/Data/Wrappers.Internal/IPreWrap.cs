using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using ToSic.Eav.Data.Sys;
using ToSic.Lib.Wrappers;
using ToSic.Sxc.Data.Internal.Convert;
using ToSic.Sxc.Data.Internal.Typed;

namespace ToSic.Sxc.Data.Internal.Wrapper;

[PrivateApi]
[JsonConverter(typeof(DynamicJsonConverter))]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal interface IPreWrap : IHasJsonSource, IPropertyLookup, IWrapper<object>
{
    TryGetResult TryGetWrap(string name, bool wrapDefault = true);

    object TryGetObject(string name, NoParamOrder noParamOrder, bool? required, [CallerMemberName] string cName = default);

    TValue TryGetTyped<TValue>(string name, NoParamOrder noParamOrder, TValue fallback, bool? required, [CallerMemberName] string cName = default);

    WrapperSettings Settings { get; }

    bool ContainsKey(string name);

    IEnumerable<string> Keys(NoParamOrder noParamOrder = default, IEnumerable<string> only = default);
}