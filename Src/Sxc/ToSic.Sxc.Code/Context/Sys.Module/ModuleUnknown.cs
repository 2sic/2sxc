using ToSic.Eav.Sys;
using ToSic.Sxc.Blocks.Sys;

namespace ToSic.Sxc.Context.Sys.Module;

[ShowApiWhenReleased(ShowApiMode.Never)]
internal class ModuleUnknown: IModule, IIsUnknown
{
    // ReSharper disable once UnusedParameter.Local
    public ModuleUnknown(WarnUseOfUnknown<ModuleUnknown> _) { }

    /// <summary>
    /// Dummy init, don't do anything
    /// </summary>
    /// <param name="id"></param>
    /// <returns></returns>
    public IModule Init(int id) => this;

    public int Id => EavConstants.NullId;
    public bool IsContent => true;

    public IBlockIdentifier BlockIdentifier =>
        new BlockIdentifier(EavConstants.NullId, EavConstants.NullId, EavConstants.NullNameId, Guid.Empty, Guid.Empty);
}