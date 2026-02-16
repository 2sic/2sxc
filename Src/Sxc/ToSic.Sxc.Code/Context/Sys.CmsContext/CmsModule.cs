using ToSic.Eav.Metadata;
using ToSic.Eav.Metadata.Sys;
using ToSic.Sxc.Blocks.Sys;

namespace ToSic.Sxc.Context.Sys.CmsContext;

internal class CmsModule(CmsContext parent, IModule module, IBlock block, IMetadataOfSource appMetadata)
    : CmsContextPartBase<IModule>(parent, module), ICmsModule
{
    public int Id => GetContents()?.Id ?? 0;

    [field: AllowNull, MaybeNull]
    public ICmsBlock Block => field ??= new CmsBlock(block.RootBlock);

    protected override IMetadata GetMetadataOf() 
        => appMetadata.GetMetadataOf(TargetTypes.Module, Id, title: "Module " + Id)
            .AddRecommendations();
}