using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Conversion;
using ToSic.Eav.Data;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Compatibility
{
    [Obsolete]
    public class OldContentBlockJsonSerialization: Serializers.Serializer
    {
        public OldContentBlockJsonSerialization(ICmsBlock cmsInstance): base(cmsInstance)
        { }

        internal Dictionary<string, object> PrepareOldFormat(IEntity entity)
        {
            // var ser = new Serializer(SxcInstance, _dimensions);
            var dicNew = GetDictionaryFromEntity(entity);
            var dicToSerialize = ConvertNewSerRelToOldSerRel(dicNew);

            dicToSerialize.Add(Constants.JsonEntityIdNodeName, entity.EntityId);

            return dicToSerialize;
        }


        internal Dictionary<string, object> ConvertNewSerRelToOldSerRel(Dictionary<string, object> dicNew)
        {
            // find all items which are of type List<SerializableRelationship>
            // then convert to EntityId and EntityTitle to conform to "old" format
            var dicToSerialize = new Dictionary<string, object>();
            foreach (string key in dicNew.Keys)
            {
                var list = dicNew[key] as List<RelationshipReference>;
                dicToSerialize.Add(key,
                    list?.Select(p => new SerializableRelationshipOld() { EntityId = p.Id, EntityTitle = p.Title }).ToList() ??
                    dicNew[key]);
            }
            return dicToSerialize;
        }

        // Helper to provide old interface with "EntityId" and "EntityTitle" instead of 
        // "Id" and "Title"
        public class SerializableRelationshipOld
        {
            public int? EntityId;
            public object EntityTitle;
        }
    }
}
