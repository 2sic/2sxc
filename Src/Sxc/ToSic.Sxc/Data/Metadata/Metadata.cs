using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.Data.PropertyLookup;
using ToSic.Eav.Metadata;
using ToSic.Lib.Documentation;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Data
{
    [PrivateApi("Hide implementation")]
    public class Metadata: DynamicEntity, IMetadata, IHasPropLookup
    {
        internal Metadata(IMetadataOf metadata, IEntity parentOrNull, CodeDataFactory cdf)
            : base(metadata, parentOrNull, "Metadata", Eav.Constants.TransientAppId, strict: false, cdf)
        {
            _metadata = metadata;
            //_propLookup = new PropLookupMetadata(this, Entity, base.PropertyLookup, () => Debug);
        }

        [PrivateApi("Hide this")]
        private readonly IMetadataOf _metadata;


        IMetadataOf IHasMetadata.Metadata => _metadata;

        public override IPropertyLookup PropertyLookup => _propLookup ?? (_propLookup = new PropLookupMetadata(this, Entity, base.PropertyLookup, () => Debug));
        private PropLookupMetadata _propLookup;


        public bool HasType(string type) => _metadata.HasType(type);

        public IEnumerable<IEntity> OfType(string type) => _metadata.OfType(type);

    }
}
