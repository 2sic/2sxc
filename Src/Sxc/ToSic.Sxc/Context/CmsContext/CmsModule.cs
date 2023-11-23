using ToSic.Eav.Metadata;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Blocks;


namespace ToSic.Sxc.Context
{
    [PrivateApi("WIP / hide implementation")]
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class CmsModule: CmsContextPartBase<IModule>, ICmsModule
    {
        public CmsModule(CmsContext parent, IModule module, IBlock block) : base(parent, module)
        {
            _block = block;
        }

        public int Id => GetContents()?.Id ?? 0;

        public ICmsBlock Block => _cmsBlock ??= new CmsBlock(_block.BlockBuilder.RootBuilder.Block);
        private ICmsBlock _cmsBlock;
        private readonly IBlock _block;

        protected override IMetadataOf GetMetadataOf() 
            => ExtendWithRecommendations(_block.Context.AppState.GetMetadataOf(TargetTypes.Module, Id, "Module " + Id));

    }
}
