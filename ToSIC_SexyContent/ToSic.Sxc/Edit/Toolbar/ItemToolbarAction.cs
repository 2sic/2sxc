using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using ToSic.Eav.Interfaces;
using ToSic.SexyContent.Interfaces;
using ToSic.Sxc.Interfaces;

namespace ToSic.Sxc.Edit.Toolbar
{
    [SuppressMessage("ReSharper", "InconsistentNaming")]
    internal class ItemToolbarAction
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? sortOrder { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? useModuleList { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public bool? isPublished { get; set; }
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public int? entityId { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string contentType { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public string action { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        public object prefill { get; set; }

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
                useModuleList = true;
            }
            else
            {
                entityId = Entity.EntityId;
                contentType = Entity.Type.Name;
            }
        }


        [JsonIgnore]
        public string Json => JsonConvert.SerializeObject(this);

    }
}