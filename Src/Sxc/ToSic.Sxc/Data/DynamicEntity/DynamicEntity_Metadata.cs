using ToSic.Eav.Metadata;
using ToSic.Lib.Documentation;

namespace ToSic.Sxc.Data
{
    public partial class DynamicEntity: IHasMetadata
    {
        /// <inheritdoc />
        public IMetadata Metadata => _metadata ?? (_metadata = new Metadata(Entity?.Metadata, Entity, _Services));
        private Metadata _metadata;

        /// <summary>
        /// Explicit implementation, so it's not really available on DynamicEntity, only when cast to IHasMetadata
        /// This is important, because it uses the same name "Metadata"
        /// </summary>
        [PrivateApi]
        IMetadataOf IHasMetadata.Metadata => Entity?.Metadata;
    }
}
