namespace ToSic.Sxc.Blocks.Sys.BlockEditor;

[ShowApiWhenReleased(ShowApiMode.Never)]
public class BlockEditorSelector(LazySvc<BlockEditorForModule> blkEdtForMod, LazySvc<BlockEditorForEntity> blkEdtForEnt)
    : ServiceBase($"{SxcLogName}.BlEdSl")
{
    public BlockEditorBase GetEditor(IBlock block)
    {
        using var l = Log.Fn<BlockEditorBase>();
        var editor = GetEditorInternal(block);
        editor.Init(block);
        return l.Return(editor);
    }

    private BlockEditorBase GetEditorInternal(IBlock block)
        => block.IsInnerBlock switch
        {
            false => blkEdtForMod.Value,
            true => blkEdtForEnt.Value,
        };
}