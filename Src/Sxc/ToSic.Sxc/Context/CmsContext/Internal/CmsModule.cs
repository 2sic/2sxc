using ToSic.Eav.Metadata;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Blocks;

namespace ToSic.Sxc.Context.Internal;

[PrivateApi("WIP / hide implementation")]
[System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
internal class CmsModule(CmsContext parent, IModule module, IBlock block)
    : CmsContextPartBase<IModule>(parent, module), ICmsModule
{
    public int Id => GetContents()?.Id ?? 0;

    public ICmsBlock Block => _cmsBlock ??= new CmsBlock(block.BlockBuilder.RootBuilder.Block);
    private ICmsBlock _cmsBlock;

    protected override IMetadataOf GetMetadataOf() 
        => ExtendWithRecommendations(block.Context.AppState.GetMetadataOf(TargetTypes.Module, Id, "Module " + Id));

}