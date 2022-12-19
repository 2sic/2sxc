using System;
using ToSic.Eav.Data;
using ToSic.Eav.DI;
using ToSic.Lib.Logging;
using ToSic.Eav.Plumbing;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Blocks;
using IRenderService = ToSic.Sxc.Services.IRenderService;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// Dependencies every DynamicEntity has.
    /// They are needed for language lookups and creating inner lists of other entities
    /// </summary>
    [PrivateApi("this should all stay internal and never be public")]
    public class DynamicEntityDependencies
    {
        public DynamicEntityDependencies(Lazy<IDataBuilder> dataBuilderLazy, Lazy<IValueConverter> valueConverterLazy, Generator<Services.IRenderService> renderServiceGenerator)
        {
            _dataBuilderLazy = dataBuilderLazy;
            _valueConverterLazy = valueConverterLazy;
            _renderServiceGenerator = renderServiceGenerator;
        }

        internal DynamicEntityDependencies Init(IBlock blockOrNull, string[] dimensions, ILog log, int compatibility = Constants.CompatibilityLevel10)
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
        private readonly Lazy<IValueConverter> _valueConverterLazy;


        internal IDataBuilder DataBuilder => _dataBuilderLazy.Value;
        private readonly Lazy<IDataBuilder> _dataBuilderLazy;


        internal IRenderService RenderService => _renderServiceGenerator.New();
        private readonly Generator<IRenderService> _renderServiceGenerator;
    }
}
