using ToSic.Eav.Data;
using ToSic.Lib.Coding;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal;

namespace ToSic.Sxc.Tests.DataTests;

internal static class CodeDataConverterTestAccessors
{
    public static ITypedItem TacAsItem(this CodeDataFactory cdf, object data, NoParamOrder noParamOrder = default, bool? required = default, ITypedItem fallback = default, bool? propsRequired = default, bool? mock = default)
        => cdf.AsItem(data, noParamOrder, required, fallback, propsRequired, mock);

    public static IEntity TacFakeEntity(this CodeDataFactory cdf, int? appId)
        => cdf.FakeEntity(appId);
}