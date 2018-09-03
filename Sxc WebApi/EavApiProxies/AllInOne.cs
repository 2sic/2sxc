using System.Collections.Generic;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Configuration;
using ToSic.Eav.ImportExport.Json.Format;
using ToSic.Eav.WebApi.Formats;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
    public class AllInOne
    {
        public List<HeaderAndJsonEntity> Items;

        public List<JsonContentType> ContentTypes;

        public List<InputTypeInfo> InputTypes;

        public bool IsPublished = true;

        public bool DraftShouldBranch = false;

        public IEnumerable<Feature> Features;
    }

    public class HeaderAndJsonEntity
    {
        public ItemIdentifier Header { get; set; }
        public JsonEntity Entity { get; set; }
    }
}
