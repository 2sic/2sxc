//using ToSic.Eav.Metadata;
//using ToSic.Lib.Helpers;

//namespace ToSic.Sxc.Data
//{
//    public partial class TypedItem: IHasMetadata
//    {
//        public IMetadata Metadata => _typedMd.Get(() => DynEntity.Metadata);
//        private readonly GetOnce<IMetadata> _typedMd = new GetOnce<IMetadata>();

//        IMetadataOf IHasMetadata.Metadata => Entity?.Metadata;

//    }
//}
