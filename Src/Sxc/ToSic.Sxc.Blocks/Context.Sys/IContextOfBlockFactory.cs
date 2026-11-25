using ToSic.Eav.Apps;
using ToSic.Eav.Context;

namespace ToSic.Sxc.Context.Sys;

public interface IContextOfBlockFactory
{
    IContextOfModule Update(IContextOfModule contextOfModule, IAppIdentity appIdentity);
    IContextOfModule Create(ISite site, IPage page, IModule module);
}