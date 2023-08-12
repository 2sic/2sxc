using System.Collections.Generic;
using System.Dynamic;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Lib.Logging;
using ToSic.Lib.Documentation;
using static ToSic.Eav.Parameters;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Data
{
    [PrivateApi]
    public abstract class DynamicEntityBase : DynamicObject, IDynamicEntityBase, IPropertyLookup, ISxcDynamicObject, ICanDebug, IHasPropLookup
    {
        protected DynamicEntityBase(CodeDataFactory cdf, bool strict)
        {
            Cdf = cdf;
            _strict = strict;
        }

        //[PrivateApi]
        //protected void CompleteSetup() => _helper = new GetAndConvertHelper(this, Cdf, _strict, () => Debug);
        [PrivateApi]
        public abstract IPropertyLookup PropertyLookup { get; }

        [PrivateApi]
        internal GetAndConvertHelper GetHelper => _helper ?? (_helper = new GetAndConvertHelper(this, Cdf, _strict, () => Debug));
        private GetAndConvertHelper _helper;

        internal SubDataFactory SubDataFactory => _subData ?? (_subData = new SubDataFactory(Cdf, _strict, Debug));
        private SubDataFactory _subData;

        [PrivateApi]
        // ReSharper disable once InconsistentNaming
        public CodeDataFactory Cdf { get; }

        private readonly bool _strict;

        #region IPropertyLookup & Debug system

        [PrivateApi]
        public abstract PropReqResult FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path);

        [PrivateApi("Internal")]
        public abstract List<PropertyDumpItem> _Dump(PropReqSpecs specs, string path);
        
        /// <inheritdoc />
        public bool Debug { get; set; }

        #endregion

        #region Get / Get<T>

        /// <inheritdoc/>
        public dynamic Get(string name) => GetHelper.Get(name);

        /// <inheritdoc/>
        // ReSharper disable once MethodOverloadWithOptionalParameter
        public dynamic Get(string name, string noParamOrder = Protector, string language = null, bool convertLinks = true, bool? debug = null)
            => GetHelper.Get(name, noParamOrder, language, convertLinks, debug);

        /// <inheritdoc/>
        public TValue Get<TValue>(string name)
            => GetHelper.Get<TValue>(name);

        /// <inheritdoc/>
        // ReSharper disable once MethodOverloadWithOptionalParameter
        public TValue Get<TValue>(string name, string noParamOrder = Protector, TValue fallback = default)
            => GetHelper.Get(name, noParamOrder, fallback);

        #endregion
    }
}
