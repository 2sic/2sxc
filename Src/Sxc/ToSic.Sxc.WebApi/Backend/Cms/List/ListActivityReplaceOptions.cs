using ToSic.Eav.Apps.Sys.State;
using ToSic.Eav.Data.ContentTypes.Fields.Sys;
using ToSic.Eav.WebApi.Sys.Cms;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sxc.Blocks.Sys.Work;
using ToSic.Sys.Utils;
using static System.StringComparison;

namespace ToSic.Sxc.Backend.Cms;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class ListActivityReplaceOptions(
    AppWorkChain<WorkBlocks> appBlocks,
    AppWorkChain<WorkEntities> workEntities
) : ServiceWithSetup<IAppWorkContext>("Act.LstRep", connect: [appBlocks, workEntities])
{
    public record Options(
        Guid Parent,
        string Part,
        int Index,
        string? TypeNames = null
    );

    /// <summary>
    /// Special Replace just like list-replace, but with content type name coming from View definition
    /// </summary>
    public ReplacementListDto ReplaceOptions(Options options)
    {
        var l = Log.Fn<ReplacementListDto>($"{options}");
        options = options with
        {
            TypeNames = options.TypeNames ?? FindTypeNameOnContentGroup(options)
        };
        //var typeNameOfField = FindTypeNameOnContentGroup(appReader, options);
        var result = GetOptions(options);
        return l.Return(result);
    }

    /// <summary>
    /// Special edge case for Content-Groups.
    /// Content-groups have pairs of content+presentation.
    /// This affects what content-types must be retrieved.
    /// </summary>
    /// <returns></returns>
    private string? FindTypeNameOnContentGroup(Options options)
    {
        var l = Log.Fn<string>($"{options}");

        var contentGroup = appBlocks.New(MyOptions).GetBlockConfig(options.Parent);
        if ((contentGroup as ICanBeEntity)?.Entity == null || contentGroup.View == null)
            return l.ReturnNull("Doesn't seem to be a content-group. Cancel.");

        var typeNameForField = string.Equals(options.Part, ViewParts.ContentLower, OrdinalIgnoreCase)
            ? contentGroup.View.ContentType
            : contentGroup.View.HeaderType;

        return l.Return(typeNameForField);
    }

    private ReplacementListDto GetOptions(Options options)
    {
        var l = Log.Fn<ReplacementListDto>($"{options}");

        var (existingItemsInField, typeNameOfField) = FindItemAndFieldTypeName(MyOptions.AppReader, options);

        var typeNameList = options.TypeNames.CsvToArrayWithoutEmpty().ToListOpt();
        if (!typeNameList.Any())
            typeNameList = typeNameOfField;

        // if no type was defined in this set, then return an empty list as there is nothing to choose from
        if (!typeNameList.Any())
            return l.Return(new() { SelectedId = 0, Items = [] }, "no type name, so no data");

        var contentTypes = typeNameList
            .Select(MyOptions.AppReader.GetContentType)
            .ToList();

        var entitiesHelper = workEntities
            .New(MyOptions);
        var listTemp = typeNameList
            .SelectMany(t => entitiesHelper.Get(t))
            .ToList();

        var preferDraft = listTemp
            .Select(MyOptions.AppReader.GetDraftOrKeep)
            .Cast<IEntity>()
            // 2026-06-22 2dm - this seems like old code, where we ended up with both the draft and published.
            // disabled for now
            //.GroupBy(e => e.EntityId)
            //.Select(g => g.OrderBy(e => e.RepositoryId).Last())
            .ToList();

        // if list is empty or shorter than index (would happen in an add-to-end-request) return null
        var selectedId = existingItemsInField.Count > options.Index
            ? existingItemsInField[options.Index]?.EntityId
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

    private (List<IEntity> items, IList<string> typeNames) FindItemAndFieldTypeName(IAppReader appReader, Options options)
    {
        var (guid, part, _, _) = options;
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
        var typeNameForField = new WorkFieldEntityInspectType()
            .PrimaryTypeNames(attribute, modeCreate: true, tryOtherModes: true);
        return l.ReturnAsOk((itemList, typeNameForField));
    }

}