using ToSic.Eav.Documentation;
using ToSic.Eav.Logging;
using ToSic.Sxc.Code;

namespace ToSic.Sxc.Web.PageService
{
    public partial class PageService: HasLog, IPageService, INeedsCodeRoot
    {
        public PageServiceShared PageServiceShared { get; }

        public PageService(PageServiceShared pageServiceShared): base("2sxc.PgeSrv")
        {
            PageServiceShared = pageServiceShared;
        }

        public void AddBlockContext(IDynamicCodeRoot codeRoot)
        {
            CodeRoot = codeRoot;
            Log.LinkTo(codeRoot?.Log);
            Log.Call(message: $"Linked {nameof(PageService)}")(null);
        }

        public IDynamicCodeRoot CodeRoot;

        /// <summary>
        /// How the changes given to this object should be processed.
        /// </summary>
        [WorkInProgressApi("not final yet")]
        public PageChangeModes ChangeMode { get; set; } = PageChangeModes.Auto;

    }
}
