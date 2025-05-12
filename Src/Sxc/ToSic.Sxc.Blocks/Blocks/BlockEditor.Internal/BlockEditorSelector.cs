using ToSic.Lib.DI;
using ToSic.Lib.Services;

namespace ToSic.Sxc.Blocks.Internal;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class BlockEditorSelector(
    LazySvc<BlockEditorForModule> blkEdtForMod,
    LazySvc<BlockEditorForEntity> blkEdtForEnt)
    : ServiceBase($"{SxcLogName}.BlEdSl", connect: [blkEdtForMod, blkEdtForEnt])
{
    public BlockEditorBase GetEditor(IBlock block) => Log.Func(() =>
    {
        var editor = GetEditorInternal(block);
        editor.Init(block);
        return editor;
    });

    private BlockEditorBase GetEditorInternal(IBlock block)
    {
        if (block is BlockOfModule) return blkEdtForMod.Value;
        if (block is BlockOfEntity) return blkEdtForEnt.Value;
        throw new("Can't find BlockEditor - the base block type in unknown");
    }
}