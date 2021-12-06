using System;
using System.Collections.Generic;
using System.Linq;
using ToSic.Eav.Apps;
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
            ContentTypeRuntime typeRead, AppState appState)
        {
            // attach original metadata assignment when creating a new one
            JsonEntity ent;
            if (bundle.Entity != null)
                ent = jsonSerializer.ToJson(bundle.Entity);
            else
            {
                ent = jsonSerializer.ToJson(ConstructEmptyEntity(appId, bundle.Header, typeRead));

                // only attach metadata, if no metadata already exists
                if (ent.For == null && bundle.Header?.For != null) ent.For = bundle.Header.For;
            }

            // new UI doesn't use this any more, reset it
            if (bundle.Header != null) bundle.Header.For = null;

            try
            {
                if (ent.For != null)
                {
                    var targetId = ent.For;
                    ent.For.Title = appState.FindTargetTitle(jsonSerializer.MetadataTargets.GetId(targetId.Target),
                        targetId.String ?? targetId.Guid?.ToString() ?? targetId.Number?.ToString());
                }
            }
            catch { /* ignore experimental */ }

            return ent;
        }

        internal static List<IContentType> UsedTypes(List<BundleIEntity> list, ContentTypeRuntime typeRead)
            => list.Select(i
                    // try to get the entity type, but if there is none (new), look it up according to the header
                    => i.Entity?.Type
                       ?? typeRead.Get(i.Header.ContentTypeName))
                .ToList();

        internal List<InputTypeInfo> GetNecessaryInputTypes(List<JsonContentType> contentTypes, ContentTypeRuntime typeRead)
        {
            var wrapLog = Log.Call<List<InputTypeInfo>>($"{nameof(contentTypes)}: {contentTypes.Count}");
            var fields = contentTypes
                .SelectMany(t => t.Attributes)
                .Select(a => a.InputType)
                .Distinct()
                .ToList();

            Log.Add("Found these input types to load: " + string.Join(", ", fields));

            var allInputType = typeRead.GetInputTypes();

            var found = allInputType
                .Where(it => fields.Contains(it.Type))
                .ToList();

            if (found.Count == fields.Count) Log.Add("Found all");
            else
            {
                Log.Add(
                    $"It seems some input types were not found. Needed {fields.Count}, found {found.Count}. Will try to log details for this.");
                try
                {
                    var notFound = fields.Where(field => found.All(fnd => fnd.Type != field));
                    Log.Add("Didn't find: " + string.Join(",", notFound));
                }
                catch (Exception)
                {
                    Log.Add("Ran into problems logging missing input types.");
                }
            }

            return wrapLog($"{found.Count}", found);
        }

        internal static IEntity ConstructEmptyEntity(int appId, ItemIdentifier header, ContentTypeRuntime typeRead)
        {
            var type = typeRead.Get(header.ContentTypeName);
            var ent = EntityBuilder.EntityTemplate(appId, header.Guid, header.EntityId, 0, type);
            return ent;
        }
    }
}
