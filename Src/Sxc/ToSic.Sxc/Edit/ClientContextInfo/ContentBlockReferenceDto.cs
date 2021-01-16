using System;
using Newtonsoft.Json;
using ToSic.Eav.Apps.Enums;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Data;

namespace ToSic.Sxc.Edit.ClientContextInfo
{
    public class ContentBlockReferenceDto
    {
        /// <summary>
        /// Info how this item is edited (draft required / optional)
        /// </summary>
        [JsonProperty("publishingMode")]
        public string PublishingMode { get; }
        
        /// <summary>
        /// ID of the reference item
        /// </summary>
        [JsonProperty("id")]
        public int Id { get; }

        /// <summary>
        /// GUID of parent
        /// </summary>
        [JsonProperty("parentGuid")]
        public Guid? ParentGuid { get; }
        
        /// <summary>
        /// Field it's being referenced in
        /// </summary>
        [JsonProperty("parentField")]
        public string ParentField { get; }
        
        /// <summary>
        /// Index / sort-order, where this is in the list of content-blocks
        /// </summary>
        [JsonProperty("parentIndex")]
        public int ParentIndex { get; }
        
        /// <summary>
        /// If this should be regarded as part of page - relevant for page publishing features
        /// </summary>
        [JsonProperty("partOfPage")]
        public bool PartOfPage { get; }

        internal ContentBlockReferenceDto(IBlock contentBlock, PublishingMode publishingMode)
        {
            Id = contentBlock.ContentBlockId;
            
            // try to get more information about the block
            var specsEntity = (contentBlock as BlockFromEntity)?.Entity as EntityInBlock;

            ParentGuid = specsEntity?.Parent;
            ParentField = specsEntity?.Field;
            ParentIndex = specsEntity?.SortOrder ?? 0;
            PublishingMode = publishingMode.ToString();

            // if the CBID is the Mod-Id, then it's part of page
            PartOfPage = contentBlock.ParentId == contentBlock.ContentBlockId;
        }
    }

}
