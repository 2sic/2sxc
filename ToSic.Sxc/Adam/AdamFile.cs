namespace ToSic.SexyContent.Adam
{
    public class AdamFile : FileInfo, IAdamItem
    {
        public AdamBrowseContext AdamBrowseContext;

        /// <summary>
        /// Metadata for this file
        /// This is usually an entity which has additional information related to this file
        /// </summary>
        public DynamicEntity Metadata => AdamBrowseContext.GetFirstMetadata(FileId, false);

        public bool HasMetadata => AdamBrowseContext.GetFirstMetadataEntity(FileId, false) != null;

        public  string Url => AdamBrowseContext.GenerateWebPath(this);

        public string Type => AdamBrowseContext.TypeName(Extension);
        public string Name { get; internal set; }
    }
}