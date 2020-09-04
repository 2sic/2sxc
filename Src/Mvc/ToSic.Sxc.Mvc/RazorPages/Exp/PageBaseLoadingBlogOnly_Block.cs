using ToSic.Eav.Apps.Run;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Mvc.Code;
using ToSic.Sxc.Mvc.Run;
using ToSic.Sxc.Mvc.TestStuff;

namespace ToSic.Sxc.Mvc.RazorPages.Exp
{
    public partial class PageBaseLoadingBlogOnly
    {
        #region DynCode 

        protected MvcDynamicCode DynCode => _dynCode ??= new MvcDynamicCode().Init(Block, Log);
        private MvcDynamicCode _dynCode;
        #endregion
        public IBlock Block 
        {
            get
            {
                if (_blockLoaded) return _block;
                _blockLoaded = true;
                var context = new InstanceContext(
                    new MvcTenant(new MvcPortalSettings()),
                    new MvcPage(0, null), 
                    new MvcContainer(),
                    new MvcUser()
                );
                _block = new BlockFromModule().Init(context, Log);
                return _block;
            }
        }
        private IBlock _block;
        private bool _blockLoaded;

    }
}
