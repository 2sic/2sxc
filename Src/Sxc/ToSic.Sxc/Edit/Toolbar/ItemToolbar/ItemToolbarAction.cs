using System;
using Newtonsoft.Json;
using ToSic.Eav.Data;
using ToSic.Sxc.Data;
using static Newtonsoft.Json.NullValueHandling;
using IEntity = ToSic.Eav.Data.IEntity;
// ReSharper disable InconsistentNaming

namespace ToSic.Sxc.Edit.Toolbar
{
    internal class ItemToolbarAction
    {
        public ItemToolbarAction(IEntity entity = null)
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


        [JsonProperty(NullValueHandling = Ignore)] public int? sortOrder { get; set; }

        [JsonProperty(NullValueHandling = Ignore)] public bool? useModuleList { get; set; }

        [JsonProperty(NullValueHandling = Ignore)] public bool? isPublished { get; set; }

        [JsonProperty(NullValueHandling = Ignore)] public int? entityId { get; set; }

        [JsonProperty(NullValueHandling = Ignore)] public string contentType { get; set; }

        [JsonProperty(NullValueHandling = Ignore)] public string action { get; set; }

        [JsonProperty(NullValueHandling = Ignore)] public object prefill { get; set; }

        [JsonProperty(NullValueHandling = Ignore)] public string title { get; set; }

        [JsonProperty(NullValueHandling = Ignore)] public Guid? entityGuid { get; set; }

        /// <summary>
        /// Experimental 11.01
        /// </summary>
        [JsonProperty(NullValueHandling = Ignore)] public Guid? parent { get; set; }

        /// <summary>
        /// Experimental 11.01
        /// </summary>
        [JsonProperty(NullValueHandling = Ignore)] public string fields { get; set; }

        [JsonIgnore]
        public string Json => JsonConvert.SerializeObject(this);

    }
}