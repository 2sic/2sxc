using System;

namespace ToSic.SexyContent.Edit.ClientContextInfo
{
    public abstract class ClientInfoEntity
    {
        public int ZoneId;  // the zone of the content-block
        public int AppId;   // the zone of the content-block
        public Guid Guid;   // the entity-guid of the content-block
        public int Id;      // the entity-id of the content-block
    }
}
