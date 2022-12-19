using ToSic.Eav.Apps;
using ToSic.Lib.DI;
using ToSic.Sxc.Apps;

namespace ToSic.Sxc.Blocks.Edit
{
    public class BlockEditorBaseDependencies: DependenciesBase<BlockEditorBaseDependencies>
    {
        public LazyInit<CmsRuntime> CmsRuntime { get; }
        public LazyInit<CmsManager> CmsManager { get; }
        public LazyInit<AppManager> AppManager { get; }
        public Generator<BlockEditorForModule> BlkEdtForMod { get; }
        public Generator<BlockEditorForEntity> BlkEdtForEnt { get; }

        public BlockEditorBaseDependencies(LazyInit<CmsRuntime> cmsRuntime, 
            LazyInit<CmsManager> cmsManager, 
            LazyInit<AppManager> appManager,
            Generator<BlockEditorForModule> blkEdtForMod,
            Generator<BlockEditorForEntity> blkEdtForEnt)
        {
            AddToLogQueue(
                CmsRuntime = cmsRuntime,
                CmsManager = cmsManager,
                AppManager = appManager,
                BlkEdtForMod = blkEdtForMod,
                BlkEdtForEnt = blkEdtForEnt
            );
        }
    }
}
