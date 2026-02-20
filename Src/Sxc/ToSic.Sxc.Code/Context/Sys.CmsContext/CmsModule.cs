using ToSic.Eav.Metadata;
using ToSic.Eav.Metadata.Sys;
using ToSic.Sxc.Blocks.Sys;

namespace ToSic.Sxc.Context.Sys.CmsContext;

internal class CmsModule(CmsContext parent, IModule module, IBlock? rootBlock, IMetadataOfSource appMetadata)
    : CmsContextPartBase<IModule>(parent, module), ICmsModule
{
    public int Id => GetContents()?.Id ?? 0;

    [field: AllowNull, MaybeNull]
    public ICmsBlock Block => field ??= new CmsBlock(rootBlock); // note: will return a dummy info if rootBlock is null

    protected override IMetadata GetMetadataOf() 
        => appMetadata.GetMetadataOf(TargetTypes.Module, Id, title: "Module " + Id)
            .AddRecommendations();
}