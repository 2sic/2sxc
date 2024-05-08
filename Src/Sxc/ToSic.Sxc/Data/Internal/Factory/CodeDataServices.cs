using ToSic.Eav.Data.Build;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Razor.Blade;
using ToSic.Sxc.Services;
using ToSic.Sxc.Services.Internal;

namespace ToSic.Sxc.Data.Internal;

[PrivateApi]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class CodeDataServices: MyServicesBase
{
    public CodeDataServices(
        LazySvc<IValueConverter> valueConverterLazy,
        Generator<IRenderService> renderServiceGenerator,
        LazySvc<IScrub> scrub,
        LazySvc<ConvertForCodeService> forCode,
        LazySvc<IDataFactory> dataFactory)
    {
        ConnectLogs([
            _valueConverterLazy = valueConverterLazy,
            RenderServiceGenerator = renderServiceGenerator,
            _scrub = scrub,
            _forCode = forCode,
            _dataFactory = dataFactory
        ]);
    }
    /// <summary>
    /// The ValueConverter is used to parse links in the format like "file:72"
    /// </summary>
    internal IValueConverter ValueConverterOrNull => _valueConverterLazy.Value;
    private readonly LazySvc<IValueConverter> _valueConverterLazy;

    /// <summary>
    /// This is used in special cases where static Render is called.
    /// It's not elegant, but necessary to maintain old code.
    /// </summary>
    internal readonly Generator<IRenderService> RenderServiceGenerator;

    /// <summary>
    /// This is provided so that ITypedItems can use Scrub in the String APIs
    /// </summary>
    internal IScrub Scrub => _scrub.Value;
    private readonly LazySvc<IScrub> _scrub;

    internal ConvertForCodeService ForCode => _forCode.Value;
    private readonly LazySvc<ConvertForCodeService> _forCode;

    internal IDataFactory DataFactory => _dataFactory.Value;
    private readonly LazySvc<IDataFactory> _dataFactory;

}