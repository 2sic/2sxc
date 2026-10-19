using ToSic.Eav.Apps;
using ToSic.Eav.Context;

namespace ToSic.Sxc.Context.Sys;

public interface IContextOfBlockFactory
{
    IContextOfBlock Update(IContextOfBlock contextOfBlock, IAppIdentity appIdentity);
    IContextOfBlock Create(ISite site, IPage page, IModule module);
}