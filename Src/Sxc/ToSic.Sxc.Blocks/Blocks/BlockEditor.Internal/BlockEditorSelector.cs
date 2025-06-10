using ToSic.Lib.Services;

namespace ToSic.Sxc.Blocks.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class BlockEditorSelector(
    LazySvc<BlockEditorForModule> blkEdtForMod,
    LazySvc<BlockEditorForEntity> blkEdtForEnt)
    : ServiceBase($"{SxcLogName}.BlEdSl", connect: [blkEdtForMod, blkEdtForEnt])
{
    public BlockEditorBase GetEditor(IBlock block)
    {
        var l = Log.Fn<BlockEditorBase>();
        var editor = GetEditorInternal(block);
        editor.Init(block);
        return l.Return(editor);
    }

    private BlockEditorBase GetEditorInternal(IBlock block) =>
        block switch
        {
            BlockOfModule => blkEdtForMod.Value,
            BlockOfEntity => blkEdtForEnt.Value,
            _ => throw new("Can't find BlockEditor - the base block type in unknown")
        };
}