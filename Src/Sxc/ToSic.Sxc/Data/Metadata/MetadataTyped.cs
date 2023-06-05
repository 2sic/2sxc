using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.Metadata;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data
{
    [PrivateApi("Hide implementation")]
    public class MetadataTyped: TypedItem, IMetadataTyped
    {
        internal MetadataTyped(IDynamicMetadata dynamicMetadata, TypedItem.MyHelpers helpers): base(dynamicMetadata, helpers)
        {
            _dynamicMetadata = dynamicMetadata;
        }
        private readonly IDynamicMetadata _dynamicMetadata;

        IMetadataOf IHasMetadata.Metadata => ((IHasMetadata)_dynamicMetadata).Metadata;

        public bool HasType(string typeName) => _dynamicMetadata.HasType(typeName);

        public IEnumerable<IEntity> OfType(string typeName) => _dynamicMetadata.OfType(typeName);
    }
}
