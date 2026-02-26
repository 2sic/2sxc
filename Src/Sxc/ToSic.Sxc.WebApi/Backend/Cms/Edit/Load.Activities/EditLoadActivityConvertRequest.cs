using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.Data.Build.Sys;
using ToSic.Eav.ImportExport.Json.Sys;
using ToSic.Eav.ImportExport.Json.V1;
using ToSic.Eav.Serialization.Sys;

namespace ToSic.Sxc.Backend.Cms.Load.Activities;

public class EditLoadActivityConvertRequest(Generator<JsonSerializer> jsonSerializerGenerator, EntityAssembler entityAssembler)
    : ServiceBase("UoW.AddCtx", connect: [jsonSerializerGenerator, entityAssembler])
{
    public EditLoadDto Run(List<BundleWithHeaderOptional<IEntity>> list, EditLoadActContextWithWork actionCtx)
    {
        var l = Log.Fn<EditLoadDto>();

        var jsonSerializer = jsonSerializerGenerator.New().SetApp(actionCtx.AppReader);

        var result = new EditLoadDto
        {
            Items = list
                .Select(bundle => new BundleWithHeaderOptional<JsonEntity>
                {
                    Header = bundle.Header,
                    Entity = GetSerializeAndMdAssignJsonEntity(actionCtx.AppId, bundle, jsonSerializer, actionCtx.AppReader, actionCtx.AppWorkCtx)
                })
                .ToList(),
        };

        // set published if some data already exists
        if (list.Any())
        {
            var entity = list.First().Entity;
            var isPublished = entity?.IsPublished ?? true; // Entity could be null (new), then true
            result = result with
            {
                IsPublished = isPublished,
                // only set draft-should-branch if this draft already has a published item
                DraftShouldBranch = !isPublished && (actionCtx.AppReader.GetPublished(entity)) != null
            };
        }
        return l.Return(result);
    }

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
    private IEntity ConstructEmptyEntity(int appId, ItemIdentifier header, IAppWorkCtx appSysCtx)
    {
        var l = Log.Fn<IEntity>();
        var type = appSysCtx.AppReader.GetContentType(header.ContentTypeName!);
        var ent = entityAssembler.EmptyOfType(appId, header.Guid, header.EntityId, type);
        return l.Return(ent);
    }

}
