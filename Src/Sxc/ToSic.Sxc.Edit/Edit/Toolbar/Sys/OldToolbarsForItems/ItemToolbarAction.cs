using System.Text.Json.Serialization;

namespace ToSic.Sxc.Edit.Toolbar.Sys;

internal class ItemToolbarAction(IEntity? entity = null) : EntityEditInfo(entity)
{
    [JsonIgnore(Condition = JsonIgnoreCondition.WhenWritingNull)]
    // ReSharper disable once InconsistentNaming
    public string? action { get; set; }

}