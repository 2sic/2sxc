using Newtonsoft.Json;
using static Newtonsoft.Json.NullValueHandling;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.Edit.Toolbar
{
    internal class ItemToolbarAction: EntityEditInfo
    {
        public ItemToolbarAction(IEntity entity = null): base(entity)
        {
        }

        [JsonProperty(NullValueHandling = Ignore)] public string action { get; set; }

    }
}