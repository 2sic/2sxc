using ToSic.Eav.Data;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Services;
using ToSic.Razor.Blade;
using ToSic.Sxc.Services;

namespace ToSic.Sxc.Data
{
    [PrivateApi]
    public class CodeDataServices: MyServicesBase
    {
        public CodeDataServices(LazySvc<IValueConverter> valueConverterLazy,
            Generator<IRenderService> renderServiceGenerator,
            LazySvc<IScrub> scrub,
            LazySvc<ConvertForCodeService> forCode)
        {
            ConnectServices(
                _valueConverterLazy = valueConverterLazy,
                _renderServiceGenerator = renderServiceGenerator,
                _scrub = scrub,
                _forCode = forCode
            );
        }
        /// <summary>
        /// The ValueConverter is used to parse links in the format like "file:72"
        /// </summary>
        
        internal IValueConverter ValueConverterOrNull => _valueConverterLazy.Value;
        private readonly LazySvc<IValueConverter> _valueConverterLazy;

        internal IRenderService RenderService => _renderServiceGenerator.New();
        private readonly Generator<IRenderService> _renderServiceGenerator;

        internal IScrub Scrub => _scrub.Value;
        private readonly LazySvc<IScrub> _scrub;

        internal ConvertForCodeService ForCode => _forCode.Value;
        private readonly LazySvc<ConvertForCodeService> _forCode;

    }
}
