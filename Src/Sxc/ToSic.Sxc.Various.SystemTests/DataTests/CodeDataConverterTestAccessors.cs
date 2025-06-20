using ToSic.Eav.Data;
using ToSic.Lib.Coding;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Sys.Factory;

namespace ToSic.Sxc.DataTests;

internal static class CodeDataConverterTestAccessors
{
    public static ITypedItem? AsItemTac(this ICodeDataFactory cdf, object data, NoParamOrder noParamOrder = default, bool? required = default, ITypedItem fallback = default, bool? propsRequired = default, bool? mock = default)
        => cdf.AsItem(data, new() { ItemIsStrict = propsRequired ?? true, UseMock = mock == true }, noParamOrder, required, fallback);

    public static IEntity FakeEntityTac(this ICodeDataFactory cdf, int? appId)
        => cdf.FakeEntity(appId ?? 0);
}