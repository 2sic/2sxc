namespace ToSic.Sxc.Web.Internal.PageService;

partial class PageServiceShared
{
    public IEnumerable<string> Activate(params string[] keys) =>
        PageFeatures.Activate(keys);

}