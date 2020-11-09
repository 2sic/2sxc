using ToSic.Eav.Apps.Run;
using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Mvc.Run;
using ToSic.Sxc.Run;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Mvc.RazorPages.Exp
{
    public partial class PageBaseLoadingBlogOnly
    {
        #region DynCode 

        protected Sxc.Code.DynamicCodeRoot DynCode => _dynCode ??= new Sxc.Code.DynamicCodeRoot().Init(Block, Log);
        private Sxc.Code.DynamicCodeRoot _dynCode;
        #endregion
        public IBlock Block 
        {
            get
            {
                if (_blockLoaded) return _block;
                _blockLoaded = true;
                var context = new InstanceContext(
                    new MvcSite(HttpContext),
                    new SxcPage(0, null, _serviceProvider.Build<IHttp>().QueryStringKeyValuePairs()), 
                    new MvcContainer(),
                    new MvcUser(),
                    _serviceProvider, new InstancePublishingState()
                );
                _block = Eav.Factory.Resolve<BlockFromModule>().Init(context, Log);
                return _block;
            }
        }
        private IBlock _block;
        private bool _blockLoaded;

    }
}
