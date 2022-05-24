using System.Collections.Generic;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Web;
using System.Linq;

namespace ToSic.Sxc.Blocks
{
    public partial class BlockBuilder
    {
        /// <summary>
        /// This list is only populated on the root builder. Child builders don't actually use this.
        /// </summary>
        public List<IClientAsset> Assets { get; private set; } = new List<IClientAsset>();

        /// <summary>
        /// This list is only populated on the root builder. Child builders don't actually use this.
        /// </summary>
        public IList<IDependentApp> DependentApps { get; } = new List<IDependentApp>();


        private void PreSetAppDependenciesToRoot()
        {
            if (Block == null) return;
            if (!(RootBuilder is BlockBuilder parentBlock)) return;
            if (Block.AppId != 0)// && Block.App?.AppState != null)
                if (parentBlock.DependentApps.All(a => a.AppId != Block.AppId)) // add dependent appId only ounce
                    parentBlock.DependentApps.Add(new DependentApp { AppId = Block.AppId });
        }

        private void TransferCurrentAssetsAndAppDependenciesToRoot(RenderEngineResult result)
        {
            if (!(RootBuilder is BlockBuilder parentBlock)) return;
            if (!result.Assets.Any()) return;
            parentBlock.Assets.AddRange(result.Assets);
            parentBlock.Assets = parentBlock.Assets.OrderBy(a => a.PosInPage).ToList();
        }
    }
}
