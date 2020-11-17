using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps.Parts;
using ToSic.Eav.Data;
using ToSic.Eav.Data.Builder;
using ToSic.Eav.ImportExport.Json;
using ToSic.Eav.ImportExport.Json.V1;
using ToSic.Eav.WebApi.Formats;

namespace ToSic.Sxc.WebApi.Cms
{
    public partial class EditLoadBackend
    {

        /// <summary>
        /// Get Serialized entity or create a new one, and assign metadata
        /// based on the header (if none already existed)
        /// </summary>
        /// <param name="appId"></param>
        /// <param name="bundle"></param>
        /// <param name="jsonSerializer"></param>
        /// <param name="typeRead"></param>
        /// <returns></returns>
        internal static JsonEntity GetSerializeAndMdAssignJsonEntity(int appId, BundleIEntity bundle, JsonSerializer jsonSerializer,
            ContentTypeRuntime typeRead)
        {
            // attach original metadata assignment when creating a new one
            JsonEntity ent;
            if (bundle.Entity != null)
                ent = jsonSerializer.ToJson(bundle.Entity);
            else
            {
                ent = jsonSerializer.ToJson(ConstructEmptyEntity(appId, bundle.Header, typeRead));

                // only attach metadata, if no metadata already exists
                if (ent.For == null)
                {
                    if (bundle.Header?.For != null)
                        ent.For = bundle.Header.For;

                    // #2134
                    // if we have "old" Metadata headers, attach these
                    //else if (bundle.Header?.Metadata != null)
                    //{
                    //    var md = bundle.Header.Metadata;
                    //    ent.For = new JsonMetadataFor
                    //    {
                    //        Guid = md.KeyGuid,
                    //        String = md.KeyString,
                    //        Number = md.KeyNumber,
                    //        Target = jsonSerializer.MetadataProvider.GetName(md.TargetType)
                    //    };
                    //}
                }
            }

            // new UI doesn't use this any more, reset it
            if (bundle.Header != null)
            {
                // #2134
                //bundle.Header.Metadata = null;
                bundle.Header.For = null;
            }
            return ent;
        }

        internal static List<IContentType> UsedTypes(List<BundleIEntity> list, ContentTypeRuntime typeRead)
            => list.Select(i
                    // try to get the entity type, but if there is none (new), look it up according to the header
                    => i.Entity?.Type
                       ?? typeRead.Get(i.Header.ContentTypeName))
                .ToList();

        internal static List<InputTypeInfo> GetNecessaryInputTypes(List<JsonContentType> types, ContentTypeRuntime typeRead)
        {
            var fields = types
                .SelectMany(t => t.Attributes)
                .Select(a => a.InputType)
                .Distinct();
            return typeRead.GetInputTypes()
                .Where(it => fields.Contains(it.Type))
                .ToList();
        }

        internal static IEntity ConstructEmptyEntity(int appId, ItemIdentifier header, ContentTypeRuntime typeRead)
        {
            var type = typeRead.Get(header.ContentTypeName);
            var ent = EntityBuilder.EntityWithAttributes(appId, header.Guid, header.EntityId, 0, type);
            return ent;
        }
    }
}
