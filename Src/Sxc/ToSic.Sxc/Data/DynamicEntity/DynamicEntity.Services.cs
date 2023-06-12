using System;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Build;
using ToSic.Lib.Logging;
using ToSic.Lib.DI;
using ToSic.Lib.Documentation;
using ToSic.Lib.Helpers;
using ToSic.Lib.Services;
using ToSic.Sxc.Adam;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Data.AsConverter;
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
            public MyServices(
                LazySvc<DataBuilder> dataBuilderLazy,
                LazySvc<IValueConverter> valueConverterLazy,
                Generator<IRenderService> renderServiceGenerator)
            {
                _dataBuilderLazy = dataBuilderLazy;
                _valueConverterLazy = valueConverterLazy;
                _renderServiceGenerator = renderServiceGenerator;
            }

            internal MyServices Init(IBlock blockOrNull, string[] dimensions, ILog log,
                AsConverterService asConverter,
                int compatibility = Constants.CompatibilityLevel10,
                Func<ServiceKit14> kit = null
                //Func<AdamManager> getAdamManager = null
            )
            {
                Dimensions = dimensions;
                LogOrNull = log;
                CompatibilityLevel = compatibility;
                BlockOrNull = blockOrNull;
                AsC = asConverter;
                _getKit = kit;
                //_getAdamManager = getAdamManager;
                return this;
            }

            internal IBlock BlockOrNull { get; private set; }

            internal string[] Dimensions { get; private set; }

            internal ILog LogOrNull { get; private set; }

            internal int CompatibilityLevel { get; private set; }

            internal AsConverterService AsC { get; private set; }

            /// <summary>
            /// The ValueConverter is used to parse links in the format like "file:72"
            /// </summary>
            [PrivateApi]
            internal IValueConverter ValueConverterOrNull => _valueConverterLazy.Value;

            private readonly LazySvc<IValueConverter> _valueConverterLazy;


            internal DataBuilder DataBuilder => _dataBuilderLazy.Value;
            private readonly LazySvc<DataBuilder> _dataBuilderLazy;


            internal IRenderService RenderService => _renderServiceGenerator.New();
            private readonly Generator<IRenderService> _renderServiceGenerator;

            internal ServiceKit14 Kit => _kit.Get(() => _getKit?.Invoke());
            private readonly GetOnce<ServiceKit14> _kit = new GetOnce<ServiceKit14>();
            private Func<ServiceKit14> _getKit;


            //internal AdamManager AdamManager => _adamManager.Get(() => _getAdamManager?.Invoke());
            //private readonly GetOnce<AdamManager> _adamManager = new GetOnce<AdamManager>();
            //private Func<AdamManager> _getAdamManager;
        }
    }
}
