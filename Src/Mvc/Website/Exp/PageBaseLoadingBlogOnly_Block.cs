using ToSic.Eav.Apps.Run;
using ToSic.Eav.Plumbing;
using ToSic.Eav.Run;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Cms.Publishing;
using ToSic.Sxc.Mvc.Dev;
using ToSic.Sxc.Mvc.Run;
using ToSic.Sxc.Run;
using ToSic.Sxc.Run.Context;
using ToSic.Sxc.Web;

namespace ToSic.Sxc.Mvc.RazorPages.Exp
{
    public partial class PageBaseLoadingBlogOnly
    {
        #region DynCode 

        protected Sxc.Code.DynamicCodeRoot DynCode => _dynCode ??= ServiceProvider.Build<Code.DynamicCodeRoot>().Init(Block, Log);
        private Sxc.Code.DynamicCodeRoot _dynCode;
        #endregion
        public IBlock Block 
        {
            get
            {
                if (_blockLoaded) return _block;
                _blockLoaded = true;
                var context = new ContextOfBlock(
                    ServiceProvider,
                    ServiceProvider.Build<ISite>().Init(TestIds.PrimaryZone),
                    new MvcUser()
                ).Init(
                    ServiceProvider.Build<SxcPage>().Init(0),
                    new MvcContainer(),
                    new BlockPublishingState()
                    );
                _block = ServiceProvider.Build<BlockFromModule>().Init(context, Log);
                return _block;
            }
        }
        private IBlock _block;
        private bool _blockLoaded;

    }
}
