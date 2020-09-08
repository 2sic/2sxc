using ToSic.Eav.Apps.Run;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Mvc.Run;

namespace ToSic.Sxc.Mvc.WebApi
{
    public abstract class SxcStatefullControllerBase: SxcStatelessControllerBase
    {

        protected IInstanceContext GetContext()
        {
            // in case the initial request didn't yet find a block builder, we need to create it now
            var context = // BlockBuilder?.Context ??
                new InstanceContext(new MvcTenant(HttpContext), new PageNull(), new ContainerNull(), new MvcUser());
            return context;
        }

        protected IBlock GetBlock()
        {
            // todo: try to construct from headers
            return null;
        }
    }
}
