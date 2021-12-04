using ToSic.Eav.Data;
using ToSic.Eav.Documentation;
using ToSic.Eav.Metadata;
using ToSic.Sxc.Blocks;
// ReSharper disable ConvertToNullCoalescingCompoundAssignment

namespace ToSic.Sxc.Context
{
    [PrivateApi("WIP / hide implementation")]
    public class CmsModule: Wrapper<IModule>, ICmsModule, IHasMetadata
    {

        public CmsModule(IModule module, IBlock block) : base(module)
        {
            _block = block;
        }

        public int Id => _contents?.Id ?? 0;

        public ICmsBlock Block => _cmsBlock ?? (_cmsBlock = new CmsBlock(_block.BlockBuilder.RootBuilder.Block));
        private ICmsBlock _cmsBlock;
        private readonly IBlock _block;


        public IMetadataOf Metadata
            => _metadata ?? (_metadata = _block.Context.AppState.GetMetadataOf((int)TargetTypes.CmsItem,
                CmsMetadata.ModulePrefix + Id,
                 "Module " + Id));
        private IMetadataOf _metadata;
    }
}
