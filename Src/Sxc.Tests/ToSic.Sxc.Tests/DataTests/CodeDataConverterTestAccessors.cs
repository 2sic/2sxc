using ToSic.Eav.Data;
using ToSic.Lib.Coding;
using ToSic.Sxc.Data;
using ToSic.Sxc.Data.Internal;
using ToSic.Sxc.Models;

namespace ToSic.Sxc.Tests.DataTests;

internal static class CodeDataConverterTestAccessors
{
    public static ITypedItem AsItemTac(this CodeDataFactory cdf, object data, NoParamOrder noParamOrder = default, bool? required = default, ITypedItem fallback = default, bool? propsRequired = default, bool? mock = default)
        => cdf.AsItem(data, noParamOrder, required, fallback, propsRequired, mock);

    public static IEntity FakeEntityTac(this CodeDataFactory cdf, int? appId)
        => cdf.FakeEntity(appId);

    public static string CdfGetContentTypeNameTac<T>()
        where T : class, IDataModel
        => CodeDataFactory.GetContentTypeName<T>();

    public static string CdfGetStreamNameTac<T>()
        where T : class, IDataModel
        => CodeDataFactory.GetStreamName<T>();

}