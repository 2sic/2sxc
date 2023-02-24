using ToSic.Eav.Data;
using ToSic.Eav.Data.Build;
using ToSic.Lib.Logging;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks;
using IRenderService = ToSic.Sxc.Services.IRenderService;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// Dependencies every DynamicEntity has.
    /// They are needed for language lookups and creating inner lists of other entities
    /// </summary>
    [PrivateApi("this should all stay internal and never be public")]
    public class DynamicEntityServices: MyServicesBase
    {
        public DynamicEntityServices(
            LazySvc<IDataBuilderInternal> dataBuilderLazy,
            LazySvc<IValueConverter> valueConverterLazy,
            Generator<IRenderService> renderServiceGenerator)
        {
            _dataBuilderLazy = dataBuilderLazy;
            _valueConverterLazy = valueConverterLazy;
            _renderServiceGenerator = renderServiceGenerator;
        }

        internal DynamicEntityServices Init(IBlock blockOrNull, string[] dimensions, ILog log, int compatibility = Constants.CompatibilityLevel10)
        {
            Dimensions = dimensions;
            LogOrNull = log;
            CompatibilityLevel = compatibility;
            BlockOrNull = blockOrNull;
            return this;
        }
        
        internal IBlock BlockOrNull { get; private set; }

        internal string[] Dimensions { get; private set; }

        internal ILog LogOrNull { get; private set; }

        internal int CompatibilityLevel { get; private set; }


        /// <summary>
        /// The ValueConverter is used to parse links in the format like "file:72"
        /// </summary>
        [PrivateApi]
        internal IValueConverter ValueConverterOrNull => _valueConverterLazy.Value;
        private readonly LazySvc<IValueConverter> _valueConverterLazy;


        internal IDataBuilderInternal DataBuilder => _dataBuilderLazy.Value;
        private readonly LazySvc<IDataBuilderInternal> _dataBuilderLazy;


        internal IRenderService RenderService => _renderServiceGenerator.New();
        private readonly Generator<IRenderService> _renderServiceGenerator;
    }
}
