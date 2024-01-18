using ToSic.Eav.ImportExport.Json.V1;

namespace ToSic.Sxc.Backend.Cms;

public class EntityInListDto
{
    public int Index;
    public int Id;
    public Guid Guid;
    public string Title;
    public string Type;
    public JsonType TypeWip;
}