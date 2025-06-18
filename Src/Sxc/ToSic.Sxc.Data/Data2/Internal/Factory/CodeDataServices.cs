using ToSic.Eav.Data.Build;
using ToSic.Eav.Data.ValueConverter.Sys;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Razor.Blade;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Data.Internal;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class CodeDataServices(
    LazySvc<IValueConverter> valueConverterLazy,
    LazySvc<IScrub> scrub,
    LazySvc<ConvertForCodeService> forCode,
    LazySvc<IDataFactory> dataFactory)
    : MyServicesBase(connect: [valueConverterLazy, scrub, forCode, dataFactory])
{
    /// <summary>
    /// The ValueConverter is used to parse links in the format like "file:72"
    /// </summary>
    /// <remarks>
    /// Before 2025-06-18 was called ValueConverterOrNull - but it's from a service so it should always be there...
    /// </remarks>
    internal IValueConverter ValueConverter => valueConverterLazy.Value;

    /// <summary>
    /// This is provided so that ITypedItems can use Scrub in the String APIs
    /// </summary>
    internal IScrub Scrub => scrub.Value;

    internal ConvertForCodeService ForCode => forCode.Value;

    internal IDataFactory DataFactory => dataFactory.Value;
}