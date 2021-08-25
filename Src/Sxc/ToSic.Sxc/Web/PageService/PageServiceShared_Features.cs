namespace ToSic.Sxc.Web.PageService
{
    public partial class PageServiceShared
    {
        public void Activate(params string[] keys) => Features.Activate(keys);

        public IPageFeatures Features { get; }

    }
}
