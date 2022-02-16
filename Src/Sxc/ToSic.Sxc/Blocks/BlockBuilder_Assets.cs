using System.Collections.Generic;
using ToSic.Sxc.Engines;
using ToSic.Sxc.Web;
using System.Linq;

namespace ToSic.Sxc.Blocks
{
    public partial class BlockBuilder
    {
        public List<IClientAsset> Assets { get; private set; } = new List<IClientAsset>();

        private void TransferEngineAssetsToParent(IEngine engine)
        {
            if (!engine.Assets.Any()) return;
            if (RootBuilder is BlockBuilder parentBlock)
            {
                parentBlock.Assets.AddRange(engine.Assets);
                parentBlock.Assets = parentBlock.Assets.OrderBy(a => a.PosInPage).ToList();
            }
        }
    }
}
