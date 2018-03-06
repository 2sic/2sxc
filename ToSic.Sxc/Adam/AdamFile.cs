using ToSic.Eav.Apps.Assets;

namespace ToSic.SexyContent.Adam
{
    public class AdamFile : File, IAdamItem
    {
        public AdamBrowseContext AdamBrowseContext;

        /// <summary>
        /// Metadata for this file
        /// This is usually an entity which has additional information related to this file
        /// </summary>
        public DynamicEntity Metadata => AdamBrowseContext.GetFirstMetadata(Id, false);

        public bool HasMetadata => AdamBrowseContext.GetFirstMetadataEntity(Id, false) != null;

        public  string Url => AdamBrowseContext.GenerateWebPath(this);

        public string Type => Classification.TypeName(Extension);
        public string Name { get; internal set; }
    }
}