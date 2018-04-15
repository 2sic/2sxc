using System.Collections.Generic;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.ImportExport.Json.Format;
using ToSic.Eav.WebApi.Formats;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
    public class AllInOne
    {
        public List<EntityWithHeader2> Items;

        public List<JsonContentType> ContentTypes;

        public List<InputTypeInfo> InputTypes;
    }

    public class EntityWithHeader2
    {
        public ItemIdentifier Header { get; set; }
        public JsonEntity Entity { get; set; }
    }
}
