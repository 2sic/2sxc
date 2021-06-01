namespace ToSic.Sxc.Web.PageService
{
    public interface IPageChangeApplicator
    {
        IPageService PageService { get; }
        int Apply();
    }
}
