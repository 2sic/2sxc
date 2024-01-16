using ToSic.Sxc.Blocks.Internal.Render;

namespace ToSic.Sxc.Blocks.Internal;

public partial class BlockBuilder
{
    /// <summary>
    /// This list is only populated on the root builder. Child builders don't actually use this.
    /// </summary>
    public IList<IDependentApp> DependentApps { get; } = new List<IDependentApp>();


    private void PreSetAppDependenciesToRoot()
    {
        if (Block == null) return;
        if (RootBuilder is not BlockBuilder parentBlock) return;
        if (Block.AppId != 0)// && Block.App?.AppState != null)
            if (parentBlock.DependentApps.All(a => a.AppId != Block.AppId)) // add dependent appId only ounce
                parentBlock.DependentApps.Add(new DependentApp { AppId = Block.AppId });
    }

}