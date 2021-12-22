using System.Collections.Generic;
using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Eav.Metadata;

namespace ToSic.Sxc.Data
{
    [PrivateApi("Hide implementation")]
    public class DynamicMetadata: DynamicEntity, IDynamicMetadata
    {
        internal DynamicMetadata(IMetadataOf metadata, IEntity parentOrNull, DynamicEntityDependencies dependencies)
            : base(metadata, parentOrNull, "Metadata", Eav.Constants.TransientAppId, dependencies)
        {
            _metadata = metadata;
        }

        [PrivateApi("Hide this")]
        private readonly IMetadataOf _metadata;

        IMetadataOf IHasMetadata.Metadata => _metadata;

        public bool HasType(string typeName) => _metadata.HasType(typeName);

        public IEnumerable<IEntity> OfType(string typeName) => _metadata.OfType(typeName);
    }
}
