using System;
using Newtonsoft.Json;
using ToSic.Eav.Data;
using ToSic.Sxc.Data;
using static Newtonsoft.Json.NullValueHandling;
using IEntity = ToSic.Eav.Data.IEntity;
// ReSharper disable InconsistentNaming

namespace ToSic.Sxc.Edit.Toolbar
{
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
            //if (entity is IHasEditingData editingData)
            if (editDecorator != null)
            {
                sortOrder = editDecorator.SortOrder;
                if (editDecorator.Parent == null)
                {
                    useModuleList = true;
                }
                else 
                // only set parent if not empty - as it's always a valid int and wouldn't be null
                {
                    parent = editDecorator.Parent;
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
        [JsonProperty(NullValueHandling = Ignore)] public int? sortOrder { get; set; }

        internal const string KeyUseModule = "useModuleList";
        [JsonProperty(NullValueHandling = Ignore)] public bool? useModuleList { get; set; }

        internal const string KeyPublished = "isPublished";
        [JsonProperty(NullValueHandling = Ignore)] public bool? isPublished { get; set; }

        internal const string KeyEntityId = "entityId";
        [JsonProperty(NullValueHandling = Ignore)] public int? entityId { get; set; }

        internal const string KeyContentType = "contentType";
        [JsonProperty(NullValueHandling = Ignore)] public string contentType { get; set; }

        internal const string KeyPrefill = "prefill";
        [JsonProperty(NullValueHandling = Ignore)] public object prefill { get; set; }

        internal const string KeyTitle = "title";
        [JsonProperty(NullValueHandling = Ignore)] public string title { get; set; }

        internal const string KeyEntityGuid = "entityGuid";
        [JsonProperty(NullValueHandling = Ignore)] public Guid? entityGuid { get; set; }

        internal const string KeyParent = "parent";
        [JsonProperty(NullValueHandling = Ignore)] public Guid? parent { get; set; }

        internal const string KeyFields = "fields";
        [JsonProperty(NullValueHandling = Ignore)] public string fields { get; set; }

    }
}