using ToSic.Eav.Metadata;
using ToSic.Sxc.Blocks.Internal;
using ToSic.Sxc.Blocks.Sys;

namespace ToSic.Sxc.Context.Internal;

internal class CmsModule(CmsContext parent, IModule module, IBlock block)
    : CmsContextPartBase<IModule>(parent, module), ICmsModule
{
    public int Id => GetContents()?.Id ?? 0;

    [field: AllowNull, MaybeNull]
    public ICmsBlock Block => field ??= new CmsBlock(block.RootBlock);

    protected override IMetadata GetMetadataOf() 
        => block.Context.AppReaderRequired.Metadata.GetMetadataOf(TargetTypes.Module, Id, title: "Module " + Id)
            .AddRecommendations();

}