using System;
using DotNetNuke.Services.FileSystem;

namespace ToSic.SexyContent.Adam
{
    public class AdamFile : FileInfo, IAdamItem
    {
        public EntityBase EntityBase;

        public new DateTime CreatedOnDate;

        /// <summary>
        /// Metadata for this file
        /// This is usually an entity which has additional information related to this file
        /// </summary>
        public DynamicEntity Metadata => EntityBase.GetFirstMetadata(FileId, false);

        public bool HasMetadata => EntityBase.GetFirstMetadataEntity(FileId, false) != null;

        public  string Url => EntityBase.GenerateWebPath(this);

        public string Type => EntityBase.TypeName(Extension);
        public string Name { get; internal set; }
    }
}