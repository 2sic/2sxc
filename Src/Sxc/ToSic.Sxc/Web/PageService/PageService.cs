using ToSic.Eav.Documentation;
using ToSic.Sxc.Context;

namespace ToSic.Sxc.Web.PageService
{
    public partial class PageService: IPageService
    {
        public PageServiceShared PageServiceShared { get; }
        public IContextResolver CtxResolver { get; }

        public PageService(PageServiceShared pageServiceShared, IContextResolver ctxResolver)
        {
            PageServiceShared = pageServiceShared;
            CtxResolver = ctxResolver;
        }

        /// <summary>
        /// How the changes given to this object should be processed.
        /// </summary>
        [WorkInProgressApi("not final yet")]
        public PageChangeModes ChangeMode { get; set; } = PageChangeModes.Auto;
    }
}
