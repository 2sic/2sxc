using System.Collections.Generic;
using ToSic.Eav.WebApi.Formats;

namespace ToSic.SexyContent.WebApi.EavApiProxies
{
    public class AllInOne
    {
        public List<EntityWithHeader2> Items;

        public List<dynamic> ContentTypes;

        public List<dynamic> InputTypes;
    }

    public class EntityWithHeader2
    {
        public ItemIdentifier Header { get; set; }
        public dynamic Entity { get; set; }
    }
}
