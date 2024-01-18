using System.Text.Json.Serialization;
using IEntity = ToSic.Eav.Data.IEntity;

namespace ToSic.Sxc.Edit.Toolbar;

internal class ItemToolbarAction(IEntity entity = null) : EntityEditInfo(entity)
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)] public string action { get; set; }

}