using ToSic.Eav.Plumbing;
using ToSic.Sxc.Blocks;
using ToSic.Sxc.Context;
using ToSic.Sxc.Mvc.Dev;


namespace ToSic.Sxc.Mvc.RazorPages.Exp
{
    public partial class PageBaseLoadingBlogOnly
    {
        #region DynCode 

        public Sxc.Code.IDynamicCodeRoot _DynCodeRoot => _dynCode ??= HttpContext.RequestServices.Build<Code.DynamicCodeRoot>().Init(Block, Log);
        private Sxc.Code.IDynamicCodeRoot _dynCode;
        #endregion
        public IBlock Block 
        {
            get
            {
                if (_blockLoaded) return _block;
                _blockLoaded = true;

                var ctx = GetService<IContextOfBlock>();
                ctx.Init(Log);
                ctx.Site.Init(TestIds.PrimaryZone);
                ctx.Site.Init(0);

                //var context = new ContextOfBlock(
                //    ServiceProvider,
                //    ServiceProvider.Build<ISite>().Init(TestIds.PrimaryZone),
                //    new MvcUser()
                //).Init(
                //    ServiceProvider.Build<SxcPage>().Init(0),
                //    new MvcContainer(),
                //    new BlockPublishingState()
                //    );
                _block = GetService<BlockFromModule>().Init(ctx, Log);
                return _block;
            }
        }
        private IBlock _block;
        private bool _blockLoaded;

    }
}
