using ToSic.Eav.Data;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Services;
using ToSic.Razor.Blade;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Services;
using IRenderService = ToSic.Sxc.Services.IRenderService;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity
    {
        /// <summary>
        /// Dependencies every DynamicEntity has.
        /// They are needed for language lookups and creating inner lists of other entities
        /// </summary>
        [PrivateApi("this should all stay internal and never be public")]
        public class MyServices : MyServicesBase
        {
            public MyServices(LazySvc<IValueConverter> valueConverterLazy,
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

            internal MyServices Init(IBlock blockOrNull, string[] dimensions, CodeDataFactory cdf)
            {
                Dimensions = dimensions;
                BlockOrNull = blockOrNull;
                Cdf = cdf;
                return this;
            }

            internal IBlock BlockOrNull { get; private set; }

            internal string[] Dimensions { get; private set; }


            internal CodeDataFactory Cdf { get; private set; }

            /// <summary>
            /// The ValueConverter is used to parse links in the format like "file:72"
            /// </summary>
            [PrivateApi]
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
}
