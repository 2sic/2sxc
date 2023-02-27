using ToSic.Eav.Metadata;
using ToSic.Lib.Documentation;
using ToSic.Sxc.Blocks;

// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Context
{
    [PrivateApi("WIP / hide implementation")]
    public class CmsModule: CmsContextPartBase<IModule>, ICmsModule
    {
        public CmsModule(CmsContext parent, IModule module, IBlock block) : base(parent, module)
        {
            _block = block;
        }

        public int Id => GetContents()?.Id ?? 0;

        public ICmsBlock Block => _cmsBlock ?? (_cmsBlock = new CmsBlock(_block.BlockBuilder.RootBuilder.Block));
        private ICmsBlock _cmsBlock;
        private readonly IBlock _block;

        protected override IMetadataOf GetMetadataOf() 
            => ExtendWithRecommendations(_block.Context.AppState.GetMetadataOf(TargetTypes.Module, Id, "Module " + Id));

    }
}
