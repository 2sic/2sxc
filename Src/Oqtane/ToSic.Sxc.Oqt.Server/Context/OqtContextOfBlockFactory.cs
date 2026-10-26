using Oqtane.Models;
using ToSic.Eav.Context;
using ToSic.Sxc.Context;
using ToSic.Sxc.Context.Sys;

namespace ToSic.Sxc.Oqt.Server.Context;

internal class OqtContextOfBlockFactory(
    LazySvc<IContextOfBlockFactory> contextFactory,
    ISite site,
    IPage page,
    Generator<IModule> moduleGenerator)
{
    public IContextOfBlock Create(int pageId, Module oqtModule)
    {
        ((OqtPage)page).OqtInit(pageId);
        var module = ((OqtModule)moduleGenerator.New()).Init(oqtModule);
        return contextFactory.Value.Create(site, page, module);
    }
}
