using System;
using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Metadata;
using ToSic.Lib.Documentation;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Data
{
    [PrivateApi("Hide implementation")]
    internal partial class Metadata: DynamicEntity, IMetadata, IHasPropLookup
    {
        internal Metadata(IMetadataOf metadata, /*IEntity parentOrNull,*/ CodeDataFactory cdf)
            : base(metadata, /*parentOrNull*/null, "Metadata(virtual-field)", Eav.Constants.TransientAppId, strict: false, cdf)
        {
            _metadata = metadata;
        }
        IPropertyLookup IHasPropLookup.PropertyLookup => _propLookup ?? (_propLookup = new PropLookupMetadata(this, () => Debug));
        private PropLookupMetadata _propLookup;

        [PrivateApi]
        private CodeItemHelper ItemHelper => _itemHelper ?? (_itemHelper = new CodeItemHelper(GetHelper));
        private CodeItemHelper _itemHelper;

        [PrivateApi("Hide this")]
        private readonly IMetadataOf _metadata;


        IMetadataOf IHasMetadata.Metadata => _metadata;



        public bool HasType(string type) => _metadata.HasType(type);

        public IEnumerable<IEntity> OfType(string type) => _metadata.OfType(type);

        #region Properties from the interfaces which are not really supported

        public new bool IsDemoItem => false;

        public new ITypedItem Presentation => throw new NotSupportedException();

        #endregion
    }
}
