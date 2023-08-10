using System.Collections.Generic;
using System.Dynamic;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Debug;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Lib.Logging;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data
{
    [PrivateApi]
    public abstract partial class DynamicEntityBase : DynamicObject, IDynamicEntityBase, IPropertyLookup, ISxcDynamicObject, ICanDebug
    {
        protected DynamicEntityBase(CodeDataFactory cdf, bool strict)
        {
            _Cdf = cdf;
            Helper = new CodeEntityHelper(this, cdf, strict);
        }

        [PrivateApi]
        internal CodeEntityHelper Helper { get; }

        [PrivateApi]
        // ReSharper disable once InconsistentNaming
        public CodeDataFactory _Cdf { get; }

        public abstract PropReqResult FindPropertyInternal(PropReqSpecs specs, PropertyLookupPath path);

        #region Debug system

        /// <inheritdoc />
        public bool Debug { get; set; }


        [PrivateApi("Internal")]
        public abstract List<PropertyDumpItem> _Dump(PropReqSpecs specs, string path);

        #endregion
    }
}
