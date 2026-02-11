using System.Collections.Immutable;
using ToSic.Eav.Apps.Sys;
using ToSic.Eav.Context.Sys.ZoneCulture;
using ToSic.Eav.DataSource.Query.Sys;
using ToSic.Sxc.Blocks.Sys.Views;

namespace ToSic.Sxc.Blocks.Sys.Work;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class WorkBlocks(IZoneCultureResolver cultureResolver, Generator<QueryDefinitionFactory> qDefBuilder, GenWorkPlus<WorkEntities> workEntities)
    : WorkUnitBase<IAppWorkCtxPlus>("SxS.Blocks", connect: [cultureResolver, qDefBuilder, workEntities])
{
    public const string BlockTypeName = "2SexyContent-ContentGroup";

    private IImmutableList<IEntity> GetContentGroups()
        => workEntities.New(AppWorkCtx).Get(BlockTypeName).ToImmutableOpt();

    public ICollection<BlockConfiguration> AllWithView()
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
            .Where(p => p != null)
            .Select(p => new BlockConfiguration(p!.Entity, appIdentity, null, qDefBuilder, cultureResolver.CurrentCultureCode, Log))
            .ToListOpt();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>Will always return an object, even if the group doesn't exist yet. The .Entity would be null then</returns>
    public BlockConfiguration GetBlockConfig(Guid contentGroupGuid)
    {
        var l = Log.Fn<BlockConfiguration>($"get CG#{contentGroupGuid}");
        var groupEntity = GetContentGroups().GetOne(contentGroupGuid);
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


    public BlockConfiguration GetOrGeneratePreviewConfig(IBlockIdentifier blockId)
    {
        var l = Log.Fn<BlockConfiguration>($"grp#{blockId.Guid}, preview#{blockId.PreviewView}");
        // Return a "faked" ContentGroup if it does not exist yet (with the preview templateId)
        var createTempBlockForPreview = blockId.Guid == Guid.Empty;
        l.A($"{nameof(createTempBlockForPreview)}:{createTempBlockForPreview}");
        var result = createTempBlockForPreview
            ? new(null,
                AppWorkCtx,
                AppWorkCtx.Data.List.GetOne(blockId.PreviewView),
                qDefBuilder,
                cultureResolver.CurrentCultureCode,
                Log
            )
            : GetBlockConfig(blockId.Guid);
        result.BlockIdentifierOrNull = blockId;
        return l.Return(result);
    }

}