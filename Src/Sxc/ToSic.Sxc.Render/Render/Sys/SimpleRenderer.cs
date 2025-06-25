using System.Text;
using ToSic.Eav.Apps.Sys;
using ToSic.Lib.DI;
using ToSic.Lib.Services;
using ToSic.Sxc.Blocks.Sys;
using ToSic.Sxc.Services;
using ToSic.Sys.Performance;

namespace ToSic.Sxc.Blocks.Internal.Render;

[PrivateApi]
[ShowApiWhenReleased(ShowApiMode.Never)]
public class SimpleRenderer(Generator<BlockOfEntity> blkFrmEntGen, Generator<IBlockBuilder> blockBuilderGenerator)
    : ServiceBase(SxcLogName + "RndSmp", connect: [blkFrmEntGen, blockBuilderGenerator])
{
    private const string EmptyMessage = "<!-- auto-render of item {0} -->";

    public string? Render(IBlock parentBlock, IEntity entity, object? data = default)
    {
        var l = Log.Fn<string?>();

        // if not the expected content-type, just output a hidden html placeholder
        if (entity.Type.Name != AppConstants.ContentGroupRefTypeName)
        {
            l.A("empty, will return hidden html placeholder");
            return string.Format(EmptyMessage, entity.EntityId);
        }

        // render it
        l.A("found, will render");
        var blockOfEntity = blkFrmEntGen.New().GetBlockOfEntity(parentBlock, entity);
        var builder = blockBuilderGenerator.New().Setup(blockOfEntity);
        var result = builder.Run(false, specs: new() { Data = data });

        // Special: during Run() various things are picked up like header changes, activations etc.
        // Depending on the code flow, it could have picked up changes of other templates (not this one)
        // because these were scoped, 
        // must attach additional info to the parent block, so it doesn't loose header changes and similar

        return l.Return(result.Html);
    }

    private const string WrapperTemplate = "<div class='{0}' {1}>{2}</div>";
    private const string WrapperMultiItems = "sc-content-block-list"; // tells quickE that it's an editable area
    private const string WrapperSingleItem = WrapperMultiItems + " show-placeholder single-item"; // enables a placeholder when empty, and limits one entry

    internal string RenderWithEditContext(IBlock block, ICanBeEntity parent, ICanBeEntity? subItem, string? cbFieldName, Guid? newGuid, IEditService edit, object? data = default)
    {
        var l = Log.Fn<string>();
        var attribs = edit.ContextAttributes(parent, field: cbFieldName, newGuid: newGuid);
        var inner = subItem == null
            ? ""
            : Render(block, subItem.Entity, data: data);
        var cbClasses = edit.Enabled ? WrapperSingleItem : "";
        // ReSharper disable FormatStringProblem
        return l.Return(string.Format(WrapperTemplate, args: [cbClasses, attribs, inner]));
        // ReSharper restore FormatStringProblem
    }

    public string RenderListWithContext(IBlock block, IEntity parent, string? fieldName, string? apps, int max, IEditService edit)
    {
        var l = Log.Fn<string>();
        var innerBuilder = new StringBuilder();
        var children = parent.Entity
            .Children(fieldName)
            .Where(child => child != null)
            .ToListOpt();
        foreach (var child in children)
            innerBuilder.Append(Render(block, child!));

        //var found = parent.TryGetMember(fieldName, out var objFound);
        //if (found && objFound is IList<DynamicEntity> items)
        //    foreach (var cb in items)
        //        innerBuilder.Append(Render(block, cb.Entity));

        // ReSharper disable FormatStringProblem
        var result = string.Format(WrapperTemplate, args: [
            edit.Enabled ? WrapperMultiItems : "",
            edit.ContextAttributes(parent, field: fieldName, apps: apps, max: max),
            innerBuilder
        ]);
        // ReSharper restore FormatStringProblem
        return l.Return(result);
    }
}