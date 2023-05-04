using System;
using ToSic.Eav.ImportExport.Json.V1;

namespace ToSic.Sxc.WebApi.ItemLists
{
    public class EntityInListDto
    {
        public int Index;
        public int Id;
        public Guid Guid;
        public string Title;
        public string Type;
        public JsonType TypeWip;
    }
}
