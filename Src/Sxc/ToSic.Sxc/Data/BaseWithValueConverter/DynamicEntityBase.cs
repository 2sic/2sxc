using System;
using System.Collections.Generic;
using System.Dynamic;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Data
{
    public abstract class DynamicEntityBase: DynamicObject
    {
        protected DynamicEntityBase(IBlock block, IServiceProvider serviceProvider, string[] dimensions, int compatibility)
        {
            Dimensions = dimensions;
            CompatibilityLevel = compatibility;
            Block = block;
            _serviceProviderOrNull = block?.Context?.ServiceProvider ?? serviceProvider;
        }
        
        //[PrivateApi("Try to replace existing list of shared properties")]
        //internal DynamicEntityDependencies Deps { get; }

        [PrivateApi("Keep internal only - should never surface")]
        internal IBlock Block { get; }


        [PrivateApi]
        public string[] Dimensions { get; }

        [PrivateApi]
        public int CompatibilityLevel { get; }


        /// <summary>
        /// Very internal implementation - we need this to allow the IValueProvider to be created, and normally it's provided by the Block context.
        /// But in rare cases (like when the App.Resources is a DynamicEntity) it must be injected separately.
        /// </summary>
        [PrivateApi]
        protected readonly IServiceProvider _serviceProviderOrNull;

        [PrivateApi]
        protected IValueConverter ValueConverterOrNull
        {
            get
            {
                if (_valueConverterOrNull != null || _triedToBuildValueConverter) return _valueConverterOrNull;
                _triedToBuildValueConverter = true;
                return _valueConverterOrNull = _serviceProviderOrNull?.Build<IValueConverter>();
            }
        }
        private IValueConverter _valueConverterOrNull;
        private bool _triedToBuildValueConverter;

        protected object ValueAutoConverted(Tuple<object, string> resultSet, bool lookup, IEntity entity, string field, string[] dimensions)
        {
            var result = resultSet.Item1;

            // New mechanism to not use resolve-hyperlink
            if (lookup && result is string strResult && ValueConverterBase.CouldBeReference(strResult) &&
                resultSet.Item2 == DataTypes.Hyperlink)
                result = ValueConverterOrNull?.ToValue(strResult, entity.EntityGuid) ?? result;

            if (result is IEnumerable<IEntity> rel)
                // Note: if it's a Dynamic Entity without block (like App.Settings) it needs the Service Provider from this object to work
                result = new DynamicEntityWithList(entity, field, rel, dimensions, CompatibilityLevel, Block,
                    _serviceProviderOrNull);

            return result;
        }

    }
}
