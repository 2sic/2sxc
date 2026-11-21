using ToSic.Eav.Data.ContentTypes.Fields.Sys;
using ToSic.Eav.Ui.Edit.Sys;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Blocks.Sys.Views;
using ToSic.Sxc.Blocks.Sys.Work;
using ToSic.Sys.HookUp;
using ToSic.Sys.Utils;

namespace ToSic.Sxc.Backend.Cms;

/// <summary>
/// Item identifier provider.
/// Will convert group-identifiers (with parent/index) to the proper IDs.
/// </summary>
/// <param name="blocks"></param>
[ShowApiWhenReleased(ShowApiMode.Never)]
public class EditIdentifierProviderContentGroupIndexToId(AppWorkChain<WorkBlocks> blocks)
    : ServiceWithSetup<IAppWorkContext>("Api.GrpPrc", connect: [blocks]),
        IEditLoadIdentifierProvider
{
    public int WorkSequenceOrder => EditIdentifierProviderContentGroupModeDetection.WorkSequenceOrderDefault + 1; // right after block determination

    public Task<Package<List<ItemIdentifier>>> Handle(WorkContext mainCtx, Package<List<ItemIdentifier>> package)
    {
        var appCtx = mainCtx.TryGet<IAppWorkContext>(EditLoadContextConstants.AppWorkCtx)
            ?? MyOptions;
        return Task.FromResult(package.RePackage(ConvertListIndexToId(appCtx, package.Data)));
    }

    private List<ItemIdentifier> ConvertListIndexToId(IAppWorkContext appCtx, List<ItemIdentifier> identifiers)
    {
        var lOuter = Log.Fn<List<ItemIdentifier>>();
        var appBlocks = blocks.New(appCtx);
        var corrected = identifiers
            .Select((identifier, index) =>
            {
                var l = lOuter.Fn<ItemIdentifier>($"{index}");
                // Case one, it's a Content-Group - in this case the content-type name comes from View configuration
                if (identifier.IsContentBlockMode)
                {
                    if (!identifier.Parent.HasValue)
                        return l.ReturnNull("identifier is ContentBlockMode, but no parent");

                    var contentGroup = appBlocks.GetBlockConfig(identifier.GetParentEntityOrError());
                    var contentTypeName = contentGroup.View?.GetTypeStaticName(identifier.Field!) ?? "";

                    // if there is no content-type for this, then skip it (don't deliver anything)
                    if (contentTypeName == "")
                        return l.ReturnNull($"identifier is ContentBlockMode, but no content-type on field {identifier.Field}");

                    identifier = identifier with { ContentTypeName = contentTypeName };
                    identifier = ConvertListIndexToEntityIds(identifier, contentGroup);
                    return l.Return(identifier, $"identifier is ContentBlockMode, type is '{contentTypeName}'");
                }

                // Not Case #2
                // Default case - just a normal identifier
                if (identifier is not { Parent: not null, Field: not null })
                    return l.Return(identifier, "default case, normal identifier");

                // Case #2 it's an entity inside a field of another entity
                // Added in v11.01
                // check if it's a request for new, in which case we should check the create-type
                if (identifier.AddSafe)
                {
                    lOuter.A("identifier is new");
                    if (identifier.ContentTypeName.HasValue())
                        return l.Return(identifier, $"using identifier with existing type {identifier.ContentTypeName}");

                    // look up type
                    var target = appCtx.AppReader.List.GetOne(identifier.Parent.Value)!;
                    var field = target.Type[identifier.Field]!;
                    identifier = identifier with
                    {
                        ContentTypeName = new WorkFieldEntityInspectType()
                            .PrimaryTypeName(field, modeCreate: true)
                    };
                    return l.Return(identifier, "identifier is new");
                }

                // Otherwise it's an edit operation, and it could be a different content-type than is configured for the field
                // because the field may be configured for multiple types
                lOuter.A("identifier is edit");
                var realEntity = appCtx.AppReader.List.GetOne(identifier.EntityId);
                if (realEntity == null)
                    return l.ReturnNull("identifier is edit, but couldn't find real entity.");
                identifier = identifier with { ContentTypeName = realEntity.Type.NameId };
                return l.Return(identifier, $"identity is edit, content-Type is {identifier.ContentTypeName}");

            })
            .Where(id => id != null)
            .Cast<ItemIdentifier>()
            .ToList();

        return lOuter.Return(corrected, $"{corrected.Count}");
    }

    private ItemIdentifier ConvertListIndexToEntityIds(ItemIdentifier identifier, BlockConfiguration blockConfiguration)
    {
        var l = Log.Fn<ItemIdentifier>();
        var part = blockConfiguration.GetPart(identifier.Field!);
        if (!identifier.AddSafe) // not in add-mode
        {
            var idx = identifier.IndexSafeOrFallback(part.Count - 1);
            // has as many items as desired and the slot has something inside it
            if (idx >= 0 && part.Count > idx && part[idx] != null!) // paranoid check
                identifier = identifier with { EntityId = part[idx].EntityId };
        }

        // tell the UI that it should not actually use this data yet, keep it locked
        if (!identifier.Field!.ToLowerInvariant().Contains(ViewParts.PresentationLower))
            return l.Return(identifier, "no presentation");

        // the following steps are only for presentation items
        identifier = identifier with { IsEmptyAllowed = true };

        if (identifier.EntityId != 0)
            return l.Return(identifier, "id != 0");

        identifier = identifier with { IsEmpty = true };

        var duplicate = identifier.Field.ToLowerInvariant() == ViewParts.PresentationLower
            ? blockConfiguration.View!.PresentationItem?.EntityId
            : blockConfiguration.View!.HeaderPresentationItem?.EntityId;
        identifier = identifier with { DuplicateEntity = duplicate };

        return l.Return(identifier, "ok");
    }

}
