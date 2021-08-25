using ToSic.Eav.Documentation;

namespace ToSic.Sxc.Web.PageService
{
    public partial class PageService: IPageService
    {
        public PageServiceShared PageServiceShared { get; }

        public PageService(IPageFeatures features, PageServiceShared pageServiceShared)
        {
            PageServiceShared = pageServiceShared;
            Features = features.Init(this);
        }

        /// <summary>
        /// How the changes given to this object should be processed.
        /// </summary>
        [WorkInProgressApi("not final yet")]
        public PageChangeModes ChangeMode { get; set; } = PageChangeModes.Auto;
    }
}
