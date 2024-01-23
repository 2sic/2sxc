using System.Text.Json.Serialization;
using ToSic.Sxc.Data.Internal.Decorators;
using IEntity = ToSic.Eav.Data.IEntity;
// ReSharper disable InconsistentNaming

namespace ToSic.Sxc.Edit.Toolbar;

internal class EntityEditInfo
{
    public EntityEditInfo(IEntity entity = null)
    {
        // a null value/missing is also valid, when all you want is a new/add toolbar
        if (entity == null)
            return;

        isPublished = entity.IsPublished;
        title = entity.GetBestTitle();
        entityGuid = entity.EntityGuid;

        var editDecorator = entity.GetDecorator<EntityInBlockDecorator>();
        if (editDecorator != null)
        {
            sortOrder = editDecorator.SortOrder;
            if (editDecorator.ParentGuid == null)
            {
                useModuleList = true;
            }
            else 
                // only set parent if not empty - as it's always a valid int and wouldn't be null
            {
                parent = editDecorator.ParentGuid;
                fields = editDecorator.Field;
                entityId = entity.EntityId;
                contentType = entity.Type.Name;
            }
        }
        else
        {
            entityId = entity.EntityId;
            contentType = entity.Type.Name;
        }
    }

    internal const string KeyIndex = "sortOrder";
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public int? sortOrder { get; set; }

    internal const string KeyUseModule = "useModuleList";
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public bool? useModuleList { get; set; }

    internal const string KeyPublished = "isPublished";
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public bool? isPublished { get; set; }

    internal const string KeyEntityId = "entityId";
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public int? entityId { get; set; }

    internal const string KeyContentType = "contentType";
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public string contentType { get; set; }

    internal const string KeyPrefill = "prefill";
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public object prefill { get; set; }

    internal const string KeyTitle = "title";
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public string title { get; set; }

    internal const string KeyEntityGuid = "entityGuid";
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public Guid? entityGuid { get; set; }

    internal const string KeyParent = "parent";
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public Guid? parent { get; set; }

    internal const string KeyFields = "fields";
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public string fields { get; set; }

    internal static string[] KeysOfLists = [KeyIndex, KeyUseModule, KeyParent, KeyFields];
}