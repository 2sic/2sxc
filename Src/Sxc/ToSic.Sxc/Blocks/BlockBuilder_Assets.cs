using System.Collections.Generic;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Web;
using System.Linq;

namespace ToSic.Sxc.Blocks
{
    public partial class BlockBuilder
    {
        public List<IClientAsset> Assets { get; private set; } = new List<IClientAsset>();

        private void TransferEngineAssetsToParent(RenderEngineResult result)
        {
            if (!result.Assets.Any()) return;
            if (!(RootBuilder is BlockBuilder parentBlock)) return;
            parentBlock.Assets.AddRange(result.Assets);
            parentBlock.Assets = parentBlock.Assets.OrderBy(a => a.PosInPage).ToList();
        }
    }
}
