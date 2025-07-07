using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using ToSic.Sxc.Data.Sys.Json;
using ToSic.Sxc.Data.Sys.Typed;

namespace ToSic.Sxc.Data.Sys.Wrappers;

[PrivateApi]
[JsonConverter(typeof(DynamicJsonConverter))]
[ShowApiWhenReleased(ShowApiMode.Never)]
internal interface IPreWrap : IHasJsonSource, IPropertyLookup, IWrapper<object>
{
    TryGetResult TryGetWrap(string? name, bool wrapDefault = true);

    object TryGetObject(string name, NoParamOrder noParamOrder, bool? required, [CallerMemberName] string? cName = default);

    [return: NotNullIfNotNull(nameof(fallback))]
    TValue? TryGetTyped<TValue>(string name, NoParamOrder noParamOrder, TValue? fallback, bool? required, [CallerMemberName] string? cName = default);

    WrapperSettings Settings { get; }

    bool ContainsKey(string name);

    IEnumerable<string> Keys(NoParamOrder noParamOrder = default, IEnumerable<string>? only = default);
}