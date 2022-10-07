using System.Text.Json.Serialization;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.Edit.Toolbar
{
    internal class ItemToolbarAction: EntityEditInfo
    {
        public ItemToolbarAction(IEntity entity = null): base(entity)
        {
        }

        [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public string action { get; set; }

    }
}