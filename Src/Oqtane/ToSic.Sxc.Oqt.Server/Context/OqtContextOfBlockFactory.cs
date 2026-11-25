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
    // Indexing already has the native module. Keep the page id explicit: a shared module
    // can have several occurrences with different URLs and page permissions.
    public IContextOfModule Create(int pageId, Module oqtModule)
        => Create(pageId, ((OqtModule)moduleGenerator.New()).Init(oqtModule));

    public IContextOfModule Create(int pageId, IModule module)
    {
        ((OqtPage)page).OqtInit(pageId);
        return contextFactory.Value.Create(site, page, module);
    }
}
