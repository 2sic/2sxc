using ToSic.Eav.Data.Build;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Razor.Blade;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Data.Internal;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class CodeDataServices(
    LazySvc<IValueConverter> valueConverterLazy,
    Generator<IRenderService> renderServiceGenerator,
    LazySvc<IScrub> scrub,
    LazySvc<ConvertForCodeService> forCode,
    LazySvc<IDataFactory> dataFactory)
    : MyServicesBase(connect: [valueConverterLazy, renderServiceGenerator, scrub, forCode, dataFactory])
{
    /// <summary>
    /// The ValueConverter is used to parse links in the format like "file:72"
    /// </summary>
    internal IValueConverter ValueConverterOrNull => valueConverterLazy.Value;

    /// <summary>
    /// This is used in special cases where static Render is called.
    /// It's not elegant, but necessary to maintain old code.
    /// </summary>
    internal readonly Generator<IRenderService> RenderServiceGenerator = renderServiceGenerator;

    /// <summary>
    /// This is provided so that ITypedItems can use Scrub in the String APIs
    /// </summary>
    internal IScrub Scrub => scrub.Value;

    internal ConvertForCodeService ForCode => forCode.Value;

    internal IDataFactory DataFactory => dataFactory.Value;
}