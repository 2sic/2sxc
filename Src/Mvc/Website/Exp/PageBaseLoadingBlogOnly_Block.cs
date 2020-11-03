using ToSic.Eav.Apps.Run;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Mvc.Run;

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
                    new MvcPage(0, null), 
                    new MvcContainer(),
                    new MvcUser(),
                    null
                );
                _block = Eav.Factory.Resolve<BlockFromModule>().Init(context, Log);
                return _block;
            }
        }
        private IBlock _block;
        private bool _blockLoaded;

    }
}
