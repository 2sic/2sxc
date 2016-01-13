using DotNetNuke.Services.FileSystem;

namespace ToSic.SexyContent.Adam
{
    public class AdamFile : FileInfo
    {
        public Core Core;

        /// <summary>
        /// Metadata for this file
        /// This is usually an entity which has additional information related to this file
        /// </summary>
        public DynamicEntity Metadata => Core.GetFirstMetadata(FileId, false);

        public bool HasMetadata => Core.GetFirstMetadataEntity(FileId, false) != null;

        public  string Url => Core.GenerateWebPath(this);
    }
}