using System;
using ToSic.Lib.DI;
using ToSic.Lib.Logging;
using ToSic.Lib.Services;
using ToSic.Sxc.Internal;

namespace ToSic.Sxc.Blocks.Internal;

[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
public class BlockEditorSelector: ServiceBase
{
    private readonly LazySvc<BlockEditorForModule> _blkEdtForMod;
    private readonly LazySvc<BlockEditorForEntity> _blkEdtForEnt;

    public BlockEditorSelector(
        LazySvc<BlockEditorForModule> blkEdtForMod,
        LazySvc<BlockEditorForEntity> blkEdtForEnt
    ) : base($"{SxcLogging.SxcLogName}.BlEdSl")
    {
        ConnectServices(
            _blkEdtForMod = blkEdtForMod,
            _blkEdtForEnt = blkEdtForEnt
        );
    }

    public BlockEditorBase GetEditor(IBlock block) => Log.Func(() =>
    {
        var editor = GetEditorInternal(block);
        editor.Init(block);
        return editor;
    });

    private BlockEditorBase GetEditorInternal(IBlock block)
    {
        if (block is BlockFromModule) return _blkEdtForMod.Value;
        if (block is BlockFromEntity) return _blkEdtForEnt.Value;
        throw new Exception("Can't find BlockEditor - the base block type in unknown");
    }
}