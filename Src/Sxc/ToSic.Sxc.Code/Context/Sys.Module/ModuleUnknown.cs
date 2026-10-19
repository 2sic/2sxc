using ToSic.Eav.Sys;
using ToSic.Sxc.Blocks.Sys;

namespace ToSic.Sxc.Context.Sys.Module;

// ReSharper disable UnusedParameter.Local

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class ModuleUnknown(WarnUseOfUnknown<ModuleUnknown> _) : IModule, IIsUnknown
{
    /// <summary>
    /// Dummy init, don't do anything
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public void Init(int id) { }

    public int Id => EavConstants.NullId;
    public bool IsContent => true;

    public IBlockIdentifier BlockIdentifier =>
        new BlockIdentifier(EavConstants.NullId, EavConstants.NullId, EavConstants.NullNameId, Guid.Empty, Guid.Empty);
}