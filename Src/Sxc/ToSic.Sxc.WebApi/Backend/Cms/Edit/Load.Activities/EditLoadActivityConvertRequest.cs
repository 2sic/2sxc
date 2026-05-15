using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.Data.Build.Sys;
using ToSic.Eav.Data.Processing;
using ToSic.Eav.ImportExport.Json.Sys;
using ToSic.Eav.ImportExport.Json.V1;
using ToSic.Eav.Serialization.Sys;
using ToSic.Eav.WebApi.Sys.Entities;

namespace ToSic.Sxc.Backend.Cms.Load.Activities;

public class EditLoadActivityConvertRequest(Generator<JsonSerializer> jsonSerializerGenerator, EntityAssembler entityAssembler)
    : ServiceBase("UoW.AddCtx", connect: [jsonSerializerGenerator, entityAssembler]),
        ILowCodeAction<List<BundleWithHeaderOptional<IEntity>>, EditLoadDto>
{
    public async Task<ActionData<EditLoadDto>> Run(LowCodeActionContext actionCtx, ActionData<List<BundleWithHeaderOptional<IEntity>>> data)
    {
        var l = Log.Fn<EditLoadDto>();

        var appReader = actionCtx.Get<IAppReader>(EditLoadContextConstants.AppReader);
        var appWorkCtxPlus = actionCtx.Get<IAppWorkCtxPlus>(EditLoadContextConstants.AppCtxWork);
        var jsonSerializer = jsonSerializerGenerator.New().SetApp(appReader);

        var list = data.Data;
        var result = new EditLoadDto
        {
            Items = list
                .Select(bundle => new BundleWithHeaderOptional<JsonEntity>
                {
                    Header = bundle.Header,
                    Entity = GetSerializeAndMdAssignJsonEntity(actionCtx.Get<int>("AppId"), bundle, jsonSerializer, appReader, appWorkCtxPlus)
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
                DraftShouldBranch = !isPublished && (appReader.GetPublished(entity)) != null
            };
        }
        return ActionData.Create(l.Return(result));
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
        var ent = GetJsonEntityOrCreateEmpty();

        // new UI doesn't use this anymore, reset it
        bundle.Header?.For = null;

        // If entity is not for something, we're done...
        if (ent.For == null)
            return l.Return(ent, "done, no 'For'");
        
        // ...otherwise we must convert older 'For' signatures
        try
        {
            var eFor = ent.For;
            // #TargetTypeIdInsteadOfTarget
            var targetType = eFor.TargetType != 0
                ? eFor.TargetType
                : jsonSerializer.MetadataTargets.GetId(eFor.Target!);
            var newTitle = appReader.FindTargetTitle(targetType, eFor.String ?? eFor.Guid?.ToString() ?? eFor.Number?.ToString());
            ent = ent with { For = eFor with { Title = newTitle } };
        }
        catch { /* ignore experimental */ }

        return l.Return(ent);

        // Quick helper to get the JsonEntity, or create an empty one if no entity exists in the bundle
        JsonEntity GetJsonEntityOrCreateEmpty()
        {
            if (bundle.Entity != null)
                return jsonSerializer.ToJson(bundle.Entity, 1);

            var emptyEntity = ConstructEmptyEntity(appId, bundle.Header!, appSysCtx);
            var jsonEntity = jsonSerializer.ToJson(emptyEntity, metadataDepth: 0);

            // only attach metadata, if no metadata already exists
            return jsonEntity.For == null && bundle.Header?.For != null
                ? jsonEntity with { For = bundle.Header.For }
                : jsonEntity;
        }
    }
    
    private IEntity ConstructEmptyEntity(int appId, ItemIdentifier header, IAppWorkCtx appSysCtx)
    {
        var l = Log.Fn<IEntity>();
        var type = appSysCtx.AppReader.GetContentType(header.ContentTypeName!);
        var ent = entityAssembler.EmptyOfType(appId, header.Guid, header.EntityId, type);
        return l.Return(ent);
    }

}
