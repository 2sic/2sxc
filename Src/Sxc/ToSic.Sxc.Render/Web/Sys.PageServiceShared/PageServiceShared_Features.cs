namespace ToSic.Sxc.Web.Sys.PageServiceShared;

partial class PageServiceShared
{
    public IEnumerable<string> Activate(params string[] keys)
        => PageFeatures.Activate(keys);

}