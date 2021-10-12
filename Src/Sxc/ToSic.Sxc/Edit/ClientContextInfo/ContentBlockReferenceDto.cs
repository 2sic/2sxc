using System;
using Newtonsoft.Json;
using ToSic.Eav.Apps.Enums;
using ToSic.Eav.Data;
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
        [JsonProperty("parentGuid", NullValueHandling = NullValueHandling.Ignore)]
        public Guid? ParentGuid { get; }
        
        /// <summary>
        /// Field it's being referenced in
        /// </summary>
        [JsonProperty("parentField", NullValueHandling = NullValueHandling.Ignore)]
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
            
            // if the CBID is the Mod-Id, then it's part of page
            PartOfPage = contentBlock.ParentId == contentBlock.ContentBlockId;
            
            PublishingMode = publishingMode.ToString();
            
            // try to get more information about the block
            //if (!((contentBlock as BlockFromEntity)?.Entity is IHasDecorator<IEntity> specsEntity)) return;
            var decorator = (contentBlock as BlockFromEntity)?.Entity.GetDecorator<EntityInListDecorator>();
            if (decorator == null) return;
            ParentGuid = decorator.Parent;
            ParentField = decorator.Field;
            ParentIndex = decorator.SortOrder;
        }
    }

}
