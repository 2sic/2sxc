using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.Data.Sys.ContentTypes;
using ToSic.Eav.WebApi.Sys.Cms;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sxc.Blocks.Sys.Work;
using ToSic.Sys.Utils;
using static System.StringComparison;

namespace ToSic.Sxc.Backend.Cms;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ListActivityReplaceOptions(
    GenWorkPlus<WorkBlocks> appBlocks,
    GenWorkPlus<WorkEntities> workEntities,
    ISxcCurrentContextService ctxService
) : ServiceBase("Act.LstRep", connect: [appBlocks, workEntities, ctxService])
{
    public record Options(
        Guid Parent,
        string Part,
        int Index
    );

    /// <summary>
    /// Special Replace just like list-replace, but with content type name coming from View definition
    /// </summary>
    public ReplacementListDto ReplaceOptions(Options options)
    {
        var (parent, part, index) = options;
        var l = Log.Fn<ReplacementListDto>($"target:{parent}, part:{part}, index:{index}");
        var appReader = ctxService.BlockContextRequired().AppReaderRequired;
        var typeNameOfField = FindTypeNameOnContentGroup(appReader, parent, part);
        var result = GetOptions(appReader, parent, part, index, typeNameOfField);
        return l.Return(result);
    }


    private string? FindTypeNameOnContentGroup(IAppReader appReader, Guid guid, string part)
    {
        var l = Log.Fn<string>($"{guid}, {part}");

        var appCtx = appBlocks.CtxSvc.ContextPlus(appReader);
        var contentGroup = appBlocks.New(appCtx).GetBlockConfig(guid);
        if ((contentGroup as ICanBeEntity)?.Entity == null || contentGroup.View == null)
            return l.ReturnNull("Doesn't seem to be a content-group. Cancel.");

        var typeNameForField = string.Equals(part, ViewParts.ContentLower, OrdinalIgnoreCase)
            ? contentGroup.View.ContentType
            : contentGroup.View.HeaderType;

        return l.Return(typeNameForField);
    }

    private ReplacementListDto GetOptions(IAppReader appReader, Guid guid, string part, int index, string? typeNames)
    {
        var l = Log.Fn<ReplacementListDto>($"{nameof(typeNames)}:{typeNames}, {nameof(part)}:{part}, {nameof(index)}:{index}");

        var (existingItemsInField, typeNameOfField) = FindItemAndFieldTypeName(appReader, guid, part);

        var typeNameList = typeNames.CsvToArrayWithoutEmpty().ToListOpt();
        if (!typeNameList.Any())
            typeNameList = typeNameOfField;

        // if no type was defined in this set, then return an empty list as there is nothing to choose from
        if (!typeNameList.Any())
            return l.Return(new() { SelectedId = 0, Items = [] }, "no type name, so no data");

        var contentTypes = typeNameList
            .Select(appReader.GetContentType)
            .ToList();

        var entitiesHelper = workEntities.New(appReader);
        var listTemp = typeNameList
            .SelectMany(t => entitiesHelper.Get(t))
            .ToList();

        var preferDraft = listTemp
            .Select(appReader.GetDraftOrKeep)
            .Cast<IEntity>()
            // 2026-06-22 2dm - this seems like old code, where we ended up with both the draft and published.
            // disabled for now
            //.GroupBy(e => e.EntityId)
            //.Select(g => g.OrderBy(e => e.RepositoryId).Last())
            .ToList();

        // if list is empty or shorter than index (would happen in an add-to-end-request) return null
        var selectedId = existingItemsInField.Count > index
            ? existingItemsInField[index]?.EntityId
            : null;

        var result = new ReplacementListDto
        {
            SelectedId = selectedId,
            Items = preferDraft.Select(e => new ReplacementListItemDto
            {
                Id = e.EntityId,
                Title = e.GetBestTitle() ?? "(no title)",
                ContentType = e.Type.Name,
            })
        };
        return l.Return(result);
    }

    private (List<IEntity> items, IList<string> typeNames) FindItemAndFieldTypeName(IAppReader appReader, Guid guid, string part)
    {
        var l = Log.Fn<(List<IEntity>, IList<string>)>($"guid:{guid},part:{part}");

        // Find owner/parent
        var parent = appReader.GetDraftOrPublished(guid);
        if (parent == null)
            throw l.Done(new Exception($"No item found for {guid}"));

        // Verify it has specified attribute
        if (!parent.Attributes.ContainsKey(part))
            throw l.Done(new Exception($"Could not find field {part} in item {guid}"));

        // Find children in the attribute
        var itemList = parent
            .Children(part)
            .Select(appReader.GetDraftOrKeep)
            .Where(e => e != null)
            .Cast<IEntity>()
            .ToList();

        // find attribute-type-name
        var attribute = parent.Type[part];
        if (attribute == null)
            throw l.Done(new Exception($"Attribute definition for '{part}' not found on the item {guid}"));
        var typeNameForField = new WorkAttributeEntityInspectType()
            .PrimaryTypeNames(attribute, modeCreate: true, tryOtherModes: true);
        return l.ReturnAsOk((itemList, typeNameForField));
    }

}