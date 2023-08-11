using System;
using System.Collections.Generic;
using System.Dynamic;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Lib.Logging;
using ToSic.Lib.Documentation;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Data
{
    [PrivateApi]
    public abstract partial class DynamicEntityBase : DynamicObject, IDynamicEntityBase, IPropertyLookup, ISxcDynamicObject, ICanDebug
    {
        protected DynamicEntityBase(CodeDataFactory cdf, bool strict)
        {
            Cdf = cdf;
            _strict = strict;
        }

        [PrivateApi]
        protected void CompleteSetup(IPropertyLookup data) 
            => _helper = new CodeEntityHelper(data, Cdf, _strict, () => Debug);

        [PrivateApi]
        internal CodeEntityHelper Helper => _helper ?? throw new Exception($"{nameof(Helper)} not ready, did you CompleteSetup()");
        private CodeEntityHelper _helper;

        internal SubDataFactory SubDataFactory => _subData ?? (_subData = new SubDataFactory(Cdf, _strict, Debug));
        private SubDataFactory _subData;

        [PrivateApi]
        // ReSharper disable once InconsistentNaming
        public CodeDataFactory Cdf { get; }

        private readonly bool _strict;

        public abstract PropReqResult FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path);

        #region Debug system

        /// <inheritdoc />
        public bool Debug { get; set; }


        [PrivateApi("Internal")]
        public abstract List<PropertyDumpItem> _Dump(PropReqSpecs specs, string path);

        #endregion
    }
}
