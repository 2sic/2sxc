using System;
using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using ToSic.Sxc.Interfaces;
using static Newtonsoft.Json.NullValueHandling;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.Edit.Toolbar
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class ItemToolbarAction
    {
        public ItemToolbarAction(IEntity dynEntityOrEntity = null)
        {
            // a null value/missing is also valid, when all you want is a new/add toolbar
            if (dynEntityOrEntity == null)
                return;

            var Entity = dynEntityOrEntity;
            isPublished = Entity.IsPublished;
            if (Entity is IHasEditingData editingData)
            {
                sortOrder = editingData.SortOrder;
                if (editingData.Parent == null)
                {
                    useModuleList = true;
                }
                else 
                // only set parent if not empty - as it's always a valid int and wouldn't be null
                {
                    parent = editingData.Parent;
                    fields = editingData.Fields;
                    entityId = Entity.EntityId;
                    contentType = Entity.Type.Name;
                }
            }
            else
            {
                entityId = Entity.EntityId;
                contentType = Entity.Type.Name;
            }
        }


        [JsonProperty(NullValueHandling = Ignore)] public int? sortOrder { get; set; }

        [JsonProperty(NullValueHandling = Ignore)] public bool? useModuleList { get; set; }

        [JsonProperty(NullValueHandling = Ignore)] public bool? isPublished { get; set; }

        [JsonProperty(NullValueHandling = Ignore)] public int? entityId { get; set; }

        [JsonProperty(NullValueHandling = Ignore)] public string contentType { get; set; }

        [JsonProperty(NullValueHandling = Ignore)] public string action { get; set; }

        [JsonProperty(NullValueHandling = Ignore)] public object prefill { get; set; }


        /// <summary>
        /// Experimental 10.27
        /// </summary>
        [JsonProperty(NullValueHandling = Ignore)] public Guid? parent { get; set; }

        /// <summary>
        /// Experimental 10.27
        /// </summary>
        [JsonProperty(NullValueHandling = Ignore)] public string fields { get; set; }

        [JsonIgnore]
        public string Json => JsonConvert.SerializeObject(this);

    }
}