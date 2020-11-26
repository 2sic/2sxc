using ToSic.Eav.Apps.Run;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Run;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Mvc.Dev;
using ToSic.Sxc.Mvc.Run;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Mvc.RazorPages.Exp
{
    public partial class PageBaseLoadingBlogOnly
    {
        #region DynCode 

        protected Sxc.Code.DynamicCodeRoot DynCode => _dynCode ??= HttpContext.RequestServices.Build<Code.DynamicCodeRoot>().Init(Block, Log);
        private Sxc.Code.DynamicCodeRoot _dynCode;
        #endregion
        public IBlock Block 
        {
            get
            {
                if (_blockLoaded) return _block;
                _blockLoaded = true;
                var sp = HttpContext.RequestServices;
                var context = new ContextOfBlock(
                    sp.Build<ISite>().Init(TestIds.PrimaryZone),
                    new SxcPage(0, null, ServiceProvider.Build<IHttp>().QueryStringKeyValuePairs()), 
                    new MvcContainer(),
                    new MvcUser(),
                    ServiceProvider, new BlockPublishingState()
                );
                _block = Eav.Factory.Resolve<BlockFromModule>().Init(context, Log);
                return _block;
            }
        }
        private IBlock _block;
        private bool _blockLoaded;

    }
}
