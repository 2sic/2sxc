using System.Text.Json.Serialization;
using ToSic.Eav.Cms.Internal;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Data.Internal.Decorators;

namespace ToSic.Sxc.Web.Internal.JsContextEdit;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class ContentBlockReferenceDto
{
    /// <summary>
    /// Info how this item is edited (draft required / optional)
    /// </summary>
    [JsonPropertyName("publishingMode")]
    public string PublishingMode { get; }
        
    /// <summary>
    /// ID of the reference item
    /// </summary>
    [JsonPropertyName("id")]
    public int Id { get; }

    /// <summary>
    /// GUID of parent
    /// </summary>
    [JsonPropertyName("parentGuid")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public Guid? ParentGuid { get; }
        
    /// <summary>
    /// Field it's being referenced in
    /// </summary>
    [JsonPropertyName("parentField")]
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    public string ParentField { get; }
        
    /// <summary>
    /// Index / sort-order, where this is in the list of content-blocks
    /// </summary>
    [JsonPropertyName("parentIndex")]
    public int ParentIndex { get; }
        
    /// <summary>
    /// If this should be regarded as part of page - relevant for page publishing features
    /// </summary>
    [JsonPropertyName("partOfPage")]
    public bool PartOfPage { get; }

    internal ContentBlockReferenceDto(IBlock contentBlock, PublishingMode publishingMode)
    {
        Id = contentBlock.ContentBlockId;
            
        // if the CBID is the Mod-Id, then it's part of page
        PartOfPage = contentBlock.ParentId == contentBlock.ContentBlockId;
            
        PublishingMode = publishingMode.ToString();
            
        // try to get more information about the block
        var decorator = (contentBlock as BlockFromEntity)?.Entity.GetDecorator<EntityInListDecorator>();
        if (decorator == null) return;
        ParentGuid = decorator.ParentGuid;
        ParentField = decorator.Field;
        ParentIndex = decorator.SortOrder;
    }
}