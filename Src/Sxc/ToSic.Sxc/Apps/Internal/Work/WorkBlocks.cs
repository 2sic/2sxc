using System.Collections.Immutable;
using ToSic.Eav.Apps;
using ToSic.Eav.Apps.Internal.Work;
using ToSic.Eav.Cms.Internal;
using ToSic.Eav.Context;
using ToSic.Eav.DataSource.Internal.Query;
using ToSic.Lib.DI;
using ToSic.Sxc.Blocks.Internal;

namespace ToSic.Sxc.Apps.Internal.Work;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class WorkBlocks(
    IZoneCultureResolver cultureResolver,
    Generator<QueryDefinitionBuilder> qDefBuilder,
    GenWorkPlus<WorkEntities> workEntities,
    LazySvc<WorkViews> workViews)
    : WorkUnitBase<IAppWorkCtxPlus>("SxS.Blocks", connect: [cultureResolver, qDefBuilder, workEntities, workViews])
{
    public const string BlockTypeName = "2SexyContent-ContentGroup";

    private IImmutableList<IEntity> GetContentGroups() => workEntities.New(AppWorkCtx).Get(BlockTypeName).ToImmutableList();

    public List<BlockConfiguration> AllWithView()
    {
        var appIdentity = AppWorkCtx.PureIdentity();
        return GetContentGroups()
            .Select(b =>
            {
                var templateGuid = b.Children(ViewParts.ViewFieldInContentBlock)
                    .FirstOrDefault()
                    ?.EntityGuid;
                return templateGuid != null
                    ? new { Entity = b, ViewGuid = templateGuid }
                    : null;
            })
            .Where(b => b != null)
            .Select(s => new BlockConfiguration(s.Entity, appIdentity, null, qDefBuilder, cultureResolver.CurrentCultureCode, Log))
            .ToList();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>Will always return an object, even if the group doesn't exist yet. The .Entity would be null then</returns>
    public BlockConfiguration GetBlockConfig(Guid contentGroupGuid)
    {
        var l = Log.Fn<BlockConfiguration>($"get CG#{contentGroupGuid}");
        var groupEntity = GetContentGroups().One(contentGroupGuid);
        var found = groupEntity != null;
        return l.Return(found
                ? new BlockConfiguration(groupEntity, AppWorkCtx, null, qDefBuilder, cultureResolver.CurrentCultureCode, Log)
                    .WarnIfMissingData()
                : new(null, AppWorkCtx, null, qDefBuilder, cultureResolver.CurrentCultureCode, Log)
                {
                    DataIsMissing = true
                },
            found ? "found" : "missing");
    }


    internal BlockConfiguration GetOrGeneratePreviewConfig(IBlockIdentifier blockId)
    {
        var l = Log.Fn<BlockConfiguration>($"grp#{blockId.Guid}, preview#{blockId.PreviewView}");
        // Return a "faked" ContentGroup if it does not exist yet (with the preview templateId)
        var createTempBlockForPreview = blockId.Guid == Guid.Empty;
        l.A($"{nameof(createTempBlockForPreview)}:{createTempBlockForPreview}");
        var result = createTempBlockForPreview
            ? new(null, AppWorkCtx, AppWorkCtx.Data.List.One(blockId.PreviewView), qDefBuilder, cultureResolver.CurrentCultureCode, Log)
            : GetBlockConfig(blockId.Guid);
        result.BlockIdentifierOrNull = blockId;
        return l.Return(result);
    }

}