using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using ToSic.Eav.Conversion;
using ToSic.Eav.Data;
using ToSic.Eav.DataSources;
using ToSic.Sxc.Conversion;

namespace ToSic.Sxc.Dnn.WebApi.App
{
    internal class AppContentJsonForInstance
    {
        internal string GeneratePleaseEnableDataError(int instanceId)
            =>
                $"2sxc Content ({instanceId}): " +
                "A module (content-block) is trying to retrieve data from the server as JSON. " +
                "If you see this message, it is because Data Publishing is not enabled on the appropriate view. " +
                $"Please enable it in the view settings. \\nThis is happening on the module {instanceId}.";

        /// <summary>
        /// Returns a JSON string for the elements
        /// </summary>
        internal string GenerateJson(IDataSource source, string[] streamsToPublish, bool userMayEdit)
        {
#pragma warning disable 612
#pragma warning disable 618
            var ser = new OldContentBlockJsonSerialization(Eav.Factory.ObsoleteBuild<EntitiesToDictionaryBase.Dependencies>(), userMayEdit);
#pragma warning restore 618
#pragma warning restore 612

            var y = streamsToPublish
                .Where(k => source.Out.ContainsKey(k))
                .ToDictionary(k => k, s => new
                {
                    List = source.Out[s].List.Select(c => ser.PrepareOldFormat(c)).ToList()
                });

            return JsonConvert.SerializeObject(y);
        }


        private class OldContentBlockJsonSerialization : DataToDictionary
        {
            public OldContentBlockJsonSerialization(Dependencies dependencies, bool withEdit) : base(dependencies)
            {
                WithEdit = withEdit;
            }

            internal Dictionary<string, object> PrepareOldFormat(IEntity entity)
            {
                var dicNew = GetDictionaryFromEntity(entity);
                var dicToSerialize = ConvertNewSerRelToOldSerRel(dicNew);

                dicToSerialize.Add(Constants.JsonEntityIdNodeName, entity.EntityId);

                return dicToSerialize;
            }


            private Dictionary<string, object> ConvertNewSerRelToOldSerRel(IDictionary<string, object> dicNew)
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
            // 'Id' and 'Title'
            // ReSharper disable once MemberCanBePrivate.Local
            public class SerializableRelationshipOld
            {
                public int? EntityId;
                public object EntityTitle;
            }
        }
    }
}