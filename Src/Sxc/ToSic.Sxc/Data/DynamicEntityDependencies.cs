using System;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Data
{
    /// <summary>
    /// Dependencies every DynamicEntity has.
    /// They are needed for language lookups and creating inner lists of other entities
    /// </summary>
    [PrivateApi("this should all stay internal and never be public")]
    public class DynamicEntityDependencies
    {
        internal DynamicEntityDependencies(IBlock block, IServiceProvider serviceProvider, string[] dimensions, int compatibility = 10)
        {
            Dimensions = dimensions;
            CompatibilityLevel = compatibility;
            Block = block;
            ServiceProviderOrNull = block?.Context?.ServiceProvider ?? serviceProvider;

        }
        
        internal IBlock Block { get; }


        public string[] Dimensions { get; }

        internal int CompatibilityLevel { get; }


        /// <summary>
        /// Very internal implementation - we need this to allow the IValueProvider to be created, and normally it's provided by the Block context.
        /// But in rare cases (like when the App.Resources is a DynamicEntity) it must be injected separately.
        /// </summary>
        internal readonly IServiceProvider ServiceProviderOrNull;

        /// <summary>
        /// The ValueConverter is used to parse links in the format like "file:72"
        /// </summary>
        [PrivateApi]
        internal IValueConverter ValueConverterOrNull
        {
            get
            {
                if (_valueConverterOrNull != null || _triedToBuildValueConverter) return _valueConverterOrNull;
                _triedToBuildValueConverter = true;
                return _valueConverterOrNull = ServiceProviderOrNull?.Build<IValueConverter>();
            }
        }
        private IValueConverter _valueConverterOrNull;
        private bool _triedToBuildValueConverter;

    }
}
