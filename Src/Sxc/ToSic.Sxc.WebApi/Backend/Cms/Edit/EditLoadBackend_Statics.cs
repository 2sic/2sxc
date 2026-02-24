using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.ImportExport.Json.Sys;
using ToSic.Eav.ImportExport.Json.V1;

namespace ToSic.Sxc.Backend.Cms;

partial class EditLoadBackend
{

    /// <summary>
    /// Get Serialized entity or create a new one, and assign metadata
    /// based on the header (if none already existed)
    /// </summary>
    /// <returns></returns>
    private JsonEntity GetSerializeAndMdAssignJsonEntity(int appId, BundleWithHeaderOptional<IEntity> bundle,
        JsonSerializer jsonSerializer, IAppReader appReader, IAppWorkCtx appSysCtx)
    {
        var l = Log.Fn<JsonEntity>();
        // attach original metadata assignment when creating a new one
        JsonEntity ent;
        if (bundle.Entity != null)
        {
            ent = jsonSerializer.ToJson(bundle.Entity, 1);

        }
        else
        {
            ent = jsonSerializer.ToJson(ConstructEmptyEntity(appId, bundle.Header!, appSysCtx), metadataDepth: 0);

            // only attach metadata, if no metadata already exists
            if (ent.For == null && bundle.Header?.For != null)
                ent = ent with { For = bundle.Header.For };
        }

        // new UI doesn't use this anymore, reset it
        if (bundle.Header != null)
            bundle.Header.For = null;

        try
        {
            if (ent.For != null)
            {
                var eFor = ent.For;
                // #TargetTypeIdInsteadOfTarget
                var targetType = eFor.TargetType != 0
                    ? eFor.TargetType
                    : jsonSerializer.MetadataTargets.GetId(eFor.Target!);
                ent = ent with
                {
                    For = eFor with
                    {
                        Title = appReader.FindTargetTitle(targetType, eFor.String ?? eFor.Guid?.ToString() ?? eFor.Number?.ToString()),
                    }
                };
            }
        }
        catch { /* ignore experimental */ }

        return l.Return(ent);
    }

    private List<IContentType> UsedTypes(List<BundleWithHeaderOptional<IEntity>> list, IAppWorkCtx appSysCtx)
        => list.Select(i
                // try to get the entity type, but if there is none (new), look it up according to the header
                => i.Entity?.Type
                   ?? appSysCtx.AppReader.GetContentType(i.Header!.ContentTypeName!))
            .ToList();

    private List<InputTypeInfo> GetNecessaryInputTypes(List<JsonContentType> contentTypes, IAppWorkCtxPlus appCtx)
    {
        var l = Log.Fn<List<InputTypeInfo>>($"{nameof(contentTypes)}: {contentTypes.Count}");
        var fields = contentTypes
            .SelectMany(t => t.AttributesSafe())
            .Select(a => a.InputType)
            .Distinct()
            .ToList();

        l.A("Found these input types to load: " + string.Join(", ", fields));

        var allInputType = inputTypes.New(appCtx).GetInputTypes();

        var found = allInputType
            .Where(it => fields.Contains(it.Type))
            .ToList();

        if (found.Count == fields.Count) 
            l.A("Found all");
        else
        {
            l.A($"It seems some input types were not found. Needed {fields.Count}, found {found.Count}. Will try to log details for this.");
            try
            {
                var notFound = fields.Where(field => found.All(fnd => fnd.Type != field));
                l.A("Didn't find: " + string.Join(",", notFound));
            }
            catch (Exception ex)
            {
                l.Ex(ex);
                l.A("Ran into problems logging missing input types.");
            }
        }

        return l.Return(found, $"{found.Count}");
    }

    private IEntity ConstructEmptyEntity(int appId, ItemIdentifier header, IAppWorkCtx appSysCtx)
    {
        var l = Log.Fn<IEntity>();
        var type = appSysCtx.AppReader.GetContentType(header.ContentTypeName!);
        var ent = entityAssembler.EmptyOfType(appId, header.Guid, header.EntityId, type);
        return l.Return(ent);
    }
}