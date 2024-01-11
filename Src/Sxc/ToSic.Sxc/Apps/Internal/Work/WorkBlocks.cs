using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using ToSic.Eav.Apps.Work;
using ToSic.Eav.Cms.Internal;
using ToSic.Eav.Context;
using ToSic.Eav.Data;
using ToSic.Eav.DataSource.Internal.Query;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Blocks.Internal;

namespace ToSic.Sxc.Apps.Internal.Work;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class WorkBlocks: WorkUnitBase<IAppWorkCtxPlus>
{
    private readonly GenWorkPlus<WorkEntities> _workEntities;
    public const string BlockTypeName = "2SexyContent-ContentGroup";

    private readonly LazySvc<QueryDefinitionBuilder> _qDefBuilder;
    private readonly IZoneCultureResolver _cultureResolver;

    public WorkBlocks(IZoneCultureResolver cultureResolver, LazySvc<QueryDefinitionBuilder> qDefBuilder, GenWorkPlus<WorkEntities> workEntities) : base("SxS.Blocks")
    {
        ConnectServices(
            _cultureResolver = cultureResolver,
            _qDefBuilder = qDefBuilder,
            _workEntities = workEntities
        );
    }

    private IImmutableList<IEntity> ContentGroups() => _workEntities.New(AppWorkCtx).Get(BlockTypeName).ToImmutableList();

    public List<BlockConfiguration> AllWithView()
    {
        return ContentGroups()
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
            .Select(e => new BlockConfiguration(e.Entity, AppWorkCtx, null, _qDefBuilder, _cultureResolver.CurrentCultureCode, Log))
            .ToList();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <returns>Will always return an object, even if the group doesn't exist yet. The .Entity would be null then</returns>
    public BlockConfiguration GetBlockConfig(Guid contentGroupGuid)
    {
        var l = Log.Fn<BlockConfiguration>($"get CG#{contentGroupGuid}");
        var groupEntity = ContentGroups().One(contentGroupGuid);
        var found = groupEntity != null;
        return l.Return(found
                ? new BlockConfiguration(groupEntity, AppWorkCtx, null, _qDefBuilder, _cultureResolver.CurrentCultureCode, Log)
                    .WarnIfMissingData()
                : new BlockConfiguration(null, AppWorkCtx, null, _qDefBuilder, _cultureResolver.CurrentCultureCode, Log)
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
            ? new BlockConfiguration(null, AppWorkCtx, AppWorkCtx.Data.List.One(blockId.PreviewView), _qDefBuilder, _cultureResolver.CurrentCultureCode, Log)
            : GetBlockConfig(blockId.Guid);
        result.BlockIdentifierOrNull = blockId;
        return l.Return(result);
    }

}