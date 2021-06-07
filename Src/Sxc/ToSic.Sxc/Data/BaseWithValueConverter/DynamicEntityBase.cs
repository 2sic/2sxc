using System;
using System.Dynamic;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Data
{
    public abstract class DynamicEntityBase: DynamicObject
    {
        protected DynamicEntityBase(IBlock block, IServiceProvider serviceProvider, string[] dimensions)
        {
            Dimensions = dimensions;
            _serviceProviderOrNull = block?.Context?.ServiceProvider ?? serviceProvider;
        }

        [PrivateApi]
        public string[] Dimensions { get; }

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


    }
}
