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
        // this is only relevant for the root builder, so we can skip it for child builders
        if (Block == null || Block.AppId == 0)
            return;

        // Cast to current object type to access internal APIs
        if ((Block.RootBlock.BlockBuilder ?? this) is not BlockBuilder rootBuilder)
            return;

        // add dependent appId only once
        if (rootBuilder.DependentApps.All(a => a.AppId != Block.AppId))
            rootBuilder.DependentApps.Add(new DependentApp { AppId = Block.AppId });
    }

}