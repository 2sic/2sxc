using ToSic.Eav.Metadata;
using ToSic.Sxc.Blocks.Internal;

namespace ToSic.Sxc.Context.Internal;

internal class CmsModule(CmsContext parent, IModule module, IBlock block)
    : CmsContextPartBase<IModule>(parent, module), ICmsModule
{
    public int Id => GetContents()?.Id ?? 0;

    public ICmsBlock Block => _cmsBlock ??= new CmsBlock(block.BlockBuilder.RootBuilder.Block);
    private ICmsBlock _cmsBlock;

    protected override IMetadataOf GetMetadataOf() 
        => ExtendWithRecommendations(block.Context.AppReader.Metadata.GetMetadataOf(TargetTypes.Module, Id, title: "Module " + Id));

}